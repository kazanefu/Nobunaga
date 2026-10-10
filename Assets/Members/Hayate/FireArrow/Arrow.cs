using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTrigerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = GameManager.Instance.Player;
            if (player == null) return;
            GameManager.Instance.Player.TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }
}
