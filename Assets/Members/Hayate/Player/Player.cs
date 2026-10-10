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
    private float incleaseRate = 1.1f; // 10% increase in water scale for each level
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
}
