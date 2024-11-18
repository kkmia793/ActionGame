using System;
using UnityEngine;
using Zenject.SpaceFighter;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool _isGameOver;
    private PlayerState _playerState;
    private float _totalDistance;

    [SerializeField] private GameSettings gameSettings; 

    public event Action<int> OnDifficultyLevelChanged;

    private int _currentDifficultyLevel = 1;
    public int CurrentDifficultyLevel => _currentDifficultyLevel;

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

    private void Start()
    {
        InitializePlayerState();
    }

    private void InitializePlayerState()
    {
        var player = FindObjectOfType<PlayerCharacter>();
        if (player != null)
        {
            _playerState = new PlayerState(player, gameSettings.FallThreshold, gameSettings.SpeedThreshold);
            _playerState.OnStateChanged += HandleGameOver;
        }
    }

    private void Update()
    {
        if (_isGameOver) return;

        UpdateDistance();
        CheckDifficultyIncrease();
        _playerState?.UpdateState();
    }

    private void UpdateDistance()
    {
        _totalDistance = _playerState.CalculatePlayerTotalDistance();
        UIManager.Instance.UpdateDistance(_totalDistance);
    }

    private void CheckDifficultyIncrease()
    {
        int newLevel = Mathf.FloorToInt(_totalDistance / gameSettings.DistancePerLevel) + 1;
        if (newLevel > _currentDifficultyLevel)
        {
            _currentDifficultyLevel = newLevel;
            _playerState.SetPlayerMoveSpeed(gameSettings.InitialSpeed + newLevel * gameSettings.SpeedIncreasePerLevel);
            OnDifficultyLevelChanged?.Invoke(newLevel);
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
        _isGameOver = false;
        _currentDifficultyLevel = 1;
        _totalDistance = 0;
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.RestartCurrentScene();
    }
}