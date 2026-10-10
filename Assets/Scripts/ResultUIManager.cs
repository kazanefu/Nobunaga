using TMPro;
using UnityEngine;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] private RankingManager _rankingManager;
    [SerializeField] private TextMeshProUGUI _rankingText;
    [SerializeField] private string _rankingKey = "testRanking";
    [SerializeField] private int _debugScore = 1000;

    private void Start()
    {
        ShowRanking(_debugScore); // デバッグ用
    }

    public void ShowRanking(int score)
    {
        int[] ranking = _rankingManager.RankingUpdate(_rankingKey, score);
        _rankingText.text = _rankingManager.RankingToText(ranking);
    }
}