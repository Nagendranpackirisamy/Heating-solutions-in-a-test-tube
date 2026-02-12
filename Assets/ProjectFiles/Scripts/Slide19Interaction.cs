using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slide19Interaction : MonoBehaviour
{
    [Header("Slide Controller")]
    public SlideCameraController slideController;

    [Header("Slide 19 Settings")]
    public int slide19PageNumber;
    public Animator testTubeAnimator;
    public float animationDuration = 2f;

    [Header("Slide 19 Camera Override")]
    public Transform cameraTransform;
    public Transform overrideCameraPoint;
    public float cameraMoveDuration = 1.5f;
    public AnimationCurve cameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Slide 20 Settings")]
    public int slide20PageNumber;
    public GameObject slide20Flame;

    private static bool slide19Completed = false;
    private bool isRunning = false;

    // =================================================
    // CLICK HANDLER (Single Object, Multi Slide Logic)
    // =================================================
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

    // =================================================
    // SLIDE 19 BEHAVIOUR
    // =================================================
    void HandleSlide19()
    {
        if (isRunning) return;
        if (slide19Completed) return;

        StartCoroutine(PlaySlide19Sequence());
    }

    IEnumerator PlaySlide19Sequence()
    {
        isRunning = true;

        // Lock navigation
        slideController.nextButton.interactable = false;
        slideController.previousButton.interactable = false;

        yield return null;

        // Play animation
        testTubeAnimator.ResetTrigger("Play");
        testTubeAnimator.SetTrigger("Play");

        // Move camera
        yield return StartCoroutine(MoveCamera());

        // Wait remaining animation time
        if (animationDuration > cameraMoveDuration)
        {
            yield return new WaitForSeconds(animationDuration - cameraMoveDuration);
        }

        slide19Completed = true;

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

            cameraTransform.position = Vector3.Lerp(startPos, endPos, curveT);
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, curveT);

            yield return null;
        }

        cameraTransform.position = endPos;
        cameraTransform.rotation = endRot;
    }

    // =================================================
    // SLIDE 20 BEHAVIOUR
    // =================================================
    void HandleSlide20()
    {
        if (slide20Flame != null)
            slide20Flame.SetActive(false);
    }
}
