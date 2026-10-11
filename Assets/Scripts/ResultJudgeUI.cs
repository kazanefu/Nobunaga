using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class ResultJudgeUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private ScoreManager _scoreManager;

    [Header("判定(秒)。この秒数以上生き残ると「あっぱれ」")]
    [SerializeField] private float _clearSeconds = 60f;

    [Header("Sprite")]
    [SerializeField] private Sprite _clearSprite; // あっぱれ
    [SerializeField] private Sprite _failSprite;  // 残念
    [SerializeField] private bool _setNativeSize = false; // Spriteごとにサイズが違うときON

    [Header("演出")]
    [SerializeField] private float _popTime = 0.5f;
    [SerializeField] private float _startScale = 2f;

    private Image _image;
    private MotionHandle _scaleHandle;
    private MotionHandle _alphaHandle;

    private void Awake()
    {
        _image = GetComponent<Image>();
        SetAlpha(0f); // タイムのカウントアップが終わるまで隠す
    }

    private void OnEnable()
    {
        if (_scoreManager != null) _scoreManager.OnScoreFinished += Judge;
    }

    private void OnDisable()
    {
        if (_scoreManager != null) _scoreManager.OnScoreFinished -= Judge;
    }

    private void Judge(int score)
    {
        bool cleared = score >= _clearSeconds;
        _image.sprite = cleared ? _clearSprite : _failSprite;
        if (_setNativeSize) _image.SetNativeSize();
        PlayPop();
    }

    private void PlayPop()
    {
        if (_scaleHandle.IsActive()) _scaleHandle.Cancel();
        if (_alphaHandle.IsActive()) _alphaHandle.Cancel();

        RectTransform rt = _image.rectTransform;

        _scaleHandle = LMotion.Create(_startScale, 1f, _popTime)
            .WithEase(Ease.OutBack)
            .Bind(s => rt.localScale = Vector3.one * s)
            .AddTo(this);

        _alphaHandle = LMotion.Create(0f, 1f, _popTime * 0.5f)
            .Bind(SetAlpha)
            .AddTo(this);
    }

    private void SetAlpha(float a)
    {
        Color c = _image.color;
        c.a = a;
        _image.color = c;
    }
}