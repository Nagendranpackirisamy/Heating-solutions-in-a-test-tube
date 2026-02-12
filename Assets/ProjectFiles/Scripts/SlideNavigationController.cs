using UnityEngine;
using UnityEngine.UI;

public class SlideNavigationController : MonoBehaviour
{
    public SlideStateController stateController;
    public Button nextButton;
    public Button previousButton;

    void Start()
    {
        nextButton.onClick.AddListener(OnNext);
        previousButton.onClick.AddListener(OnPrevious);

        stateController.OnSlideChanged.AddListener(UpdateButtons);

        UpdateButtons(stateController.CurrentSlideIndex);
    }

    void OnNext()
    {
        stateController.GoNext();
    }

    void OnPrevious()
    {
        stateController.GoPrevious();
    }

    void UpdateButtons(int index)
    {
        nextButton.interactable = stateController.CanGoNext();
        previousButton.interactable = stateController.CanGoPrevious();
    }
}