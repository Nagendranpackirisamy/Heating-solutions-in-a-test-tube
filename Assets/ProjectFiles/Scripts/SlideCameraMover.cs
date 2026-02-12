using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideCameraMover : MonoBehaviour
{
    [Header("References")]
    public SlideStateController stateController;
    public Transform cameraTransform;
    public List<Transform> cameraPoints;

    [Header("Motion")]
    public float moveDuration = 1.5f;
    public AnimationCurve positionCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve rotationCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isMoving;

    void OnEnable()
    {
        stateController.OnSlideChanged.AddListener(MoveToSlide);
    }

    void OnDisable()
    {
        stateController.OnSlideChanged.RemoveListener(MoveToSlide);
    }

    void MoveToSlide(int index)
    {
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines();
        StartCoroutine(MoveRoutine(index));
    }

    IEnumerator MoveRoutine(int index)
    {
        isMoving = true;

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 endPos = cameraPoints[index].position;
        Quaternion endRot = cameraPoints[index].rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            cameraTransform.position =
                Vector3.Lerp(startPos, endPos, positionCurve.Evaluate(t));

            cameraTransform.rotation =
                Quaternion.Slerp(startRot, endRot, rotationCurve.Evaluate(t));

            yield return null;
        }

        isMoving = false;
    }
}