using UnityEngine;
using UnityEngine.Events;

public class DirectCollisionSwap : MonoBehaviour
{
    [Header("Other Collider To Detect")]
    public Collider targetCollider;

    [Header("Objects To Hide")]
    public GameObject[] objectsToHide;

    [Header("Objects To Show")]
    public GameObject[] objectsToShow;

    [Header("SFX Event")]
    public UnityEvent onCollisionSFX;

    [Header("Collision Effect Object")]
    public GameObject collisionEffectObject; // object to activate
    public float effectDuration = 2f;

    private bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (triggered) return;

        if (collision.collider == targetCollider)
        {
            TriggerSwap();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other == targetCollider)
        {
            TriggerSwap();
        }
    }

    void TriggerSwap()
    {
        triggered = true;

        // Hide objects
        foreach (GameObject obj in objectsToHide)
        {
            if (obj) obj.SetActive(false);
        }

        // Show objects
        foreach (GameObject obj in objectsToShow)
        {
            if (obj) obj.SetActive(true);
        }

        // Play SFX
        onCollisionSFX?.Invoke();

        // Activate and destroy effect object
        if (collisionEffectObject != null)
        {
            collisionEffectObject.SetActive(true);
            Destroy(collisionEffectObject, effectDuration);
        }
    }
}
