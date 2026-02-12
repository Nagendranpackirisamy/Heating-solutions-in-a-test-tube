using UnityEngine;
using UnityEngine.Events;

public class EnableButton : MonoBehaviour
{
    [Header("Slide Binding")]
    public SlideStateController stateController;
    public int targetSlideIndex;

    [Header("Images To Check")]
    public GameObject[] imagesToCheck;

    [Header("Event When All Active")]
    public UnityEvent onAllImagesActive;

    private bool eventTriggered = false;
    private bool isActiveSlide = false;

    void OnEnable()
    {
        stateController.OnSlideChanged.AddListener(OnSlideChanged);
    }

    void OnDisable()
    {
        stateController.OnSlideChanged.RemoveListener(OnSlideChanged);
    }

    void OnSlideChanged(int index)
    {
        isActiveSlide = (index == targetSlideIndex);
        eventTriggered = false;

        if (isActiveSlide)
        {
            // Lock slide by default
            // Do NOT complete it yet
        }
    }

    void Update()
    {
        if (!isActiveSlide || eventTriggered)
            return;

        if (AreAllImagesActive())
        {
            eventTriggered = true;

            stateController.CompleteCurrentSlide();
            onAllImagesActive?.Invoke();
        }
    }

    bool AreAllImagesActive()
    {
        foreach (GameObject img in imagesToCheck)
        {
            if (!img.activeInHierarchy)
                return false;
        }
        return true;
    }
}