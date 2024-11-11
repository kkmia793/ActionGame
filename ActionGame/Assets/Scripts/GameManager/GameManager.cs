using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks; // UniTaskを使用

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool _isGameOver;
    private PlayerState playerState;

    [SerializeField] private float fallThreshold = -10f; // 落下の閾値
    [SerializeField] private float speedThreshold = 1f;  // 速度の閾値
    [SerializeField] private float monitorDelay = 3f;    // 監視開始までの遅延時間

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
        var player = FindObjectOfType<PlayerCharacter>(); // シーン内のPlayerCharacterを検索
        if (player != null)
        {
            playerState = new PlayerState(player, fallThreshold, speedThreshold);
            playerState.OnStateChanged += HandleGameOver; // 状態変化イベントを購読
            await playerState.StartMonitoringWithDelay(monitorDelay); // 監視開始を遅延させる
        }
        else
        {
            Debug.LogError("PlayerCharacterが見つかりませんでした。");
        }
    }

    private void Update()
    {
        playerState?.UpdateState(); // PlayerStateの状態を毎フレーム更新
    }

    private void OnDisable()
    {
        if (playerState != null)
        {
            playerState.OnStateChanged -= HandleGameOver;
        }
    }

    private void HandleGameOver(PlayerState.State newState)
    {
        if (_isGameOver || newState != PlayerState.State.Dead) return;

        _isGameOver = true;
        Debug.Log("Game Over");

        // ゲームオーバー時に時間を停止
        Time.timeScale = 0f;

        // ゲームオーバー処理（例: UI表示やシーン遷移）
        //ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {
        Debug.Log("Displaying Game Over UI...");
        SceneManager.LoadScene("GameOverScene"); // ゲームオーバー画面への遷移
    }

    public void RestartGame()
    {
        _isGameOver = false;
        Time.timeScale = 1f; // 時間を再開
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再読み込み
    }

    public void LoadMainMenu()
    {
        _isGameOver = false;
        Time.timeScale = 1f; // 時間を再開
        SceneManager.LoadScene("MainMenu");
    }
}