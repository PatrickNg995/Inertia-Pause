using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : MonoBehaviour
{
    [field: SerializeField] public GameObject ObjectivesElements { get; private set; }
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text ScenarioObjectivesText { get; private set; }
    [field: SerializeField] public TMP_Text OptionalObjectivesText { get; private set; }
    [field: SerializeField] public GameObject ButtonPrompts { get; private set; }
    [field: SerializeField] public GameObject RedoPrompt { get; private set; }
    [field: SerializeField] public TMP_Text TelemetryText { get; private set; }
    [field: SerializeField] public TMP_Text ActionsTakenText { get; private set; }
    [field: SerializeField] public Image DefaultCrosshair { get; private set; }
    [field: SerializeField] public TMP_Text InteractableNameText { get; private set; }
    [field: SerializeField] public TMP_Text InteractableActionText { get; set; }
    [field: SerializeField] public GameObject PromptsLooking { get; private set; }
    [field: SerializeField] public GameObject PromptsInteracting { get; private set; }

}
