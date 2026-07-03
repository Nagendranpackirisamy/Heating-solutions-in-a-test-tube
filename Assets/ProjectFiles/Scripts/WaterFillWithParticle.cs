using UnityEngine;
using System.Collections;

namespace HeatingSolutionsInaTestTube
{
    public class WaterFillWithParticle : MonoBehaviour
    {
        [Header("Particle Object")]
        public GameObject particleObject;

        [Header("Water Object")]
        public Transform waterTransform;

        [Header("Fill Settings")]
        public float fillDuration = 2f;
        public float minZScale = 0f;
        public float maxZScale = 1f;

        private Coroutine fillRoutine;

        public void StartFill()
        {
            if (particleObject != null)
                particleObject.SetActive(true);

            if (fillRoutine != null)
                StopCoroutine(fillRoutine);

            fillRoutine = StartCoroutine(FillWater());
        }

        public void StopFill()
        {
            if (particleObject != null)
                particleObject.SetActive(false);
        }

        IEnumerator FillWater()
        {
            float elapsed = 0f;

            Vector3 baseScale = waterTransform.localScale;

            while (elapsed < fillDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fillDuration);

                float zScale = Mathf.Lerp(minZScale, maxZScale, t);

                Vector3 scale = baseScale;
                scale.z = zScale;

                waterTransform.localScale = scale;

                yield return null;
            }

            Vector3 finalScale = waterTransform.localScale;
            finalScale.z = maxZScale;
            waterTransform.localScale = finalScale;
        }
    }
}