using UnityEngine;


namespace HeatingSolutionsInaTestTube
{
    public class WaterFillController : MonoBehaviour
    {
        public float fillDuration = 3f;
        public float startScaleZ = 0f;
        public float endScaleZ = 1f;
    
        private Vector3 initialScale;
        private float timer = 0f;
        private bool isFilling = false;
    
        void Start()
        {
            initialScale = transform.localScale;
            SetFill(startScaleZ);
        }
    
        void Update()
        {
            if (!isFilling) return;
    
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fillDuration);
    
            float currentY = Mathf.Lerp(startScaleZ, endScaleZ, t);
            SetFill(currentY);
    
            if (t >= 1f)
                isFilling = false;
        }
    
        void SetFill(float z)
        {
            transform.localScale = new Vector3(
                initialScale.x,
                z,
                initialScale.y
            );
        }
    
        public void StartFill()
        {
            timer = 0f;
            isFilling = true;
        }
    
        public void StopFill()
        {
            isFilling = false;
        }
    }
    
}