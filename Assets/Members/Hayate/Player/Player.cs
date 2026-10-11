using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStatus
    {
        public int hp;
        public int level;
    }
    [System.Serializable]
    public enum ToolType
    {
        Oke,
        Juu,
        Bazuka,
    }
    [SerializeField] public PlayerStatus playerStatus;
    [SerializeField] private Transform water;
    private int previousLevel = 0;
    [SerializeField]
    private int initialHp = 100; // Initial HP for the player
    [SerializeField]
    private float incleaseRate = 1.1f; // 10% increase in water scale for each level

    [SerializeField]
    private ParticleSystem waterParticleSystem; // Reference to the Particle System for water
    private float baseWaterStartSpeedMultiplier = 1f;
    private bool hasWaterStartSpeed;
    [SerializeField]
    private List<GameObject> Tools;
    public ToolType currentTool = ToolType.Oke; // Current tool type
    public int levelFirstThreshold = 5; // Level threshold for the first level
    public int levelSecondThreshold = 10; // Level threshold for the second level
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waterParticleSystem == null && water != null)
        {
            waterParticleSystem = water.GetComponentInChildren<ParticleSystem>(true);
        }

        if (waterParticleSystem != null)
        {
            baseWaterStartSpeedMultiplier =
                waterParticleSystem.main.startSpeedMultiplier;
            hasWaterStartSpeed = true;
        }

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
        UpdateTools();
    }
    private void UpdateTools()
    {
        if (playerStatus.level >= levelFirstThreshold)
        {
            currentTool = ToolType.Juu;
        }
        if (playerStatus.level >= levelSecondThreshold)
        {
            currentTool = ToolType.Bazuka;
        }
        switch (currentTool)
        {
            case ToolType.Oke:
                Tools[0].SetActive(true);
                Tools[1].SetActive(false);
                Tools[2].SetActive(false);
                break;
            case ToolType.Juu:
                Tools[0].SetActive(false);
                Tools[1].SetActive(true);
                Tools[2].SetActive(false);
                break;
            case ToolType.Bazuka:
                Tools[0].SetActive(false);
                Tools[1].SetActive(false);
                Tools[2].SetActive(true);
                break;
        }
    }


    private void SetWaterScale(float scale)
    {
        if (water == null)
        {
            return;
        }
        water.localScale = new Vector3(scale, 1.0f, scale);

        if (waterParticleSystem != null && hasWaterStartSpeed)
        {
            ParticleSystem.MainModule main = waterParticleSystem.main;
            main.startSpeedMultiplier = baseWaterStartSpeedMultiplier * scale;
        }
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
            GameManager.Instance.EndGame();
        }
    }

}
