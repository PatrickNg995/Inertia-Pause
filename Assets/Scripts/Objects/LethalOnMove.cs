using UnityEngine;

public class LethalOnMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private const string LETHAL_TAG = "Lethal";
    private const string SAFE_TAG = "Untagged";

    private const float LETHAL_SPEED_THRESHOLD = 0.01f;

    private void Update()
    {
        if (_rb.linearVelocity.magnitude > LETHAL_SPEED_THRESHOLD)
        {
            gameObject.tag = LETHAL_TAG;
        }
        else
        {
            gameObject.tag = SAFE_TAG;
        }
    }
}
