using LitMotion;
using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float _motionTime = 3f;
    [SerializeField] private float _debugSeconds = 62.5f; // GameManager が無いときだけ使う

    private TextMeshProUGUI _scoreText;

    // カウントアップ完了時に、最終タイム(100分の1秒)を渡して発火
    public event Action<int> OnScoreFinished;

    private void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        _scoreText.text = TimeFormat.Format(0);
    }

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        float seconds = gm != null ? gm.GameTimer : _debugSeconds;
        GetScore(seconds);
    }

    public void GetScore(float seconds)
    {
        int finalScore = Mathf.RoundToInt(seconds * 100f);

        LMotion.Create(0, finalScore, _motionTime)
            .WithEase(Ease.OutQuad)
            .WithOnComplete(() =>
            {
                OnScoreFinished?.Invoke(finalScore);
                DecidedScoreMotion();
            })
            .Bind(v => _scoreText.text = TimeFormat.Format(v))
            .AddTo(gameObject);
    }

    private void DecidedScoreMotion()
    {
        RectTransform rt = _scoreText.rectTransform;

        LMotion.Create(1.4f, 1f, 3f)
            .WithEase(Ease.OutBack)
            .Bind(s => rt.localScale = Vector3.one * s)
            .AddTo(gameObject);
    }
}