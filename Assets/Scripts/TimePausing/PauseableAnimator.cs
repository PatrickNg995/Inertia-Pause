using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    [SerializeField] private Animator _animator;

    [SerializeField] private bool _useRandomStartTime = true;

    private float _animationStartTime;

    private void Awake()
    {
        _animationStartTime = _useRandomStartTime ? Random.value : 0;
        _animator.Play(0, 0, _animationStartTime);
        _animator.Update(0f);
    }

    public void Pause()
    {
        Debug.Log("Animator has paused.");
        _animator.speed = 0f;
    }

    public void Unpause()
    {
        Debug.Log("Animator has unpaused.");
        _animator.speed = 1f;
    }

    public void ResetStateBeforeUnpause()
    {
        _animator.Play(0, 0, _animationStartTime);
    }
}
