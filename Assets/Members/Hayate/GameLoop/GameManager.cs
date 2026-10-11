using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Title,
        Countdown,
        Playing,
        Ending
    }
    [SerializeField]
    private string gameSceneName = "GameScene";
    [SerializeField]
    private string endingSceneName = "EndingScene";
    [SerializeField]
    private string titleSceneName = "TitleScene";
    [SerializeField]
    private float countdownMaxTime = 3f; // Countdown duration in seconds
    private float _gameStartCountdown = 3f; // Countdown duration in seconds
    private float _gameTimer = 0f;

    public float GameTimer => _gameTimer;
    public GameState CurrentGameState { get; private set; } = GameState.Title;

    public Player Player { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        TickTimers();
    }

    private void TickTimers()
    {
        if (CurrentGameState == GameState.Countdown)
        {
            _gameStartCountdown -= Time.unscaledDeltaTime;
            if (_gameStartCountdown <= 0f)
            {
                StartGame();
            }
        }
        else if (CurrentGameState == GameState.Playing)
        {
            _gameTimer += Time.deltaTime;
        }
    }

    public int GetCountdownTimer()
    {
        return Mathf.CeilToInt(_gameStartCountdown);
    }
    public float GetGameTimer()
    {
        return _gameTimer;
    }
    public void LoadGameScene()
    {
        CurrentGameState = GameState.Countdown;
        _gameStartCountdown = countdownMaxTime;
        AudioManager.Instance?.PlayBGM("InGame");
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        Player = FindFirstObjectByType<Player>();
        _gameTimer = 0f;
    }

    public void EndGame()
    {
        CurrentGameState = GameState.Ending;
        AudioManager.Instance?.PlayBGM("Result");
        SceneManager.LoadScene(endingSceneName);
        Debug.Log($"Game ended. Total time played: {_gameTimer} seconds.");
    }

    public void LoadTitleScene()
    {
        CurrentGameState = GameState.Title;
        AudioManager.Instance?.PlayBGM("Title");
        SceneManager.LoadScene(titleSceneName);
        Player = null;
    }
}
