using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStatus
    {
        public int hp;
        public int level;
    }
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Transform water;
    private int previousLevel = 0;
    [SerializeField]
    private int initialHp = 100; // Initial HP for the player
    [SerializeField]
    private float incleaseRate = 1.1f; // 10% increase in water scale for each level
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatus.hp = initialHp;
        previousLevel = playerStatus.level;
        SetWaterScale(1f + (playerStatus.level - 1) * incleaseRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStatus.level != previousLevel)
        {
            // 10% increase in water scale for each level
            SetWaterScale(1f + (playerStatus.level - 1) * incleaseRate);
            previousLevel = playerStatus.level;
        }
    }

    private void SetWaterScale(float scale)
    {
        if (water == null)
        {
            return;
        }
        water.localScale = new Vector3(scale, 1.0f, scale);
    }

    public void AddReward(int amount)
    {
        if (amount > 0)
        {
            playerStatus.level += amount;
        }
    }

    public void TakeDamage(int damage)
    {
        playerStatus.hp -= damage;
        if (playerStatus.hp <= 0)
        {
            // Handle player death
            Debug.Log("Player has died.");
        }
    }
}
