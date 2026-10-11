using LitMotion;
using LitMotion.Extensions;
using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float _motionTime = 3f;
    [SerializeField] private float _debugScore = 1500f;
    private TextMeshProUGUI _scoreText;

    private float _lastSeconds = default;

    private MotionHandle _scoreMotion;
    // カウントアップ完了時に、最終スコアを渡して発火
    public event Action<int> OnScoreFinished;

    private void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        GameManager gm = GameManager.Instance;
        _lastSeconds = gm.GameTimer;
        GetScore(_lastSeconds);
    }

    private void Start()
    {
        GetScore(_debugScore); // デバッグ用
    }


    //シーンが変わったタイミングでイベントから呼ばれる想定
    //どの型で送られるのか未定なので要調整
    public void GetScore(float score)
    {
        int finalScore = Mathf.RoundToInt(score);

        LMotion.Create(0, finalScore, _motionTime)
            .WithOnComplete(() =>
            {
                OnScoreFinished?.Invoke(finalScore);
                DecidedScoreMotion();
            })
            .BindToText(_scoreText);
    }

    private void DecidedScoreMotion()
    {
        RectTransform rt = _scoreText.rectTransform;

        LMotion.Create(1.4f, 1f, 3f)
            .WithEase(Ease.OutBack)
            .Bind(time => rt.localScale = Vector3.one * time)
            .AddTo(gameObject);

    }
}