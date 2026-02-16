using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SmoothHighlight3D : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = Color.red;
    public float blinkSpeed = 1.5f;
    public float intensity = 2.5f;

    [Header("Material (Assign Manually)")]
    public Material highlightMaterial;

    [Header("On Object Click (Inspector Event)")]
    public UnityEvent onClick;

    private Material material;
    private Color baseEmission;
    private Color baseColor;
    private bool isHighlightOn = true;

    void Start()
    {
        if (highlightMaterial == null)
        {
            Debug.LogError("SmoothHighlight3D: No material assigned!", this);
            enabled = false;
            return;
        }

        material = highlightMaterial;

        // Cache original values
        baseColor = material.color;

        // ----- MOBILE SAFE EMISSION SETUP -----
        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            baseEmission = material.GetColor("_EmissionColor");

            material.globalIlluminationFlags =
                MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        else
        {
            baseEmission = Color.black;
        }
    }

    void Update()
    {
        if (!isHighlightOn || material == null)
            return;

        float pulse = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
        float strength = pulse * intensity;

        // ✅ Emission path (desktop + supported mobile)
        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", highlightColor * strength);
        }
        else
        {
            // ✅ Fallback path (mobile safe)
            material.color = Color.Lerp(baseColor, highlightColor, pulse);
        }
    }

    // 🔥 Detect click on THIS 3D object
    void OnMouseDown()
    {
        onClick?.Invoke();
    }

    // Example function you can assign in Inspector
    public void TurnOffHighlight()
    {
        isHighlightOn = false;

        if (material == null) return;

        if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", baseEmission);

        material.color = baseColor;
    }

    void OnDisable()
    {
        if (material == null) return;

        if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", baseEmission);

        material.color = baseColor;
    }
}
