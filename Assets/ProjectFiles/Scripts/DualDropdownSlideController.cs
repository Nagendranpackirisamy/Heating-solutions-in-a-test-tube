using UnityEngine;

public class DualDropdownSlideController : MonoBehaviour
{
    public CustomDropDown dropdown1;
    public CustomDropDown dropdown2;

    public SlideCameraController slideController;

    private bool nextEnabled = false;

    void Start()
    {
        dropdown1.onCorrectSelected.AddListener(CheckCompletion);
        dropdown1.onWrongSelected.AddListener(CheckCompletion);

        dropdown2.onCorrectSelected.AddListener(CheckCompletion);
        dropdown2.onWrongSelected.AddListener(CheckCompletion);
    }

    void CheckCompletion()
    {
        if (dropdown1.IsCorrect && dropdown2.IsCorrect)
        {
            if (!nextEnabled)
            {
                nextEnabled = true;
                slideController.EnableNextButton();
            }
        }
    }
}