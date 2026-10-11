using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody _rb;
    private InputHandler _inputHandler;
    [SerializeField] private Transform waterMuzzle;

    public enum MovementState
    {
        Idle,
        Walk,
        Run
    }
    public MovementState CurrentMovementState { get; private set; } = MovementState.Idle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        AimWaterMuzzleAtMouse();
        PlaySound();
    }

    void Move()
    {
        float yVelocity = _rb.linearVelocity.y;
        _rb.linearVelocity = _inputHandler.MoveDirection * moveSpeed + new Vector3(0f, yVelocity, 0f);
        CurrentMovementState = _inputHandler.MoveDirection.sqrMagnitude > 0f ? MovementState.Run : MovementState.Idle;
    }

    private void PlaySound()
    {
        if(CurrentMovementState == MovementState.Run)
        {
            // Play running sound
            AudioManager.Instance?.PlayLoopSE("Run");
        }
        else if(CurrentMovementState == MovementState.Idle)
        {
            // Stop running sound
            AudioManager.Instance?.StopLoopSE("Run");
        }
    }

    private void AimWaterMuzzleAtMouse()
    {
        if (waterMuzzle == null || Camera.main == null)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
        Plane movementPlane = new Plane(Vector3.up, waterMuzzle.position);

        if (!movementPlane.Raycast(mouseRay, out float distance))
        {
            return;
        }

        Vector3 targetPosition = mouseRay.GetPoint(distance);
        Vector3 direction = targetPosition - waterMuzzle.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0f)
        {
            waterMuzzle.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
