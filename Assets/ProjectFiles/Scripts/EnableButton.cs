using UnityEngine;
using UnityEngine.Events;
using System.Reflection;


namespace HeatingSolutionsInaTestTube
{
    public class EnableButton : MonoBehaviour
    {
        [Header("Slide Controller")]
        public SlideCameraController slideController;
    
        [Header("Target Slide Number (pageNumber)")]
        public int targetPageNumber;
    
        [Header("Images to Track")]
        public GameObject[] imagesToCheck;
    
        [Header("Event When All Active")]
        public UnityEvent onAllImagesActive;
    
        private bool eventTriggered = false;
        private bool hasLockedNavigation = false;
    
        private FieldInfo currentIndexField;
    
        private void Start()
        {
            // Get private field "currentIndex" using reflection
            currentIndexField = typeof(SlideCameraController)
                .GetField("currentIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    
        private void Update()
        {
            if (slideController == null || currentIndexField == null)
                return;
    
            int currentIndex = (int)currentIndexField.GetValue(slideController);
    
            if (currentIndex < 0 || currentIndex >= slideController.steps.Count)
                return;
    
            int currentPage = slideController.steps[currentIndex].pageNumber;
    
            // Lock navigation when this slide becomes active
            if (!hasLockedNavigation && currentPage == targetPageNumber)
            {
                hasLockedNavigation = true;
                slideController.nextButton.interactable = false;
                slideController.previousButton.interactable = false;
            }
    
            // Run logic only on target slide
            if (hasLockedNavigation && !eventTriggered && currentPage == targetPageNumber)
            {
                if (AreAllImagesActive())
                {
                    eventTriggered = true;
    
                    slideController.EnableNextButton();
                    slideController.previousButton.interactable = true;
    
                    onAllImagesActive?.Invoke();
                }
            }
        }
    
        private bool AreAllImagesActive()
        {
            foreach (GameObject img in imagesToCheck)
            {
                if (!img.activeInHierarchy)
                    return false;
            }
            return true;
        }
    }
    
}