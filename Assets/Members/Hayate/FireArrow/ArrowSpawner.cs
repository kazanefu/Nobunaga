using UnityEngine.Pool;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float emitForce = 10f;
    [SerializeField] private Vector2 stageSize = new Vector2(20f, 20f);
    [SerializeField] private Vector2 stageCenter;
    [SerializeField, Min(0.01f)] private float initialMinInterval = 2f;
    [SerializeField, Min(0.01f)] private float initialMaxInterval = 4f;
    [SerializeField, Min(0.01f)] private float finalMinInterval = 0.3f;
    [SerializeField, Min(0.01f)] private float finalMaxInterval = 1f;
    [SerializeField, Min(0f)] private float difficultyRampDuration = 60f;
    [SerializeField, Min(0f)] private float arrowLifetime = 5f;
    [SerializeField, Min(0f)] private float targetEdgeMargin = 0.5f;
    [SerializeField, Min(1)] private int initialPoolSize = 10;
    [SerializeField, Min(1)] private int maxPoolSize = 100;

    private float elapsedTime;
    private float spawnTimer;
    private ObjectPool<GameObject> arrowPool;

    private void Start()
    {
        arrowPool = new ObjectPool<GameObject>(
            CreateArrow,
            arrow => arrow.SetActive(true),
            arrow => arrow.SetActive(false),
            arrow => Destroy(arrow),
            true,
            initialPoolSize,
            Mathf.Max(initialPoolSize, maxPoolSize));
        ScheduleNextArrow();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        EmitArrow();
        ScheduleNextArrow();
        elapsedTime += Time.deltaTime;
    }

    private void EmitArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition = GetRandomEdgePosition();
        Vector3 targetPosition = GetRandomInnerPosition();
        Vector3 direction = targetPosition - spawnPosition;
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(direction.normalized, transform.up);
        GameObject arrow = arrowPool.Get();
        arrow.transform.SetPositionAndRotation(spawnPosition, rotation);
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.linearVelocity = Vector3.zero;
            arrowRb.angularVelocity = Vector3.zero;
            arrowRb.AddForce(direction.normalized * emitForce, ForceMode.Impulse);
        }
        StartCoroutine(ReturnArrowAfterLifetime(arrow, arrowLifetime));
    }

    private GameObject CreateArrow()
    {
        return Instantiate(arrowPrefab);
    }

    private System.Collections.IEnumerator ReturnArrowAfterLifetime(GameObject arrow, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (arrow != null && arrow.activeSelf)
        {
            arrowPool.Release(arrow);
        }
    }

    private void OnDestroy()
    {
        arrowPool?.Clear();
    }

    private void ScheduleNextArrow()
    {
        float progress = difficultyRampDuration <= 0f
            ? 1f
            : Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        float minInterval = Mathf.Lerp(initialMinInterval, finalMinInterval, progress);
        float maxInterval = Mathf.Lerp(initialMaxInterval, finalMaxInterval, progress);
        minInterval = Mathf.Max(0.01f, minInterval);
        maxInterval = Mathf.Max(minInterval, maxInterval);
        spawnTimer = Random.Range(minInterval, maxInterval);
    }

    private Vector3 GetRandomEdgePosition()
    {
        Vector2 halfSize = GetHalfStageSize();
        float edgePosition = Random.value;
        Vector2 localPosition;

        switch (Random.Range(0, 4))
        {
            case 0:
                localPosition = new Vector2(Mathf.Lerp(-halfSize.x, halfSize.x, edgePosition), -halfSize.y);
                break;
            case 1:
                localPosition = new Vector2(Mathf.Lerp(-halfSize.x, halfSize.x, edgePosition), halfSize.y);
                break;
            case 2:
                localPosition = new Vector2(-halfSize.x, Mathf.Lerp(-halfSize.y, halfSize.y, edgePosition));
                break;
            default:
                localPosition = new Vector2(halfSize.x, Mathf.Lerp(-halfSize.y, halfSize.y, edgePosition));
                break;
        }

        return LocalPlaneToWorld(localPosition);
    }

    private Vector3 GetRandomInnerPosition()
    {
        Vector2 halfSize = GetHalfStageSize();
        float marginX = Mathf.Min(targetEdgeMargin, halfSize.x);
        float marginY = Mathf.Min(targetEdgeMargin, halfSize.y);
        Vector2 localPosition = new Vector2(
            Random.Range(-halfSize.x + marginX, halfSize.x - marginX),
            Random.Range(-halfSize.y + marginY, halfSize.y - marginY));
        return LocalPlaneToWorld(localPosition);
    }

    private Vector2 GetHalfStageSize()
    {
        return new Vector2(Mathf.Max(0.01f, Mathf.Abs(stageSize.x) * 0.5f),
            Mathf.Max(0.01f, Mathf.Abs(stageSize.y) * 0.5f));
    }

    private Vector3 LocalPlaneToWorld(Vector2 localPosition)
    {
        Vector3 localPoint = new Vector3(stageCenter.x + localPosition.x, 0f, stageCenter.y + localPosition.y);
        return transform.TransformPoint(localPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 halfSize = GetHalfStageSize();
        Vector3 bottomLeft = LocalPlaneToWorld(new Vector2(-halfSize.x, -halfSize.y));
        Vector3 bottomRight = LocalPlaneToWorld(new Vector2(halfSize.x, -halfSize.y));
        Vector3 topRight = LocalPlaneToWorld(new Vector2(halfSize.x, halfSize.y));
        Vector3 topLeft = LocalPlaneToWorld(new Vector2(-halfSize.x, halfSize.y));

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
