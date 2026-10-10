using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUIController : MonoBehaviour
{
    [SerializeField] private float _hpChangeDuration = 0.5f;
    [SerializeField] private Ease _ease = Ease.OutQuad;

    private Slider _hpSlider;
    private Player _player;
    private int _lastHP;
    private bool _initialized;
    private MotionHandle _handle;

    private void Awake()
    {
        _hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        // Player がまだ見つかっていなければ探す(見つかるまで毎フレーム)
        if (_player == null)
        {
            _player = FindFirstObjectByType<Player>();
            if (_player == null) return;
        }

        int hp = _player.playerStatus.hp;

        if (!_initialized)
        {
            if (hp <= 0) return;
            _hpSlider.maxValue = hp;
            _hpSlider.value = hp;
            _lastHP = hp;
            _initialized = true;
            return;
        }

        if (hp == _lastHP) return;
        _lastHP = hp;

        if (_handle.IsActive()) _handle.Cancel();

        _handle = LMotion.Create(_hpSlider.value, Mathf.Max(hp, 0), _hpChangeDuration)
            .WithEase(_ease)
            .Bind(value => _hpSlider.value = value)
            .AddTo(this);
    }
}