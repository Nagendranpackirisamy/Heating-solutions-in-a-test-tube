using UnityEngine;
using UnityEngine.Events;


namespace HeatingSolutionsInaTestTube
{
    public class ClickOrTouchEvent : MonoBehaviour
    {
        [Header("Event to Trigger on Click/Touch")]
        public UnityEvent onClicked; // Assign function from Inspector
    
        private void Update()
        {
            // For mouse click or touch
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        onClicked.Invoke();
                    }
                }
            }
        }
    }
    
}