using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SlideEventController : MonoBehaviour
{
    [System.Serializable]
    public class SlideEvent
    {
        public int slideIndex;
        public UnityEvent onEnter;
    }

    public SlideStateController stateController;
    public List<SlideEvent> slideEvents;

    void OnEnable()
    {
        stateController.OnSlideChanged.AddListener(HandleSlideChange);
    }

    void OnDisable()
    {
        stateController.OnSlideChanged.RemoveListener(HandleSlideChange);
    }

    void HandleSlideChange(int index)
    {
        foreach (var s in slideEvents)
        {
            if (s.slideIndex == index)
            {
                s.onEnter?.Invoke();
            }
        }
    }
}