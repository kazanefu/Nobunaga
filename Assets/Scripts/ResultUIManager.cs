using System;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class ResultUIManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private RankingManager _rankingManager;
    [SerializeField] private string _rankingKey = "testRanking";

    [Header("ランキング行(1位から順に)")]
    [SerializeField] private TextMeshProUGUI[] _rankRows;

    [Header("演出")]
    [SerializeField] private float _rowInterval = 0.3f;
    [SerializeField] private float _rowFadeTime = 0.3f;
    [SerializeField] private float _slideDistance = 30f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.yellow;

    private const string NewTag = " <color=#FF4040>NEW!</color>";

    private void Awake()
    {
        // 演出前は全行を非表示にしておく
        foreach (var row in _rankRows)
        {
            row.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        _scoreManager.OnScoreFinished += ShowRanking;
    }

    private void OnDisable()
    {
        _scoreManager.OnScoreFinished -= ShowRanking;
    }

    private void ShowRanking(int score)
    {
        RankingResult result = _rankingManager.RankingUpdate(_rankingKey, score);
        int count = Mathf.Min(_rankRows.Length, result.Scores.Length);

        for (int i = 0; i < count; i++)
        {
            bool isNew = i == result.NewRankIndex;
            TextMeshProUGUI row = _rankRows[i];

            row.text = $"{i + 1},  {result.Scores[i]}" + (isNew ? NewTag : "");
            row.color = isNew ? _highlightColor : _normalColor;
            row.alpha = 0f;

            RectTransform rt = row.rectTransform;
            Vector2 basePos = rt.anchoredPosition;

            LMotion.Create(0f, 1f, _rowFadeTime)
                .WithDelay(i * _rowInterval)
                .WithEase(Ease.OutQuad)
                .WithOnComplete(() =>
                {
                    if (isNew) PlayNewRankEffect(row);
                })
                .Bind(t =>
                {
                    row.alpha = t;
                    rt.anchoredPosition = basePos + Vector2.down * (_slideDistance * (1f - t));
                })
                .AddTo(gameObject);
        }
    }

    private void PlayNewRankEffect(TextMeshProUGUI row)
    {
        RectTransform rt = row.rectTransform;

        // 拡大してから戻る
        LMotion.Create(1.4f, 1f, 0.4f)
            .WithEase(Ease.OutBack)
            .Bind(time => rt.localScale = Vector3.one * time)
            .AddTo(gameObject);

        // 色をゆっくり点滅　今はループ
        LMotion.Create(0f, 1f, 0.5f)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(time => row.color = Color.Lerp(_highlightColor, Color.white, time))
            .AddTo(gameObject);
    }
}