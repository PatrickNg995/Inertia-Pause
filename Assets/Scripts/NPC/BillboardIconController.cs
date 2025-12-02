using System;
using UnityEngine;

public class BillboardIconController : MonoBehaviour, IPausable
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

    public void Pause()
    {
        // No implementation needed for pausing billboard icons.
    }

    public void Unpause()
    {
        // No implementation needed for unpausing billboard icons.
    }

    public void ResetStateBeforeUnpause()
    {
        switch (_iconAppearOnState)
        {
            case IconAppearOnState.AppearOnDeath:
                _iconObject.SetActive(_npc.LastCauseOfDeath != null);
                break;
            case IconAppearOnState.AppearOnSurvive:
                _iconObject.SetActive(_npc.LastCauseOfDeath == null);
                break;
            default:
                Debug.LogError("Unsupported IconAppearOnState in BillboardIconController.");
                return;
        }
    }
}
