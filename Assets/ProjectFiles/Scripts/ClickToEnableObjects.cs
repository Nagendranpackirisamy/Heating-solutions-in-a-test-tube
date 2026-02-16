using UnityEngine;

public class ClickToEnableObjects : MonoBehaviour
{
    [Header("Slide")]
    public SlideCameraController slideController;
    public int targetPageNumber;

    [Header("Objects To Enable")]
    public GameObject[] objectsToEnable;

    [Header("Disable This Object After Click (Optional)")]
    public bool disableSelfAfterClick = false;

    private static bool slideCompleted = false;

    private void OnEnable()
    {
        RestoreState();
    }

    void OnMouseDown()
    {
        if (slideCompleted) return;

        if (slideController.CurrentPageNumber != targetPageNumber)
            return;

        EnableObjects();

        slideController.EnableNextButton();

        slideCompleted = true;

        if (disableSelfAfterClick)
            gameObject.SetActive(false);
    }

    void EnableObjects()
    {
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    void RestoreState()
    {
        if (!slideCompleted) return;

        EnableObjects();
        slideController.EnableNextButton();

        if (disableSelfAfterClick)
            gameObject.SetActive(false);
    }
}