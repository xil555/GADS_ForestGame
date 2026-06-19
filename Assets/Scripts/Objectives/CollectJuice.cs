using System.Collections;
using UnityEngine;

/// <summary>
/// Lightweight pickup feedback: shrink and rise before hide/destroy.
/// </summary>
public static class CollectJuice
{
    const float DefaultDuration = 0.1f;
    const float DefaultRise = 0.35f;

    public static IEnumerator ShrinkAndRise(Transform target, float duration = DefaultDuration, float rise = DefaultRise)
    {
        if (target == null)
            yield break;

        Vector3 startScale = target.localScale;
        Vector3 startPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.LerpUnclamped(startScale, Vector3.zero, t);
            target.localPosition = startPos + Vector3.up * (rise * t);
            yield return null;
        }
    }

    public static void DisableColliders(Transform target)
    {
        if (target == null)
            return;

        foreach (var c in target.GetComponentsInChildren<Collider>())
            c.enabled = false;
    }
}
