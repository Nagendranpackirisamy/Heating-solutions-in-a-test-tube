using UnityEngine;
using UnityEngine.Events;

public class EnableButton : MonoBehaviour
{
    [Header("Images to Track")]
    public GameObject[] imagesToCheck;

    [Header("Event When All Active")]
    public UnityEvent onAllImagesActive;

    private bool eventTriggered = false;

    private void Update()
    {
        if (!eventTriggered && AreAllImagesActive())
        {
            onAllImagesActive?.Invoke();
            eventTriggered = true;
            enabled = false; // stop checking
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
