using System;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField, Min(0)] private int reward = 1;
    [SerializeField, Min(0f)] private float shrinkSpeed = 0.5f;
    [SerializeField, Min(0.01f)] private float minimumScale = 0.1f;

    private Action returnToPool;
    private Player player;
    private Vector3 baseScale;
    private readonly HashSet<Collider> waterColliders = new();
    private bool rewardGranted;

    public void Initialize(Vector3 scale, Player targetPlayer, Action releaseAction)
    {
        baseScale = scale;
        transform.localScale = baseScale;
        player = targetPlayer;
        returnToPool = releaseAction;
        waterColliders.Clear();
        rewardGranted = false;
    }

    private void Update()
    {
        if (waterColliders.Count == 0 || rewardGranted)
        {
            return;
        }

        float scale = transform.localScale.x - shrinkSpeed * Time.deltaTime;
        transform.localScale = new Vector3(scale, baseScale.y, scale);
        if (scale <= minimumScale)
        {
            GrantRewardAndRelease();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterColliders.Remove(other);
        }
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
