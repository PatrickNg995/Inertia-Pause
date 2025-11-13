using UnityEngine;

public class Chandelier : MonoBehaviour, IPausable
{
    [SerializeField] GameObject _chandelierBottom;
    [SerializeField] Rigidbody _chandelierBottomRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _chandelierBottom.tag = "Untagged";
    }

    public void ChandelierFalling()
    {
        _chandelierBottom.tag = "Lethal";
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().freezeRotation = false;
        }
    }

    public void Pause()
    {
        return;
    }

    public void Unpause()
    {
        return;
    }

    public void ResetStateBeforeUnpause()
    {
        _chandelierBottom.tag = "Untagged";
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
}
