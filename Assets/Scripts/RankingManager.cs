using System;
using System.Linq;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    private const string DefaultRanking = "500,400,300,200,100";

    public int[] RankingUpdate(string rankingKey, int newScore)
    {
        int[] ranking = RankingLoad(rankingKey);
        int topN = ranking.Length;

        int[] updated = ranking
            .Append(newScore)
            .OrderByDescending(s => s)
            .Take(topN)
            .ToArray();

        PlayerPrefs.SetString(rankingKey, string.Join(",", updated));
        PlayerPrefs.Save();

        return updated;
    }

    public int[] RankingLoad(string rankingKey)
    {
        string text = PlayerPrefs.GetString(rankingKey, DefaultRanking);
        return text.Split(',').Select(int.Parse).ToArray();
    }

    public string RankingToText(int[] ranking)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < ranking.Length; i++)
        {
            sb.AppendLine($"{i + 1},：{ranking[i]}");
        }
        return sb.ToString();
    }
}