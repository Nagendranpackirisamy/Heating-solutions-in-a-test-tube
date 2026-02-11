using UnityEngine;
using UnityEngine.Events;

public class Drag3D : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zDistance;
    private bool isDragging = false;

    private Vector3 targetPosition;

    [Header("Initial Position Slot")]
    public Transform initialPositionSlot; // assign in Inspector

    [Header("Movement Limits (X & Y only)")]
    public Vector2 minXY;
    public Vector2 maxXY;

    [Header("Return Settings")]
    public float returnSpeed = 5f;

    [Header("Drag Events")]
    public UnityEvent onDragStart;
    public UnityEvent onDragging;
    public UnityEvent onDragEnd;

    void Start()
    {
        mainCamera = Camera.main;

        if (initialPositionSlot != null)
        {
            transform.position = initialPositionSlot.position;
            targetPosition = initialPositionSlot.position;
        }
        else
        {
            targetPosition = transform.position;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;

        zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();

        onDragStart?.Invoke(); // 🔔 Drag start event
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Set return target to initial slot
        if (initialPositionSlot != null)
            targetPosition = initialPositionSlot.position;

        onDragEnd?.Invoke(); // 🔔 Drag end event
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPosition() + offset;

            // Clamp only X and Y
            targetPos.x = Mathf.Clamp(targetPos.x, minXY.x, maxXY.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minXY.y, maxXY.y);

            // Keep original Z
            targetPos.z = transform.position.z;

            transform.position = targetPos;

            onDragging?.Invoke(); // 🔔 Dragging event
        }
        else
        {
            // Smooth return to initial position
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * returnSpeed
            );
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zDistance;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
