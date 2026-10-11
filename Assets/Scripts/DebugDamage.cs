using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDamage : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            FindFirstObjectByType<Player>()?.TakeDamage(_damage);
        }
    }
}