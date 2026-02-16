using UnityEngine;
using System.Collections;

public class Slide19Interaction : MonoBehaviour
{
    [Header("Slide Controller")]
    public SlideCameraController slideController;

    [Header("Slide 19 Settings")]
    public int slide19PageNumber;
    public Animator testTubeAnimator;
    public float animationDuration = 2f;

    [Header("Camera Movement During Animation")]
    public Transform cameraTransform;
    public Transform overrideCameraPoint;
    public float cameraMoveDuration = 1.5f;
    public AnimationCurve cameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Slide 20 Settings")]
    public int slide20PageNumber;
    public GameObject slide20Flame;
    public GameObject slide20ObjectToDisable;

    private static bool slide19Completed = false;
    private bool isRunning = false;

    void OnMouseDown()
    {
        int currentPage = slideController.CurrentPageNumber;

        if (currentPage == slide19PageNumber)
        {
            HandleSlide19();
        }
        else if (currentPage == slide20PageNumber)
        {
            HandleSlide20();
        }
    }

    // =====================================================
    // SLIDE 19 LOGIC
    // =====================================================
    void HandleSlide19()
    {
        if (isRunning) return;
        if (slide19Completed) return;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isRunning = true;

        slideController.nextButton.interactable = false;
        slideController.previousButton.interactable = false;

        yield return null;

        // Play animation
        testTubeAnimator.ResetTrigger("Play");
        testTubeAnimator.SetTrigger("Play");

        // Move camera while animation plays
        yield return StartCoroutine(MoveCamera());

        // Wait remaining animation time if needed
        if (animationDuration > cameraMoveDuration)
        {
            yield return new WaitForSeconds(animationDuration - cameraMoveDuration);
        }

        slide19Completed = true;

        // 🔥 CRITICAL PART:
        // Change anchor reference permanently after completion
        foreach (var step in slideController.steps)
        {
            if (step.pageNumber == slide19PageNumber)
            {
                step.cameraPoint = overrideCameraPoint;
                break;
            }
        }

        slideController.EnableNextButton();
        slideController.previousButton.interactable = true;

        isRunning = false;
    }

    IEnumerator MoveCamera()
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 endPos = overrideCameraPoint.position;
        Quaternion endRot = overrideCameraPoint.rotation;

        float elapsed = 0f;

        while (elapsed < cameraMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraMoveDuration);
            float curveT = cameraCurve.Evaluate(t);

            cameraTransform.position =
                Vector3.Lerp(startPos, endPos, curveT);

            cameraTransform.rotation =
                Quaternion.Slerp(startRot, endRot, curveT);

            yield return null;
        }

        cameraTransform.position = endPos;
        cameraTransform.rotation = endRot;
    }

    // =====================================================
    // SLIDE 20 LOGIC
    // =====================================================
    private bool slide20Completed = false;

    void HandleSlide20()
    {
        if (slide20Completed) return;

        if (slide20Flame != null)
            slide20Flame.SetActive(false);

        if (slide20ObjectToDisable != null)
            slide20ObjectToDisable.SetActive(false);

        slideController.EnableNextButton();

        slide20Completed = true;
    }


}
