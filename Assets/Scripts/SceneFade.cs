using System;
using UnityEngine;
using LitMotion;
using Coffee.UIEffects;

[RequireComponent(typeof(UIEffect))]
public class SceneFade : MonoBehaviour
{
    [Header("シーンに入った時")]
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private float _inDelay = 1f;

    [Header("フェードの設定")]
    [SerializeField] private float _fadeDuration = 0.2f;
    [SerializeField] private Ease _inEase = Ease.OutCirc;
    [SerializeField] private Ease _outEase = Ease.InCirc;

    [Header("transitionRate の値")]
    [SerializeField] private float _beforeRate = 0f;
    [SerializeField] private float _afterRate = 1f;

    private UIEffect _effect;
    private MotionHandle _handle;

    // フェードにかかる時間
    public float Duration => _fadeDuration;

    private void Awake()
    {
        _effect = GetComponent<UIEffect>();

        SetRate(_beforeRate);
    }

    private void Start()
    {
        if (_playOnStart) PlayIn();
    }

    // 画面に入る
    public void PlayIn(Action onComplete = null)
    {
        Play(_beforeRate, _afterRate, _inDelay, _inEase, onComplete);
    }

    // 画面を閉じる
    public void PlayOut(Action onComplete = null)
    {
        Play(_afterRate, _beforeRate, 0f, _outEase, onComplete);
    }

    private void Play(float from, float to, float delay, Ease ease, Action onComplete)
    {
        Cancel();
        SetRate(from);

        _handle = LMotion.Create(from, to, _fadeDuration)
            .WithDelay(delay)
            .WithEase(ease)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .WithOnComplete(() => onComplete?.Invoke())
            .Bind(SetRate);
    }

    private void SetRate(float value)
    {
        if (_effect != null) _effect.transitionRate = value;
    }

    private void Cancel()
    {
        if (_handle.IsActive()) _handle.Cancel();
    }

    private void OnDisable()
    {
        Cancel();
    }
}