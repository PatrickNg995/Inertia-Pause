using UnityEngine;

public class AllyAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayRelievedAnimation()
    {
        _animator.SetBool("HasWon", true);
    }

    // Gives us more room for separate animations later.
}
