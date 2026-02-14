using UnityEngine;

public class Slide20FlameInteraction : MonoBehaviour
{
    [Header("Slide Binding")]
    public SlideCameraController slideController;
    public int slidePageNumber; // Slide 20 page number

    [Header("Flame Object")]
    public GameObject flameObject;
    
    private static bool flameOff = false;

    void OnEnable()
    {
        if (flameOff && flameObject != null)
            flameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        Debug.Log("Child Clicked");
        if (slideController.CurrentPageNumber != slidePageNumber)
            return;

        if (flameObject != null)
        {
            flameObject.SetActive(false);
            flameOff = true;
        }
    }
}