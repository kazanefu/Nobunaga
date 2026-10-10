using UnityEngine;

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
    public GameState CurrentGameState { get; private set; } = GameState.Title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TickTimers();
    }

    private void TickTimers()
    {
        if (CurrentGameState == GameState.Countdown)
        {
            _gameStartCountdown -= Time.deltaTime;
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
    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        CurrentGameState = GameState.Countdown;
        _gameStartCountdown = countdownMaxTime;
    }

    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        _gameTimer = 0f;
    }

    public void EndGame()
    {
        CurrentGameState = GameState.Ending;
        UnityEngine.SceneManagement.SceneManager.LoadScene(endingSceneName);
    }

    public void LoadTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        CurrentGameState = GameState.Title;
    }


}
