using LitMotion;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [Header("対象")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private CanvasGroup _group;   // Panel に付けた CanvasGroup

    [Header("演出")]
    [SerializeField] private float _time = 0.25f;
    [SerializeField] private float _startScale = 0.8f;

    public bool IsOpen { get; private set; }

    private MotionHandle _handle;

    private void Awake()
    {
        _panel.SetActive(false);
        IsOpen = false;
    }

    // ボタンの OnClick から呼ぶ
    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        _panel.SetActive(true);
        Play(0f, 1f, _startScale, 1f, Ease.OutBack, null);
    }

    // 閉じるボタンの OnClick から呼ぶ
    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Play(1f, 0f, 1f, _startScale, Ease.InQuad, () => _panel.SetActive(false));
    }

    private void Play(float fromAlpha, float toAlpha, float fromScale, float toScale,
                      Ease ease, System.Action onComplete)
    {
        if (_handle.IsActive()) _handle.Cancel();

        RectTransform rt = (RectTransform)_panel.transform;

        _handle = LMotion.Create(0f, 1f, _time)
            .WithEase(ease)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .WithOnComplete(() => onComplete?.Invoke())
            .Bind(t =>
            {
                _group.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                rt.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, t);
            })
            .AddTo(this);
    }
}