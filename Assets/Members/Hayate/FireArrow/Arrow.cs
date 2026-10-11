using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = GameManager.Instance.Player;
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
