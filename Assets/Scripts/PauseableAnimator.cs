using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator.Play(0, 0, Random.value);
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
}
