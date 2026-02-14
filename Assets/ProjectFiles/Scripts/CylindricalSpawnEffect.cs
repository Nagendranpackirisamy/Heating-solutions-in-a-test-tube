using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindricalSpawnEffect : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private int spawnCount = 25;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.1f;

    [Header("Default Trigger Values (For UI Button)")]
    [SerializeField] private float defaultDelay = 0f;
    [SerializeField] private float defaultDuration = 3f;

    private Coroutine runningRoutine;
    private readonly List<GameObject> spawnedObjects = new();

    // =========================
    // PUBLIC API
    // =========================

    /// <summary>
    /// Start effect with custom delay and duration.
    /// </summary>
    public void StartEffect(float delaySeconds, float durationSeconds)
    {
        StopEffect(); // Ensure no stacking coroutines
        runningRoutine = StartCoroutine(StartEffectRoutine(delaySeconds, durationSeconds));
    }

    /// <summary>
    /// UI-friendly trigger (no parameters required).
    /// </summary>
    public void TriggerEffect()
    {
        StartEffect(defaultDelay, defaultDuration);
    }

    /// <summary>
    /// Stop effect immediately.
    /// </summary>
    public void StopEffect()
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }

        ClearSpawnedObjects();
    }

    // =========================
    // INTERNAL ROUTINES
    // =========================

    private IEnumerator StartEffectRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        yield return SpawnRoutine(duration);

        StopEffect();
    }

    private IEnumerator SpawnRoutine(float duration)
    {
        float timer = 0f;
        int spawned = 0;

        while (timer < duration && spawned < spawnCount)
        {
            SpawnOne();
            spawned++;

            yield return new WaitForSeconds(spawnInterval);
            timer += spawnInterval;
        }
    }

    private void SpawnOne()
    {
        Vector3 randomPos = GetRandomPointInCylinder();

        GameObject obj = Instantiate(
            prefab,
            transform.position + randomPos,
            Quaternion.identity,
            transform
        );

        spawnedObjects.Add(obj);
    }

    private Vector3 GetRandomPointInCylinder()
    {
        float randomRadius = Random.Range(0f, radius);
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        float x = randomRadius * Mathf.Cos(randomAngle);
        float z = randomRadius * Mathf.Sin(randomAngle);
        float y = Random.Range(0f, height);

        return new Vector3(x, y, z);
    }

    private void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // Bottom circle
        Gizmos.DrawWireSphere(transform.position, radius);

        // Top circle
        Gizmos.DrawWireSphere(transform.position + Vector3.up * height, radius);

        // Vertical line
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * height
        );
    }
#endif
}
