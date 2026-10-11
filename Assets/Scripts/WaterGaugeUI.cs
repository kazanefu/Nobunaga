using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class WaterGaugeUI : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponStage
    {
        public int fromLevel;   // このレベル以上で使う
        public Sprite sprite;
    }

    [Header("Lv.N 表示(0番 = Lv.1 ... 19番 = Lv.20)")]
    [SerializeField] private Image _levelImage;
    [SerializeField] private Sprite[] _levelSprites;

    [Header("円内の武器(並び順は自由。fromLevel が一番近いものが選ばれる)")]
    [SerializeField] private Image _weaponImage;
    [SerializeField] private WeaponStage[] _weaponStages;

    [Header("ゲージ(Image Type = Filled / Vertical / Bottom)")]
    [SerializeField] private Image _waterFill;
    [SerializeField] private float _fillTime = 0.5f;    // 空 → 満タン
    [SerializeField] private float _resetTime = 0.15f;  // 満タン → 空
    [SerializeField] private Ease _fillEase = Ease.OutQuad;

    [Header("レベルアップ演出")]
    [SerializeField] private float _popScale = 1.3f;
    [SerializeField] private float _popTime = 0.3f;

    private Player _player;
    private bool _initialized;
    private int _lastLevel;
    private int _pendingFills;      // これから再生するレベルアップ演出の回数
    private bool _playing;
    private MotionHandle _fillHandle;
    private MotionHandle _popHandle;

    private void Update()
    {
        if (_player == null)
        {
            _player = FindFirstObjectByType<Player>();
            if (_player == null) return;
        }

        int level = _player.playerStatus.level;

        if (!_initialized)
        {
            _initialized = true;
            _lastLevel = level;
            ApplyLevel(level, false);
            SetFill(0f);
            return;
        }

        if (level == _lastLevel) return;

        int gained = level - _lastLevel;
        _lastLevel = level;
        ApplyLevel(level, gained > 0);

        if (gained > 0)
        {
            _pendingFills += gained;
            if (!_playing) PlayNextFill();
        }
    }

    private void PlayNextFill()
    {
        if (_pendingFills <= 0)
        {
            _playing = false;
            return;
        }

        _playing = true;
        _pendingFills--;

        SetFill(0f);
        _fillHandle = LMotion.Create(0f, 1f, _fillTime)
            .WithEase(_fillEase)
            .WithOnComplete(() =>
            {
                _fillHandle = LMotion.Create(1f, 0f, _resetTime)
                    .WithOnComplete(PlayNextFill)
                    .Bind(SetFill)
                    .AddTo(this);
            })
            .Bind(SetFill)
            .AddTo(this);
    }

    private void SetFill(float rate)
    {
        if (_waterFill != null) _waterFill.fillAmount = Mathf.Clamp01(rate);
    }

    private void ApplyLevel(int level, bool pop)
    {
        if (_levelImage != null && _levelSprites != null && _levelSprites.Length > 0)
        {
            _levelImage.sprite = _levelSprites[Mathf.Clamp(level - 1, 0, _levelSprites.Length - 1)];
        }

        if (_weaponImage != null)
        {
            Sprite weapon = PickWeapon(level);
            if (weapon != null) _weaponImage.sprite = weapon;
        }

        if (pop) PlayPop();
    }

    private Sprite PickWeapon(int level)
    {
        if (_weaponStages == null) return null;

        Sprite result = null;
        int best = int.MinValue;
        foreach (WeaponStage stage in _weaponStages)
        {
            if (stage.sprite != null && stage.fromLevel <= level && stage.fromLevel > best)
            {
                best = stage.fromLevel;
                result = stage.sprite;
            }
        }
        return result;
    }

    private void PlayPop()
    {
        if (_weaponImage == null) return;
        if (_popHandle.IsActive()) _popHandle.Cancel();

        RectTransform rt = _weaponImage.rectTransform;
        _popHandle = LMotion.Create(_popScale, 1f, _popTime)
            .WithEase(Ease.OutBack)
            .Bind(s => rt.localScale = Vector3.one * s)
            .AddTo(this);
    }
}