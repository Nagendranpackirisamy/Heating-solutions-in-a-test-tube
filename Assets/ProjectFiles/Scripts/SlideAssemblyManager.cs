using UnityEngine;
using TMPro;


namespace HeatingSolutionsInaTestTube
{
    public class SlideAssemblyManager : MonoBehaviour
    {
        [Header("Slide")]
        public SlideCameraController slideController;
        public int targetPageNumber;
    
        [Header("UI Text (Inside Slide UI)")]
        public TMP_Text instructionText;   // Drag the existing text from Slide UI here
    
        [TextArea]
        public string defaultInstruction =
            "Assemble the tripod stand, wire gauze, water bath and test tube in the correct order for heating the ethanol.";
    
        [TextArea]
        public string completedInstruction =
            "You have assembled the water bath setup in the correct order.";
    
        [Header("Wrong Order Panel")]
        public GameObject wrongPanel;
        public TMP_Text wrongText;
    
        [Header("Correct Order")]
        public DraggableItem[] correctOrder;
    
        private static int completedCount = 0;
        private static bool slideCompleted = false;
    
        private void OnEnable()
        {
            RestoreState();
        }
    
        public void TryPlaceItem(DraggableItem item)
        {
            if (slideController.CurrentPageNumber != targetPageNumber)
                return;
    
            if (slideCompleted)
                return;
    
            if (correctOrder[completedCount] == item)
            {
                item.SnapToTarget();
                completedCount++;
    
                wrongPanel.SetActive(false);
    
                if (completedCount >= correctOrder.Length)
                {
                    slideCompleted = true;
    
                    // 🔥 Override existing slide UI text
                    instructionText.text = completedInstruction;
    
                    slideController.EnableNextButton();
                }
            }
            else
            {
                wrongPanel.SetActive(true);
                wrongText.text =
                    "The wire gauze cannot be placed over the burner directly, place the tripod stand first.";
    
                item.ResetPosition();
                
            }
        }
    
        void RestoreState()
        {
            if (slideCompleted)
            {
                foreach (var item in correctOrder)
                {
                    item.ForceSnap();
                }
    
                instructionText.text = completedInstruction;
                slideController.EnableNextButton();
            }
            else
            {
                instructionText.text = defaultInstruction;
            }
        }
    }
    
    
}