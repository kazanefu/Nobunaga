using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector3 MoveDirection { get; private set; }

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput?.Move.Enable();
    }

    private void Update()
    {
        Vector2 inputDirection = _playerInput.Move.Direction.ReadValue<Vector2>();
        MoveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
    }

    private void OnDisable()
    {
        _playerInput?.Move.Disable();
        MoveDirection = Vector3.zero;
    }

    private void OnDestroy()
    {
        _playerInput?.Dispose();
    }
}
