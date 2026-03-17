using UnityEngine;
using System.Collections;


namespace HeatingSolutionsInaTestTube
{
    public class SlideBottlePourInteraction : MonoBehaviour
    {
        [Header("Slide Controller")]
        public SlideCameraController slideController;
        public int targetPageNumber;
    
        [Header("Objects")]
        public GameObject bottleVariant;
        public GameObject animatedBottle;
    
        [Header("Animation")]
        public Animator bottleAnimator;
        public float animationDuration = 3f;
    
        private static bool slideCompleted = false;
        private bool isRunning = false;
    
        void OnEnable()
        {
            RestoreStateIfCompleted();
        }
    
        void RestoreStateIfCompleted()
        {
            if (!slideCompleted) return;
    
            // Show final state
            bottleVariant.SetActive(false);
            animatedBottle.SetActive(false);
    
            slideController.EnableNextButton();
        }
    
        void OnTriggerEnter(Collider other)
        {
            if (isRunning) return;
            if (slideCompleted) return;
    
            if (slideController.CurrentPageNumber != targetPageNumber)
                return;
    
            if (other.gameObject == bottleVariant)
            {
                StartCoroutine(PlayPourSequence());
            }
        }
    
        IEnumerator PlayPourSequence()
        {
            isRunning = true;
    
            slideController.nextButton.interactable = false;
            slideController.previousButton.interactable = false;
    
            // Swap models
            bottleVariant.SetActive(false);
            animatedBottle.SetActive(true);
    
            yield return null;
    
            bottleAnimator.SetTrigger("Play");
    
            yield return new WaitForSeconds(animationDuration);
    
            // Final state
            animatedBottle.SetActive(false);
    
            slideCompleted = true;
    
            slideController.EnableNextButton();
            slideController.previousButton.interactable = true;
    
            isRunning = false;
        }
    }
    
}