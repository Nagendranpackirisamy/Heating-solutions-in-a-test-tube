using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SlideStateController : MonoBehaviour
{
    [System.Serializable]
    public class SlideData
    {
        public int pageNumber;
        public bool enableNextOnStart;
    }

    [Header("Slides")]
    public List<SlideData> slides;

    public UnityEvent<int> OnSlideChanged;

    public int CurrentSlideIndex { get; private set; }

    private bool[] slideCompleted;
    private bool[] slideLocked;

    void Awake()
    {
        slideCompleted = new bool[slides.Count];
        slideLocked = new bool[slides.Count];

        CurrentSlideIndex = 0;

        if (slides[0].enableNextOnStart)
            slideCompleted[0] = true;

        OnSlideChanged?.Invoke(CurrentSlideIndex);
    }

    public void CompleteCurrentSlide()
    {
        slideCompleted[CurrentSlideIndex] = true;
        slideLocked[CurrentSlideIndex] = true;
    }

    public bool CanGoNext()
    {
        return slideCompleted[CurrentSlideIndex]
               && CurrentSlideIndex < slides.Count - 1;
    }

    public bool CanGoPrevious()
    {
        return CurrentSlideIndex > 0;
    }

    public void GoNext()
    {
        if (!CanGoNext()) return;

        CurrentSlideIndex++;
        OnSlideChanged?.Invoke(CurrentSlideIndex);
    }

    public void GoPrevious()
    {
        if (!CanGoPrevious()) return;

        CurrentSlideIndex--;
        OnSlideChanged?.Invoke(CurrentSlideIndex);
    }
}
