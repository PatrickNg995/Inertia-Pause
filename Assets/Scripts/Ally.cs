using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayRelievedAnimation()
    {
        Debug.Log("Playing Relieved Animation");
        _animator.SetBool("HasWon", true);
    }
}
