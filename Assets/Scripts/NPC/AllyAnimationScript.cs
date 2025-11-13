using UnityEngine;

public class AllyAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayRelievedAnimation()
    {
        _animator.SetBool("HasWon", true);
    }
}
