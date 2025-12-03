using UnityEngine;

public class BillboardIconController : MonoBehaviour
{
    [Header("Icon Appearance Settings")]
    [SerializeField] private IconAppearOnState _iconAppearOnState;

    [Header("References")]
    [SerializeField] private NPC _npc;
    [SerializeField] private GameObject _iconObject;

    private Transform _mainCamera;

    private enum IconAppearOnState
    {
        AppearOnDeath,
        AppearOnSurvive
    }

    private void Start()
    {
        _mainCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(_mainCamera);
    }

    public void UpdateBillboardIconState()
    {
        switch (_iconAppearOnState)
        {
            case IconAppearOnState.AppearOnDeath:
                // Show icon if the NPC died on the last attempt.
                _iconObject.SetActive(_npc.HasDiedLastAttempt == true);
                break;
            case IconAppearOnState.AppearOnSurvive:
                // Show icon if NPC survived the last attempt.
                _iconObject.SetActive(_npc.HasDiedLastAttempt == false);
                break;
            default:
                Debug.LogError("Unsupported IconAppearOnState in BillboardIconController.");
                return;
        }
    }

    public void DisableBillboardIcon()
    {
        _iconObject.SetActive(false);
    }
}
