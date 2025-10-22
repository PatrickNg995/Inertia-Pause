using UnityEngine;

public class PauseableAnimator : MonoBehaviour, IPausable
{
    Animator animator;
    float prevSpeed = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play(0, 0, Random.value);
        animator.Update(0f);
    }

    public void Pause()
    {
        Debug.Log("Animator has paused.");
        animator.speed = 0f;
    }

    public void Unpause()
    {
        Debug.Log("Animator has unpaused.");
        animator.speed = 1f;
    }
}
