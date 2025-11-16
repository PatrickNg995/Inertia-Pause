using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _frameToLoad = -1;
    [SerializeField] private float _totalFrames;

    private float _animationStartTime = -1;
    private int _pausedStateHash;
    private float _pausedNormalizedTime;
    private bool _isPaused;

    private void Awake()
    {
        if (_frameToLoad < 0 )
            _animationStartTime = Random.value;
        else
            _animationStartTime = _frameToLoad / _totalFrames;
            _animator.Play(0, 0, _animationStartTime);
        _animator.Update(0f);
    }

    public void Pause()
    {
        if (_isPaused) return;

        Debug.Log("Animator has paused.");

        // Capture current state + time.
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _pausedStateHash = stateInfo.fullPathHash;
        _pausedNormalizedTime = stateInfo.normalizedTime;

        _animator.speed = 0f;
        _isPaused = true;
    }

    public void Unpause()
    {
        if (!_isPaused) return;

        Debug.Log("Animator has unpaused.");
        _animator.speed = 1f;
        _isPaused = false;
    }

    public void ResetStateBeforeUnpause()
    {
        if (!_isPaused) return;

        _animator.Play(_pausedStateHash, 0, _pausedNormalizedTime);
        _animator.Update(0f);
    }
}
