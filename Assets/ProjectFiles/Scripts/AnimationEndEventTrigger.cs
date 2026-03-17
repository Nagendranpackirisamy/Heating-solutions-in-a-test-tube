using UnityEngine;
using UnityEngine.Events;


namespace HeatingSolutionsInaTestTube
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEndEventTrigger : MonoBehaviour
    {
        [Header("Animation Settings")]
        public string animationName; // The clip name you want to watch
        public bool useNormalizedTime = true; // true = 1 means last frame
    
        [Header("Event to Trigger")]
        public UnityEvent onAnimationEnd;
    
        private Animator animator;
        private bool hasTriggered = false;
    
        void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
        }
    
        void Update()
        {
            if (animator == null || string.IsNullOrEmpty(animationName) || hasTriggered)
                return;
    
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
    
            if (state.IsName(animationName))
            {
                // Normalized time 1 = last frame
                if (useNormalizedTime && state.normalizedTime >= 1f)
                {
                    hasTriggered = true;
                    onAnimationEnd?.Invoke();
                }
                else if (!useNormalizedTime)
                {
                    // Optionally, you can add frame counting logic here
                }
            }
        }
    
        /// <summary>
        /// Optional: reset trigger to reuse the animation
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
    
    
}