using UnityEngine;

/// <summary>
/// Loops a walk/footstep clip while the player moves on the ground. Does <b>not</b> use
/// <see cref="CharacterController.velocity"/> — controllers that call <c>Move</c> twice per frame
/// (horizontal then gravity) only report the last move in <c>velocity</c>, so horizontal speed reads ~0.
/// Speed comes from planar position change; grounding uses a downward ray on <see cref="groundLayers"/>.
/// Runs in <see cref="LateUpdate"/> so movement from <c>FirstPersonController</c> has already applied.
/// </summary>
[DefaultExecutionOrder(100)]
public class PlayerFootstepAudio : MonoBehaviour
{
    [SerializeField] AudioSource footstepSource;

    [Tooltip("Short looping walk/footstep bed.")]
    [SerializeField] AudioClip footstepWalkLoop;

    [Tooltip("If Walk Loop is empty, uses the first entry here (for older setups).")]
    [SerializeField] AudioClip[] footstepClipsLegacy;

    [SerializeField] float minHorizontalSpeed = 0.08f;

    [SerializeField] float groundRayLength = 0.45f;

    [SerializeField] LayerMask groundLayers = ~0;

    CharacterController _controller;
    Vector3 _lastPlanarPos;
    bool _hasLastPlanarPos;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
            _controller = GetComponentInChildren<CharacterController>();

        if (footstepSource == null)
            footstepSource = GetComponent<AudioSource>();
        if (footstepSource == null)
            footstepSource = gameObject.AddComponent<AudioSource>();

        AudioClip clip = footstepWalkLoop;
        if (clip == null && footstepClipsLegacy != null && footstepClipsLegacy.Length > 0)
            clip = footstepClipsLegacy[0];

        footstepSource.clip = clip;
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = 0f;
    }

    void LateUpdate()
    {
        if (footstepSource == null || footstepSource.clip == null)
            return;

        if (Time.timeScale <= 0f)
        {
            StopFootsteps();
            return;
        }

        float speed = GetPlanarSpeedFromMotion();
        bool grounded = IsOnGroundLayer();
        bool walking = grounded && speed >= minHorizontalSpeed;

        if (walking)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();

            footstepSource.pitch = Mathf.Clamp(0.9f + speed * 0.06f, 0.9f, 1.25f);
        }
        else
            StopFootsteps();
    }

    void OnDisable()
    {
        StopFootsteps();
    }

    void StopFootsteps()
    {
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();
    }

    /// <summary>Horizontal speed from world motion — reliable with split <c>Move</c> calls.</summary>
    float GetPlanarSpeedFromMotion()
    {
        Vector3 p = transform.position;
        Vector3 planar = new Vector3(p.x, 0f, p.z);

        if (!_hasLastPlanarPos)
        {
            _lastPlanarPos = planar;
            _hasLastPlanarPos = true;
            return 0f;
        }

        float dist = Vector3.Distance(planar, _lastPlanarPos);
        _lastPlanarPos = planar;
        return dist / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    bool IsOnGroundLayer()
    {
        Vector3 origin = GetFootRayOrigin();
        return Physics.Raycast(origin, Vector3.down, groundRayLength, groundLayers, QueryTriggerInteraction.Ignore);
    }

    Vector3 GetFootRayOrigin()
    {
        if (_controller != null)
        {
            Bounds b = _controller.bounds;
            return new Vector3(b.center.x, b.min.y + 0.02f, b.center.z);
        }

        return transform.position + Vector3.up * 0.08f;
    }
}
