using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    [SerializeField] private Animator _animator;
    private float _animationStartTime;

    private void Awake()
    {
        _animationStartTime = Random.value;
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
