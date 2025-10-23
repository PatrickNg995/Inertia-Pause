using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayRelievedAnimation()
    {
        _animator.SetBool("HasWon", true);
    }
}
