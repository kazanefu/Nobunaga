using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody _rb;
    private InputHandler _inputHandler;
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
    }

    void Move()
    {
        float yVelocity = _rb.linearVelocity.y;
        _rb.linearVelocity = _inputHandler.MoveDirection * moveSpeed + new Vector3(0f, yVelocity, 0f);
    }
}
