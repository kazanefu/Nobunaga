using System;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField, Min(0)] private int reward = 1;
    [SerializeField, Min(0f)] private float shrinkSpeed = 0.5f;
    [SerializeField, Min(0.01f)] private float minimumScale = 0.1f;
    [SerializeField, Min(0)] private int damagePerTick = 10;
    [SerializeField, Min(0.01f)] private float damageInterval = 1f;

    private Action returnToPool;
    private Player player;
    private Vector3 baseScale;
    private readonly HashSet<Collider> waterColliders = new();
    private readonly HashSet<Collider> playerColliders = new();
    private bool rewardGranted;
    private float damageTimer;

    public void Initialize(Vector3 scale, Player targetPlayer, Action releaseAction)
    {
        baseScale = scale;
        transform.localScale = baseScale;
        player = targetPlayer;
        returnToPool = releaseAction;
        waterColliders.Clear();
        playerColliders.Clear();
        rewardGranted = false;
        damageTimer = 0f;
    }

    private void Update()
    {
        if (waterColliders.Count > 0 && !rewardGranted)
        {
            float scale = transform.localScale.x - shrinkSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, baseScale.y, scale);
            if (scale <= minimumScale)
            {
                GrantRewardAndRelease();
                return;
            }
        }

        if (playerColliders.Count == 0 || player == null || damagePerTick <= 0)
        {
            return;
        }

        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0f)
        {
            player.TakeDamage(damagePerTick);
            damageTimer = damageInterval;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterColliders.Add(other);
        }
        else if (IsPlayerCollider(other))
        {
            playerColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterColliders.Remove(other);
        }
        else if (IsPlayerCollider(other))
        {
            playerColliders.Remove(other);
        }
    }

    private bool IsPlayerCollider(Collider other)
    {
        Player colliderPlayer = other.GetComponentInParent<Player>();
        if (colliderPlayer == null)
        {
            return false;
        }

        if (player == null)
        {
            player = colliderPlayer;
        }

        return colliderPlayer == player;
    }

    private void GrantRewardAndRelease()
    {
        rewardGranted = true;
        if (player != null)
        {
            player.AddReward(reward);
        }

        if (returnToPool != null)
        {
            returnToPool.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
