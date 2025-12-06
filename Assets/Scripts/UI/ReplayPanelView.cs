using TMPro;
using UnityEngine;

public class ReplayPanelView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text CameraText { get; private set; }
    [field: SerializeField] public CanvasGroup SkipPrompt { get; private set; }
}
