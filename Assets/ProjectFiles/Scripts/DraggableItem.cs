using UnityEngine;


namespace HeatingSolutionsInaTestTube
{
    [RequireComponent(typeof(Collider))]
    public class DraggableItem : MonoBehaviour
    {
        public SlideAssemblyManager manager;
        public Transform snapPoint;
    
        [Header("Label (Name Tag)")]
        public GameObject label;
    
        [Header("Snap Settings")]
        public float snapDistance = 1.0f;  // Adjust in inspector
    
        private Vector3 startPos;
        private bool isSnapped = false;
        private bool hasDragged = false;
    
        void Start()
        {
            startPos = transform.position;
    
            if (!isSnapped && label != null)
                label.SetActive(true);
        }
    
        void OnMouseDown()
        {
            if (isSnapped) return;
    
            hasDragged = false;
    
            if (label != null)
                label.SetActive(false);
        }
    
        void OnMouseDrag()
        {
            if (isSnapped) return;
    
            hasDragged = true;
    
            Vector3 mouse = Input.mousePosition;
            mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(mouse);
        }
    
        void OnMouseUp()
        {
            if (isSnapped) return;
    
            // If user only clicked without dragging
            if (!hasDragged)
            {
                ResetPosition();
                return;
            }
    
            // Check if near snap point
            float distance = Vector3.Distance(transform.position, snapPoint.position);
    
            if (distance <= snapDistance)
            {
                manager.TryPlaceItem(this);
            }
            else
            {
                ResetPosition();
            }
        }
    
        public void SnapToTarget()
        {
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
    
            isSnapped = true;
    
            if (label != null)
                label.SetActive(false);
        }
    
        public void ForceSnap()
        {
            SnapToTarget();
            isSnapped = true;
        }
    
        public void ResetPosition()
        {
            transform.position = startPos;
    
            if (label != null)
                label.SetActive(true);
        }
    }
    
}