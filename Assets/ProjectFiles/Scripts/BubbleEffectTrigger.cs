using UnityEngine;

public class BubbleEffectTrigger : MonoBehaviour
{
    [Header("Effect Reference")]
    public CylindricalSpawnEffect bubbleEffect;

    // Called from animation event
    public void TriggerBubble()
    {
        if (bubbleEffect != null)
            bubbleEffect.TriggerEffect();
    }

    // Optional: stop from animation event
    public void StopBubble()
    {
        if (bubbleEffect != null)
            bubbleEffect.StopEffect();
    }
}