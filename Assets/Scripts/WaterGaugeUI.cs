using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class WaterGaugeUI : MonoBehaviour
{
    [System.Serializable]
    public struct ToolSprite
    {
        public Player.ToolType tool;   // Player 側の enum をそのまま使う
        public Sprite sprite;
    }

    [Header("Lv.N 表示(0番 = Lv.1 ... 19番 = Lv.20)")]
    [SerializeField] private Image _levelImage;
    [SerializeField] private Sprite[] _levelSprites;

    [Header("円内の武器(Player.currentTool に対応する絵)")]
    [SerializeField] private Image _weaponImage;
    [SerializeField] private ToolSprite[] _toolSprites;

    [Header("ゲージ(Image Type = Filled / Vertical / Bottom)")]
    [SerializeField] private Image _waterFill;
    [SerializeField] private float _fillTime = 0.5f;    // 空 → 満タン
    [SerializeField] private float _resetTime = 0.15f;  // 満タン → 空
    [SerializeField] private Ease _fillEase = Ease.OutQuad;

    [Header("武器が変わったときの演出")]
    [SerializeField] private float _popScale = 1.3f;
    [SerializeField] private float _popTime = 0.3f;

    private Player _player;
    private bool _initialized;
    private int _lastLevel;
    private Player.ToolType _lastTool;
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
        Player.ToolType tool = _player.currentTool;

        if (!_initialized)
        {
            _initialized = true;
            _lastLevel = level;
            _lastTool = tool;
            ApplyLevel(level);
            ApplyTool(tool, false);
            SetFill(0f);
            return;
        }

        // 武器:Player が決めた currentTool が変わったら差し替える
        if (tool != _lastTool)
        {
            _lastTool = tool;
            ApplyTool(tool, true);
        }

        // レベル:表示とゲージ演出
        if (level == _lastLevel) return;

        int gained = level - _lastLevel;
        _lastLevel = level;
        ApplyLevel(level);

        if (gained > 0)
        {
            _pendingFills += gained;
            if (!_playing) PlayNextFill();
        }
    }

    // ---- ゲージ:満タン → 空 を回数ぶん繰り返す ----
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

    // ---- レベル表示 ----
    private void ApplyLevel(int level)
    {
        if (_levelImage == null || _levelSprites == null || _levelSprites.Length == 0) return;
        _levelImage.sprite = _levelSprites[Mathf.Clamp(level - 1, 0, _levelSprites.Length - 1)];
    }

    // ---- 武器 ----
    private void ApplyTool(Player.ToolType tool, bool pop)
    {
        if (_weaponImage == null || _toolSprites == null) return;

        foreach (ToolSprite entry in _toolSprites)
        {
            if (entry.tool != tool || entry.sprite == null) continue;

            _weaponImage.sprite = entry.sprite;
            if (pop) PlayPop();
            return;
        }

        Debug.LogWarning($"WaterGaugeUI: {tool} の Sprite が設定されていません", this);
    }

    private void PlayPop()
    {
        if (_popHandle.IsActive()) _popHandle.Cancel();

        RectTransform rt = _weaponImage.rectTransform;
        _popHandle = LMotion.Create(_popScale, 1f, _popTime)
            .WithEase(Ease.OutBack)
            .Bind(s => rt.localScale = Vector3.one * s)
            .AddTo(this);
    }
}