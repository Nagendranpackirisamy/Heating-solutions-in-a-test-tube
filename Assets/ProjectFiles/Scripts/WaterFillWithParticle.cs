using UnityEngine;
using System.Collections;

public class WaterFillWithParticle : MonoBehaviour
{
    [Header("Particle Object")]
    public GameObject particleObject;

    [Header("Water Object (center pivot)")]
    public Transform waterTransform;

    [Header("Fill Settings")]
    public float fillDuration = 2f;
    public float minYScale = 0f;
    public float maxYScale = 1f;

    private Coroutine fillRoutine;

    // Called from animation event
    public void StartFill()
    {
        if (particleObject != null)
            particleObject.SetActive(true);

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillWater());
    }

    // Called from animation event
    public void StopFill()
    {
        if (particleObject != null)
            particleObject.SetActive(false);
    }

    IEnumerator FillWater()
    {
        float elapsed = 0f;

        Vector3 baseScale = waterTransform.localScale;
        Vector3 basePos = waterTransform.localPosition;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fillDuration;

            float yScale = Mathf.Lerp(minYScale, maxYScale, t);

            // Scale
            Vector3 scale = baseScale;
            scale.y = yScale;
            waterTransform.localScale = scale;

            // Offset upward to keep bottom fixed
            Vector3 pos = basePos;
            pos.y = (yScale - minYScale) * 0.5f;
            waterTransform.localPosition = pos;

            yield return null;
        }
    }
}