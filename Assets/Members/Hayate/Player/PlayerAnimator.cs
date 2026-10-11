using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _playerController;
    [SerializeField] private Animator _animator;
    private PlayerController.MovementState _currentMovementState;
    private bool _hasPlayedState;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_playerController == null || _animator == null)
        {
            return;
        }

        PlayerController.MovementState movementState = _playerController.CurrentMovementState;
        if (_hasPlayedState && movementState == _currentMovementState)
        {
            return;
        }

        string stateName = $"Rifle {movementState}";
        int stateHash = Animator.StringToHash(stateName);
        if (!_animator.HasState(0, stateHash))
        {
            Debug.LogWarning($"Animator state not found: {stateName}", _animator);
            return;
        }

        _animator.Play(stateHash);
        _currentMovementState = movementState;
        _hasPlayedState = true;
    }
}
