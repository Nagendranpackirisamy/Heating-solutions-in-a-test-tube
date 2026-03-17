using UnityEngine;


namespace HeatingSolutionsInaTestTube
{
    public class StepCameraPointSwapper : MonoBehaviour
    {
        [System.Serializable]
        public class CameraSwap
        {
            [Tooltip("Page number to assign current camera position")]
            public int pageNumber;
    
            [Tooltip("Target camera point of that page")]
            public Transform cameraPoint;
        }
    
        [Header("Reference")]
        public SlideCameraController controller;
    
        [Header("Camera Swaps")]
        public CameraSwap[] swaps;
    
        // This is the function you will call from Drag3D event
        public void AssignCurrentCameraToPages()
        {
            if (controller == null || swaps == null) return;
    
            Transform cam = controller.cameraTransform;
    
            foreach (var swap in swaps)
            {
                if (swap.cameraPoint == null) continue;
    
                swap.cameraPoint.position = cam.position;
                swap.cameraPoint.rotation = cam.rotation;
            }
        }
    }
    
    
}