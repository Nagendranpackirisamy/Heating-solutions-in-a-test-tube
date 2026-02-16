using UnityEngine;

public class WaterFillController : MonoBehaviour
{
    public float fillDuration = 3f;
    public float startScaleY = 0f;
    public float endScaleY = 1f;

    private Vector3 initialScale;
    private float timer = 0f;
    private bool isFilling = false;

    void Start()
    {
        initialScale = transform.localScale;
        SetFill(startScaleY);
    }

    void Update()
    {
        if (!isFilling) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fillDuration);

        float currentY = Mathf.Lerp(startScaleY, endScaleY, t);
        SetFill(currentY);

        if (t >= 1f)
            isFilling = false;
    }

    void SetFill(float y)
    {
        transform.localScale = new Vector3(
            initialScale.x,
            y,
            initialScale.z
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