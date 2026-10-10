using LitMotion;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CountdownSpriteUI : MonoBehaviour
{
    [Header("Sprite(0 = 始め, 1 = 壱, 2 = 弐, 3 = 参)")]
    [SerializeField] private Sprite[] _numberSprites;

    [Header("Spriteごとの表示倍率 XYZ(Spriteと同じ並び)")]
    [SerializeField]
    private Vector3[] _spriteScales =
    {
        new Vector3(1.5f, 1f, 1f), // 0 = 始め
        Vector3.one,               // 1 = 壱
        Vector3.one,               // 2 = 弐
        Vector3.one                // 3 = 参
    };

    [Header("演出")]
    [SerializeField] private float _popTime = 0.4f;
    [SerializeField] private float _startScale = 1.6f;
    [SerializeField] private float _startHoldTime = 0.8f;

    private Image _image;
    private int _lastCount = -1;
    private MotionHandle _scaleHandle;
    private MotionHandle _alphaHandle;
    private MotionHandle _hideHandle;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.CurrentGameState == GameManager.GameState.Countdown)
        {
            int count = gm.GetCountdownTimer();
            if (count == _lastCount) return;
            _lastCount = count;
            Show(count);
            return;
        }

        if (_lastCount > 0 && gm.CurrentGameState == GameManager.GameState.Playing)
        {
            _lastCount = 0;
            Show(0);

            if (_hideHandle.IsActive()) _hideHandle.Cancel();
            _hideHandle = LMotion.Create(0f, 0f, _startHoldTime)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithOnComplete(() =>
                {
                    _image.enabled = false;
                    _lastCount = -1;
                })
                .RunWithoutBinding()
                .AddTo(this);
            return;
        }

        if (_lastCount == 0) return;

        _image.enabled = false;
        _lastCount = -1;
    }

    private Vector3 GetScale(int index)
    {
        return (_spriteScales != null && index >= 0 && index < _spriteScales.Length)
            ? _spriteScales[index]
            : Vector3.one;
    }

    private void Show(int index)
    {
        if (index < 0 || index >= _numberSprites.Length) return;

        _image.sprite = _numberSprites[index];
        _image.enabled = true;
        PlayPop(index);
    }

    private void PlayPop(int index)
    {
        if (_scaleHandle.IsActive()) _scaleHandle.Cancel();
        if (_alphaHandle.IsActive()) _alphaHandle.Cancel();

        RectTransform rt = _image.rectTransform;
        Vector3 target = GetScale(index);

        _scaleHandle = LMotion.Create(target * _startScale, target, _popTime)
            .WithEase(Ease.OutBack)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .Bind(s => rt.localScale = s)
            .AddTo(this);

        _alphaHandle = LMotion.Create(0f, 1f, _popTime * 0.5f)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .Bind(a =>
            {
                Color c = _image.color;
                c.a = a;
                _image.color = c;
            })
            .AddTo(this);
    }
}