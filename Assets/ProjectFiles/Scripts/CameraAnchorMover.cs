using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CameraAnchorMover : MonoBehaviour
{
    [Header("Camera Anchors")]
    public Transform[] cameraAnchors;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Camera Move Event")]
    public UnityEvent OnCameraMoveStart;   // 🔊 Audio event here

    private Camera mainCamera;
    private Coroutine moveRoutine;

    void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Camera tagged as MainCamera found!");
        }
    }

    // 🔘 Call this from Button (pass anchor index)
    public void MoveCameraToIndex(int anchorIndex)
    {
        if (mainCamera == null)
            return;

        if (cameraAnchors == null || cameraAnchors.Length == 0)
            return;

        if (anchorIndex < 0 || anchorIndex >= cameraAnchors.Length)
        {
            Debug.LogWarning("Invalid camera anchor index: " + anchorIndex);
            return;
        }

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        // 🔊 FIRE EVENT WHEN CAMERA STARTS MOVING
        OnCameraMoveStart?.Invoke();

        moveRoutine = StartCoroutine(SmoothMove(cameraAnchors[anchorIndex]));
    }

    IEnumerator SmoothMove(Transform target)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            mainCamera.transform.position =
                Vector3.Lerp(startPos, target.position, t);

            mainCamera.transform.rotation =
                Quaternion.Slerp(startRot, target.rotation, t);

            yield return null;
        }
    }
}
