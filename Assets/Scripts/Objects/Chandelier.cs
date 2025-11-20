using UnityEngine;

public class Chandelier : MonoBehaviour, IPausable
{
    [SerializeField] GameObject _chandelierBottom;
    [SerializeField] Rigidbody _chandelierBottomRb;

    private Vector3 _chandelierBottomPosition;
    private Quaternion _chandelierBottomRotation;
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
        _chandelierBottomRb.isKinematic = true;
    }

    public void Unpause()
    {
        _chandelierBottomPosition = _chandelierBottom.transform.position;
        _chandelierBottomRotation = _chandelierBottom.transform.rotation;
        _chandelierBottomRb.isKinematic = false;
        return;
    }

    public void ResetStateBeforeUnpause()
    {
        _chandelierBottom.tag = "Untagged";

        // Reset position & rotation of chandelier bottom to pre-unpause state.
        _chandelierBottom.transform.SetPositionAndRotation(_chandelierBottomPosition,
            _chandelierBottomRotation);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
}
