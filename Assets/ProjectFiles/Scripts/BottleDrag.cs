using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BottleDrag : MonoBehaviour
{
    private Rigidbody rb;
    private Camera cam;
    private bool isDragging;
    private Vector3 offset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        rb.useGravity = false;
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            rb.MovePosition(GetMouseWorldPosition() + offset);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}