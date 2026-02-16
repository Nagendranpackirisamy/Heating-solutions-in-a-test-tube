using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AngleDragController : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Rotation Settings")]
    public float minAngle = 0f;
    public float maxAngle = 90f;
    public float dragSensitivity = 0.4f;

    [Header("Answer Settings")]
    public float correctAngle = 45f;
    public float tolerance = 2f;
    public float snapRange = 5f;

    [Header("UI Display")]
    public TMP_Text degreeText;
    public Image fillImage;

    [Header("Fill Settings")]
    public float baseFillAtZero = 0.3f;
    public float maxFill = 1f;

    [Header("UI Buttons")]
    public GameObject submitButton;
    public GameObject retryButton;

    [Header("Objects To Hide On Submit")]
    public GameObject[] objectsToHide;

    [Header("Show Only At 90 Degree")]
    public GameObject showOnlyAt90;

    [Header("Events")]
    public UnityEvent onSubmitSFX;   // Drag your SFX here

    private float currentAngle = 0f;
    private Vector2 lastMousePos;
    private float initialZRotation;

    private bool dragEnabled = true;

    void Start()
    {
        initialZRotation = transform.localEulerAngles.z;

        currentAngle = 0f;
        ApplyRotation();
        UpdateUI();

        if (submitButton != null) submitButton.SetActive(false);
        if (retryButton != null) retryButton.SetActive(false);

        if (showOnlyAt90 != null)
            showOnlyAt90.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;
        lastMousePos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;

        float deltaX = eventData.position.x - lastMousePos.x;

        currentAngle += deltaX * dragSensitivity;
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        ApplyRotation();
        UpdateUI();
        Check90DegreeObject();

        lastMousePos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;

        if (Mathf.Abs(currentAngle - correctAngle) <= snapRange)
        {
            currentAngle = correctAngle;
            ApplyRotation();
            UpdateUI();
        }

        // Snap to 90° if near max
        if (Mathf.Abs(currentAngle - maxAngle) <= snapRange)
        {
            currentAngle = maxAngle;
            ApplyRotation();
            UpdateUI();
        }

        CheckAnswer();
        Check90DegreeObject();
    }

    void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, initialZRotation - currentAngle);
    }

    void UpdateUI()
    {
        if (degreeText != null)
            degreeText.text = Mathf.RoundToInt(currentAngle) + "°";

        if (fillImage != null)
        {
            float normalized = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
            fillImage.fillAmount = Mathf.Lerp(baseFillAtZero, maxFill, normalized);
        }
    }

    void CheckAnswer()
    {
        float diff = Mathf.Abs(currentAngle - correctAngle);
        bool isCorrect = diff <= tolerance;

        if (submitButton != null)
            submitButton.SetActive(isCorrect);

        if (retryButton != null)
            retryButton.SetActive(!isCorrect);
    }

    void Check90DegreeObject()
    {
        if (showOnlyAt90 == null) return;

        bool isAt90 = Mathf.Abs(currentAngle - maxAngle) <= tolerance;

        showOnlyAt90.SetActive(isAt90);
    }

    public void Submit()
    {
        // Stop Drag
        dragEnabled = false;

        // Hide multiple objects
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Play SFX Event
        onSubmitSFX?.Invoke();
    }

    public void Retry()
    {
        dragEnabled = true;

        currentAngle = 0f;
        ApplyRotation();
        UpdateUI();

        if (submitButton != null) submitButton.SetActive(false);
        if (retryButton != null) retryButton.SetActive(false);

        if (showOnlyAt90 != null)
            showOnlyAt90.SetActive(false);
    }
}
