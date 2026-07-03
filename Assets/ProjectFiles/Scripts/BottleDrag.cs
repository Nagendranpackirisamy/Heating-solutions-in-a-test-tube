using UnityEngine;
using System.Collections;

namespace HeatingSolutionsInaTestTube
{
    [RequireComponent(typeof(Rigidbody))]
    public class BottleDrag : MonoBehaviour
    {
        private Rigidbody rb;
        private Camera cam;
        private bool isDragging;
        private Vector3 offset;

        private Vector3 initialPosition;
        private Coroutine returnRoutine;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;

            rb.useGravity = false;

            initialPosition = transform.position;
        }

        void OnMouseDown()
        {
            if (returnRoutine != null)
                StopCoroutine(returnRoutine);

            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
        }

        void OnMouseUp()
        {
            isDragging = false;

            // Return if no other script has snapped it to the target.
            returnRoutine = StartCoroutine(ReturnToInitialPosition());
        }

        void FixedUpdate()
        {
            if (isDragging)
            {
                rb.MovePosition(GetMouseWorldPosition() + offset);
            }
        }

        Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam.WorldToScreenPoint(transform.position).z;
            return cam.ScreenToWorldPoint(mousePos);
        }

        IEnumerator ReturnToInitialPosition()
        {
            // Wait one frame so other scripts can snap the bottle to the target first.
            yield return null;

            // If another script moved the bottle away from where it was released,
            // assume it snapped successfully and don't return it.
            Vector3 releasedPosition = transform.position;

            yield return new WaitForSeconds(0.05f);

            if (Vector3.Distance(transform.position, releasedPosition) > 0.01f)
                yield break;

            while (Vector3.Distance(transform.position, initialPosition) > 0.01f)
            {
                rb.MovePosition(Vector3.Lerp(
                    transform.position,
                    initialPosition,
                    Time.fixedDeltaTime * 5f));

                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(initialPosition);
        }
    }
}