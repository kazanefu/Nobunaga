using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    [System.Serializable]
    private class FireType
    {
        public GameObject prefab;
        [Min(0f)] public float probability = 1f;
    }

    [SerializeField] private GameObject normalFirePrefab;
    [SerializeField] private GameObject blueFirePrefab;
    [SerializeField] private Player player;
    [SerializeField, Min(0f)] private float normalFireProbability = 1f;
    [SerializeField, Min(0f)] private float blueFireProbability = 1f;
    [SerializeField] private GameObject firePredicatorPrefab;
    [SerializeField] private FireType[] fireTypes;
    [SerializeField] private Vector2 stageSize = new Vector2(20f, 20f);
    [SerializeField] private Vector2 stageCenter;
    [SerializeField, Min(0.01f)] private float minFireSize = 1f;
    [SerializeField, Min(0.01f)] private float maxFireSize = 2f;
    [SerializeField, Min(0f)] private float warningDuration = 1.5f;
    [SerializeField, Min(0f)] private float fireLifetime = 5f;
    [SerializeField, Min(0.01f)] private float minSpawnInterval = 2f;
    [SerializeField, Min(0.01f)] private float maxSpawnInterval = 4f;
    [SerializeField, Min(0f)] private float difficultyRampDuration = 60f;
    [SerializeField, Min(1)] private int initialPoolSize = 5;
    [SerializeField, Min(1)] private int maxPoolSize = 50;

    private float spawnTimer;
    private float elapsedTime;
    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new();

    private void Start()
    {
        ScheduleNextFire();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        elapsedTime += Time.deltaTime;
        if (spawnTimer > 0f)
        {
            return;
        }

        SpawnWarning();
        ScheduleNextFire();
    }

    private void SpawnWarning()
    {
        Vector3 position = GetRandomStagePosition();
        float size = Random.Range(Mathf.Min(minFireSize, maxFireSize), Mathf.Max(minFireSize, maxFireSize));
        GameObject warning = null;

        if (firePredicatorPrefab != null)
        {
            warning = GetFromPool(firePredicatorPrefab);
            warning.transform.SetPositionAndRotation(position, transform.rotation);
            warning.transform.localScale = Vector3.Scale(warning.transform.localScale, new Vector3(size, 1f, size));
        }

        StartCoroutine(SpawnFireAfterWarning(position, size, warning));
    }

    private System.Collections.IEnumerator SpawnFireAfterWarning(Vector3 position, float size, GameObject warning)
    {
        yield return new WaitForSeconds(warningDuration);

        if (warning != null)
        {
            ReleaseToPool(firePredicatorPrefab, warning);
        }

        GameObject firePrefab = SelectFirePrefab();
        if (firePrefab == null)
        {
            Debug.LogWarning("No fire prefab is assigned to FireSpawner.");
            yield break;
        }

        GameObject fire = GetFromPool(firePrefab);
        fire.transform.SetPositionAndRotation(position, transform.rotation);
        Fire fireComponent = fire.GetComponent<Fire>();
        Vector3 fireScale = Vector3.Scale(fire.transform.localScale, new Vector3(size, 1f, size));
        if (fireComponent != null)
        {
            fireComponent.Initialize(fireScale, player, () => ReleaseToPool(firePrefab, fire));
        }
        else
        {
            fire.transform.localScale = fireScale;
        }
        StartCoroutine(ReturnToPoolAfterLifetime(firePrefab, fire, fireLifetime));
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
        {
            pool = new ObjectPool<GameObject>(
                () => Instantiate(prefab),
                instance => instance.SetActive(true),
                instance => instance.SetActive(false),
                instance => Destroy(instance),
                true,
                initialPoolSize,
                Mathf.Max(initialPoolSize, maxPoolSize));
            pools.Add(prefab, pool);
        }

        GameObject instance = pool.Get();
        instance.transform.localScale = prefab.transform.localScale;
        return instance;
    }

    private void ReleaseToPool(GameObject prefab, GameObject instance)
    {
        if (instance == null || !pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
        {
            return;
        }

        pool.Release(instance);
    }

    private System.Collections.IEnumerator ReturnToPoolAfterLifetime(
        GameObject prefab, GameObject instance, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (instance != null && instance.activeSelf)
        {
            ReleaseToPool(prefab, instance);
        }
    }

    private void OnDestroy()
    {
        foreach (ObjectPool<GameObject> pool in pools.Values)
        {
            pool.Clear();
        }
        pools.Clear();
    }

    private GameObject SelectFirePrefab()
    {
        float totalProbability = 0f;
        if (fireTypes != null)
        {
            foreach (FireType fireType in fireTypes)
            {
                if (fireType != null && fireType.prefab != null && fireType.probability > 0f)
                {
                    totalProbability += fireType.probability;
                }
            }
        }

        if (totalProbability > 0f)
        {
            float selection = Random.Range(0f, totalProbability);
            foreach (FireType fireType in fireTypes)
            {
                if (fireType == null || fireType.prefab == null || fireType.probability <= 0f)
                {
                    continue;
                }

                selection -= fireType.probability;
                if (selection <= 0f)
                {
                    return fireType.prefab;
                }
            }
        }

        float normalProbability = normalFirePrefab != null ? normalFireProbability : 0f;
        float blueProbability = blueFirePrefab != null ? blueFireProbability : 0f;
        if (normalProbability + blueProbability <= 0f)
        {
            return null;
        }

        return Random.value * (normalProbability + blueProbability) < normalProbability
            ? normalFirePrefab
            : blueFirePrefab;
    }

    private void ScheduleNextFire()
    {
        float progress = difficultyRampDuration <= 0f
            ? 1f
            : Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        float interval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, 1f - progress);
        spawnTimer = Random.Range(Mathf.Max(0.01f, interval * 0.75f), Mathf.Max(0.01f, interval * 1.25f));
    }

    private Vector3 GetRandomStagePosition()
    {
        Vector2 halfSize = new Vector2(Mathf.Max(0.01f, Mathf.Abs(stageSize.x) * 0.5f),
            Mathf.Max(0.01f, Mathf.Abs(stageSize.y) * 0.5f));
        Vector3 localPosition = new Vector3(
            stageCenter.x + Random.Range(-halfSize.x, halfSize.x),
            0f,
            stageCenter.y + Random.Range(-halfSize.y, halfSize.y));
        return transform.TransformPoint(localPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 halfSize = new Vector2(Mathf.Max(0.01f, Mathf.Abs(stageSize.x) * 0.5f),
            Mathf.Max(0.01f, Mathf.Abs(stageSize.y) * 0.5f));
        Vector3 bottomLeft = transform.TransformPoint(new Vector3(stageCenter.x - halfSize.x, 0f, stageCenter.y - halfSize.y));
        Vector3 bottomRight = transform.TransformPoint(new Vector3(stageCenter.x + halfSize.x, 0f, stageCenter.y - halfSize.y));
        Vector3 topRight = transform.TransformPoint(new Vector3(stageCenter.x + halfSize.x, 0f, stageCenter.y + halfSize.y));
        Vector3 topLeft = transform.TransformPoint(new Vector3(stageCenter.x - halfSize.x, 0f, stageCenter.y + halfSize.y));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
