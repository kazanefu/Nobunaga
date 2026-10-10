using UnityEngine;

public class CountdownFreezer : MonoBehaviour
{
    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // カウントダウン中は 0、それ以外は 1
        Time.timeScale = gm.CurrentGameState == GameManager.GameState.Countdown ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f; // シーン遷移で 0 のまま残らないように
    }
}