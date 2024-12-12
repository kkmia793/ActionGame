using System;
using UnityEngine;
using Zenject; // Zenjectを使う場合

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool _isGameOver;
    private PlayerState _playerState;
    private DifficultyManager _difficultyManager;
    private float _totalDistance;

    [SerializeField] private GameSettings gameSettings; 

    public event Action<int> OnDifficultyLevelChanged;

    private int _currentDifficultyLevel = 1;
    public int CurrentDifficultyLevel => _currentDifficultyLevel;

    [Inject]
    private void Construct(PlayerState playerState, DifficultyManager difficultyManager)
    {
        _playerState = playerState;
        _difficultyManager = difficultyManager;
        _playerState.OnDistanceUpdated += UpdateDistance;
        _playerState.OnStateChanged += HandleGameOver;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    } 
    
    private async void Start()
    {
        await _playerState.StartMonitoringWithDelay(1f); 
    }

    private void Update()
    {
        if (_isGameOver) return;
        _playerState.UpdateState();
    }

    private void UpdateDistance(float distance)
    {
        _totalDistance = distance;
        CheckDifficultyIncrease();
        UIManager.Instance.UpdateDistance(_totalDistance);
    }

    private void CheckDifficultyIncrease()
    {
        int newLevel = _difficultyManager.CalculateDifficulty(_totalDistance, gameSettings.DistancePerLevel);
        if (newLevel > _currentDifficultyLevel)
        {
            _currentDifficultyLevel = newLevel;
            
            float newSpeed = _difficultyManager.GetPlayerSpeed(newLevel, gameSettings.InitialSpeed, gameSettings.SpeedIncreasePerLevel);
            
            newSpeed = Mathf.Min(newSpeed, gameSettings.MaxSpeed);
            
            _playerState.SetPlayerMoveSpeed(newSpeed);
            
            OnDifficultyLevelChanged?.Invoke(newLevel);

            Debug.Log($"New Difficulty Level: {_currentDifficultyLevel}, New Speed: {newSpeed}");
        }
    }

    private void HandleGameOver(PlayerState.State newState)
    {
        if (_isGameOver || newState != PlayerState.State.Dead) return;

        _isGameOver = true;
        Time.timeScale = 0f;
        UIManager.Instance.DisplayGameOverScreen();
    }

    public void RestartGame()
    {
        ResetGame();
        SceneTransitionManager.Instance.RestartCurrentScene();
    }

    private void ResetGame()
    {
        _isGameOver = false;
        _currentDifficultyLevel = 1;
        _totalDistance = 0;
        Time.timeScale = 1f;
        _playerState.ResetState();
    }
}