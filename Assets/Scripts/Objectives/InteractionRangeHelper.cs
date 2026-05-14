using UnityEngine;

/// <summary>
/// Cached player lookup + distance checks so interactables still work when trigger messages
/// are unreliable (e.g. CharacterController setups without a Rigidbody on the player).
/// </summary>
public static class InteractionRangeHelper
{
    static Transform s_player;

    public static void InvalidatePlayerCache()
    {
        s_player = null;
    }

    public static bool TryGetPlayerTransform(out Transform player)
    {
        if (s_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            s_player = go != null ? go.transform : null;
        }

        player = s_player;
        return player != null;
    }

    public static bool IsPlayerWithinRange(Vector3 worldPoint, float range)
    {
        if (!TryGetPlayerTransform(out Transform p))
            return false;

        return (p.position - worldPoint).sqrMagnitude <= range * range;
    }
}
