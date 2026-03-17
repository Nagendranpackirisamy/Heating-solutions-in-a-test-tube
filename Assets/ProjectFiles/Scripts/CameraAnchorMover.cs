using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace HeatingSolutionsInaTestTube
{
    public class CameraAnchorMover : MonoBehaviour
    {
        [System.Serializable]
        public class PageCameraData
        {
            [Header("Page Info")]
            public int pageNumber;
    
            [Header("Move Points")]
            public Transform[] movePoints;
    
            [Header("Final Position")]
            public Transform finalPosition;
        }
    
        [Header("Slide Controller Reference")]
        public SlideCameraController slideController;
    
        [Header("Camera")]
        public Transform cameraTransform;
    
        [Header("Movement Settings")]
        public float moveDuration = 1f;
        public AnimationCurve moveCurve =
            AnimationCurve.EaseInOut(0, 0, 1, 1);
    
        [Header("Pages")]
        public List<PageCameraData> pages = new List<PageCameraData>();
    
        private Dictionary<int, Transform> lockedFinalPositions =
            new Dictionary<int, Transform>();
    
        private Coroutine moveRoutine;
        private int lastPage = -1;
    
        void Start()
        {
            if (cameraTransform == null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                    cameraTransform = cam.transform;
            }
    
            if (slideController != null)
                lastPage = slideController.CurrentPageNumber;
        }
    
        void Update()
        {
            if (slideController == null) return;
    
            int currentPage = slideController.CurrentPageNumber;
    
            // Detect page change automatically
            if (currentPage != lastPage)
            {
                lastPage = currentPage;
                ApplyLockedPosition(currentPage);
            }
        }
    
        PageCameraData GetPage(int pageNumber)
        {
            foreach (var p in pages)
            {
                if (p.pageNumber == pageNumber)
                    return p;
            }
            return null;
        }
    
        int GetCurrentPage()
        {
            if (slideController == null) return -1;
            return slideController.CurrentPageNumber;
        }
    
        IEnumerator SmoothMove(Transform target)
        {
            Vector3 startPos = cameraTransform.position;
            Quaternion startRot = cameraTransform.rotation;
    
            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;
    
            float time = 0f;
    
            while (time < moveDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / moveDuration);
                float curveT = moveCurve.Evaluate(t);
    
                cameraTransform.position =
                    Vector3.Lerp(startPos, endPos, curveT);
    
                cameraTransform.rotation =
                    Quaternion.Slerp(startRot, endRot, curveT);
    
                yield return null;
            }
    
            cameraTransform.position = endPos;
            cameraTransform.rotation = endRot;
        }
    
        void MoveCameraSmooth(Transform target)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
    
            moveRoutine = StartCoroutine(SmoothMove(target));
        }
    
        // -------------------------------------------------------
        // MOVE TO ELEMENT POINT
        // -------------------------------------------------------
        public void MoveToElement(int elementIndex)
        {
            int page = GetCurrentPage();
            if (page == -1) return;
    
            PageCameraData p = GetPage(page);
            if (p == null) return;
    
            if (elementIndex < 0 || elementIndex >= p.movePoints.Length)
                return;
    
            Transform target = p.movePoints[elementIndex];
            if (target == null) return;
    
            MoveCameraSmooth(target);
        }
    
        // -------------------------------------------------------
        // SINGLE FUNCTION YOU CALL FROM EVENT
        // -------------------------------------------------------
        public void LockCurrentPageFinalPosition()
        {
            int page = GetCurrentPage();
            if (page == -1) return;
    
            PageCameraData p = GetPage(page);
            if (p == null || p.finalPosition == null)
            {
                Debug.LogWarning("Final position not set for page " + page);
                return;
            }
    
            lockedFinalPositions[page] = p.finalPosition;
    
            // Smooth move to final position
            MoveCameraSmooth(p.finalPosition);
        }
    
        // -------------------------------------------------------
        // AUTO APPLY WHEN PAGE CHANGES
        // -------------------------------------------------------
        void ApplyLockedPosition(int page)
        {
            if (lockedFinalPositions.ContainsKey(page))
            {
                Transform finalPos = lockedFinalPositions[page];
                if (finalPos != null)
                {
                    cameraTransform.position = finalPos.position;
                    cameraTransform.rotation = finalPos.rotation;
                }
            }
        }
    }
    
    
}