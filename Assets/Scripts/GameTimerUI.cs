using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int _lastSeconds = -1;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // カウントダウン中は前回のタイムが残っているので 0 扱いにする
        bool showTime = gm.CurrentGameState == GameManager.GameState.Playing
                     || gm.CurrentGameState == GameManager.GameState.Ending;
        int seconds = showTime ? Mathf.FloorToInt(gm.GameTimer) : 0;

         if (seconds == _lastSeconds) return;
         _lastSeconds = seconds;

         _text.text = $"{seconds / 60:00}:{seconds % 60:00}";

        //GameTimerをpublic float GameTimer => _gameTimer;にして、GameManagerからGameTimerを取得するように変更してもらえたら完成。
    }
}