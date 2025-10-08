using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewInteractableObjectInfo", menuName = "Inertia Pause/Interactable Info")]
public class InteractableObjectInfo : ScriptableObject
{
    public string ObjectName;
    public string ActionName;
    public TutorialInfo TutorialInfo;
}
