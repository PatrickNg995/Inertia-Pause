using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    [Header("Animator")]
    [SerializeField] private Animator _animator;

    [Header("Start Pose")]
    [Tooltip("Layer whose pose you want to start at a specific frame.")]
    [SerializeField] private int _startLayerIndex = 0;

    [Tooltip("If < 0, start at a random time. If >= 0, start at this frame index of the clip on the start layer.")]
    [SerializeField] private int _frameToLoad = -1;

    // Saved pause state per layer
    private int[] _pausedStateHashes;
    private float[] _pausedNormalizedTimes;

    private bool _isPaused;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        int layerCount = _animator.layerCount;
        _pausedStateHashes = new int[layerCount];
        _pausedNormalizedTimes = new float[layerCount];

        // Ensure layer index is valid
        _startLayerIndex = Mathf.Clamp(_startLayerIndex, 0, layerCount - 1);

        // First, let the animator enter its default state so the right clip is active
        _animator.Play(0, _startLayerIndex, 0f);
        _animator.Update(0f);

        // Decide start time for that layer
        float normalizedStartTime;

        if (_frameToLoad < 0)
        {
            // Random point in the current state's timeline
            normalizedStartTime = Random.value;
        }
        else
        {
            // Get the clip currently playing on that layer
            AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(_startLayerIndex);
            if (clips.Length > 0)
            {
                AnimationClip clip = clips[0].clip;

                // Compute total frames from the *actual* clip
                float totalFrames = clip.length * clip.frameRate;

                // Map frame index -> [0,1] normalized time (0..N-1)
                normalizedStartTime = Mathf.Clamp01(_frameToLoad / Mathf.Max(totalFrames - 1f, 1f));
            }
            else
            {
                // Fallback if no clip info – just use random
                normalizedStartTime = Random.value;
            }
        }

        // Get current state on that layer so we can reuse its hash
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(_startLayerIndex);

        // Play that state at the computed normalized time
        _animator.Play(stateInfo.fullPathHash, _startLayerIndex, normalizedStartTime);
        _animator.Update(0f);   // force it to this pose immediately
    }

    public void Pause()
    {
        if (_isPaused) return;

        int layerCount = _animator.layerCount;

        if (_pausedStateHashes == null || _pausedStateHashes.Length != layerCount)
        {
            _pausedStateHashes = new int[layerCount];
            _pausedNormalizedTimes = new float[layerCount];
        }

        // Capture current state + time for every layer
        for (int layer = 0; layer < layerCount; layer++)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
            _pausedStateHashes[layer] = stateInfo.fullPathHash;
            _pausedNormalizedTimes[layer] = stateInfo.normalizedTime; // keep exact value
        }

        _animator.speed = 0f;
        _isPaused = true;
    }

    public void Unpause()
    {
        if (!_isPaused) return;

        _animator.speed = 1f;
        _isPaused = false;
    }

    public void ResetStateBeforeUnpause()
    {
        int layerCount = _animator.layerCount;

        for (int layer = 0; layer < layerCount; layer++)
        {
            int hash = _pausedStateHashes[layer];
            float t = _pausedNormalizedTimes[layer];

            if (hash != 0)
                _animator.Play(hash, layer, t);
        }

        _animator.Update(0f);
    }
}
