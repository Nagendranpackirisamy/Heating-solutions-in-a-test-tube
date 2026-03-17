using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace HeatingSolutionsInaTestTube
{
    public class Drag3D : MonoBehaviour
    {
        private Camera mainCamera;
        private Vector3 offset;
        private float zDistance;
        private bool isDragging = false;
        private bool isLocked = false;
    
        private Vector3 targetPosition;
    
        [Header("Initial Position Slot")]
        public Transform initialPositionSlot;
    
        [Header("Target Position Slot")]
        public Transform targetSlot;
        public float snapDistance = 1f;
    
        [Header("Y Axis Restriction")]
        public float minY = 0f;
    
        [Header("Movement Smoothness")]
        public float dragSmoothSpeed = 10f;
    
        [Header("Return Settings")]
        public float returnSpeed = 5f;
    
        [Header("Drag Events")]
        public UnityEvent onDragStart;
        public UnityEvent onDragging;
        public UnityEvent onDragEnd;
        public UnityEvent onSnappedToTarget;
    
        [Header("Camera Swap After Snap")]
        public Transform dragCameraPoint;
        public SlideCameraController slideController;
        public int targetPageNumber;
    
        [Header("Camera Move Settings")]
        public float cameraMoveDuration = 1.2f;
    
        bool hasCompleted = false;
    
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
            if (isLocked) return;
    
            if (slideController != null)
            {
                int currentPage = GetCurrentPageNumber();
                if (currentPage != targetPageNumber)
                    return;
            }
    
            isDragging = true;
    
            zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
            offset = transform.position - GetMouseWorldPosition();
    
            onDragStart?.Invoke();
        }
    
        void OnMouseUp()
        {
            if (isLocked) return;
    
            isDragging = false;
    
            if (targetSlot != null)
            {
                float dist = Vector3.Distance(transform.position, targetSlot.position);
    
                if (dist <= snapDistance)
                {
                    targetPosition = targetSlot.position;
                    isLocked = true;
                    onSnappedToTarget?.Invoke();
                    hasCompleted = true;
    
                    // Smooth camera move
                    Debug.Log("Snapped! Starting camera move.");
                    StartCoroutine(MoveCameraToDragPoint());
    
    
                    if (slideController != null)
                        slideController.EnableNextButton();
    
                    onSnappedToTarget?.Invoke();
                }
                else if (initialPositionSlot != null)
                {
                    targetPosition = initialPositionSlot.position;
                }
            }
            else if (initialPositionSlot != null)
            {
                targetPosition = initialPositionSlot.position;
            }
    
            onDragEnd?.Invoke();
        }
    
        void Update()
        {
            if (hasCompleted)
            {
                transform.position = targetPosition;
                return;
            }
    
            if (isDragging && !isLocked)
            {
                targetPosition = GetMouseWorldPosition() + offset;
    
                if (targetPosition.y < minY)
                    targetPosition.y = minY;
    
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.deltaTime * dragSmoothSpeed
                );
    
                onDragging?.Invoke();
            }
            else
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.deltaTime * returnSpeed
                );
            }
        }
    
        IEnumerator MoveCameraToDragPoint()
        {
            Debug.Log("Camera coroutine started");
    
            if (dragCameraPoint == null)
            {
                Debug.LogError("dragCameraPoint is NULL");
                yield break;
            }
    
            if (slideController == null)
            {
                Debug.LogError("slideController is NULL");
                yield break;
            }
    
            Transform cam = slideController.cameraTransform;
    
            if (cam == null)
            {
                Debug.LogError("cameraTransform is NULL");
                yield break;
            }
    
            Debug.Log("Camera moving now...");
    
            Vector3 startPos = cam.position;
            Quaternion startRot = cam.rotation;
    
            float duration = slideController.moveDuration;
            float time = 0f;
    
            while (time < duration)
            {
                float t = time / duration;
    
                cam.position = Vector3.Lerp(startPos, dragCameraPoint.position, t);
                cam.rotation = Quaternion.Slerp(startRot, dragCameraPoint.rotation, t);
    
                time += Time.deltaTime;
                yield return null;
            }
    
            cam.position = dragCameraPoint.position;
            cam.rotation = dragCameraPoint.rotation;
    
            Debug.Log("Camera move finished");
    
            // Save into slide
            foreach (var step in slideController.steps)
            {
                if (step.pageNumber == targetPageNumber)
                {
                    step.cameraPoint.position = dragCameraPoint.position;
                    step.cameraPoint.rotation = dragCameraPoint.rotation;
                    break;
                }
            }
    
            slideController.EnableNextButton();
        }
    
    
    
        int GetCurrentStepIndex()
        {
            for (int i = 0; i < slideController.steps.Count; i++)
            {
                if (slideController.steps[i].canvasGroup != null &&
                    slideController.steps[i].canvasGroup.alpha > 0.5f)
                {
                    return i;
                }
            }
            return 0;
        }
    
        int GetCurrentPageNumber()
        {
            int index = GetCurrentStepIndex();
            return slideController.steps[index].pageNumber;
        }
    
        Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = zDistance;
            return mainCamera.ScreenToWorldPoint(mousePoint);
        }
    }
    
    
}