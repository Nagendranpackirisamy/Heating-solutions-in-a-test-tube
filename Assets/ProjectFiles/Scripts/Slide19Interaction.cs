using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slide19Interaction : MonoBehaviour
{
    [Header("Slide Binding")]
    public SlideCameraController slideController;
    public int targetPageNumber;

    [Header("Animation")]
    public Animator testTubeAnimator;
    public float animationDuration = 2f;

    [Header("Camera Override")]
    public Transform cameraTransform;
    public Transform overrideCameraPoint;
    public float cameraMoveDuration = 1.5f;
    public AnimationCurve cameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static Dictionary<int, bool> slideCompletedState
        = new Dictionary<int, bool>();

    private bool isRunning = false;

    private void OnEnable()
    {
        StartCoroutine(DelayedRestore());
    }

    IEnumerator DelayedRestore()
    {
        yield return null; // wait 1 frame so SlideController finishes

        RestoreIfCompleted();
    }

    
    void OnMouseDown()
    {
        Debug.Log("Object Clicked");

        OnTestTubeClicked();
    }

    public void OnTestTubeClicked()
    {
        if (!IsOnCorrectSlide()) return;
        if (isRunning) return;

        if (IsSlideCompleted())
            return;

        StartCoroutine(PlaySequence());
    }

    bool IsOnCorrectSlide()
    {
        return slideController.CurrentPageNumber == targetPageNumber;
    }

    bool IsSlideCompleted()
    {
        return slideCompletedState.ContainsKey(targetPageNumber)
               && slideCompletedState[targetPageNumber];
    }

    IEnumerator PlaySequence()
    {
        isRunning = true;

        // Lock navigation immediately
        slideController.nextButton.interactable = false;
        slideController.previousButton.interactable = false;

        // Small delay to avoid fighting SlideController movement
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

        slideCompletedState[targetPageNumber] = true;

        // Enable navigation AFTER animation
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

        // Force exact final position (important)
        cameraTransform.position = endPos;
        cameraTransform.rotation = endRot;
    }


    void RestoreIfCompleted()
    {
        if (!IsOnCorrectSlide()) return;

        if (IsSlideCompleted())
        {
            // Force camera to override point instantly
            cameraTransform.position = overrideCameraPoint.position;
            cameraTransform.rotation = overrideCameraPoint.rotation;

            slideController.EnableNextButton();
        }
    }
}
