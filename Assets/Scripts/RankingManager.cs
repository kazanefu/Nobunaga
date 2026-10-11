using System.Linq;
using UnityEngine;

public readonly struct RankingResult
{
    public readonly int[] Scores;
    public readonly int NewRankIndex;
    public bool IsRankIn => NewRankIndex >= 0;

    public RankingResult(int[] scores, int newRankIndex)
    {
        Scores = scores;
        NewRankIndex = newRankIndex;
    }
}

public class RankingManager : MonoBehaviour
{
    private const string DefaultRanking = "0000,0000,0000,0000,0000"; 
    public RankingResult RankingUpdate(string rankingKey, int newScore)
    {
        int[] ranking = RankingLoad(rankingKey);
        int topN = ranking.Length;

        // 同点なら現在のスコアを上位に表示
        int rankIndex = ranking.Count(s => s > newScore);
        if (rankIndex >= topN) rankIndex = -1; // 圏外

        int[] updated = ranking
            .Append(newScore)
            .OrderByDescending(s => s)
            .Take(topN)
            .ToArray();

        PlayerPrefs.SetString(rankingKey, string.Join(",", updated));
        PlayerPrefs.Save();

        return new RankingResult(updated, rankIndex);
    }

    public int[] RankingLoad(string rankingKey)
    {
        string text = PlayerPrefs.GetString(rankingKey, DefaultRanking);
        return text.Split(',').Select(int.Parse).ToArray();
    }
}