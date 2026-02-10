using UnityEngine;
using UnityEngine.UI;

public class EnableButton : MonoBehaviour
{
    [Header("Images to Track")]
    public GameObject[] imagesToCheck;

    [Header("Target Button")]
    public Button targetButton;

    private void Start()
    {
        // Button starts disabled
        targetButton.interactable = false;
    }

    private void Update()
    {
        if (AreAllImagesActive())
        {
            targetButton.interactable = true;
            enabled = false; // Stop checking once done (important for performance)
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