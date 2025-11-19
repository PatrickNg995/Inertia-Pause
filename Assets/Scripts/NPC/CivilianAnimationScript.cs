using UnityEngine;

public class CivilianAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayRelievedAnimation()
    {
        _animator.SetBool("HasWon", true);
    }

    // Gives us more room for separate animations later.
}
