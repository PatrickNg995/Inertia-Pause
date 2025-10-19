using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    [field: Header("Information")]
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text ScenarioObjectivesText { get; private set; }
    [field: SerializeField] public TMP_Text OptionalObjectivesText { get; private set; }
    [field: SerializeField] public TMP_Text ActionsTakenText { get; private set; }

    [field: Header("Main Buttons")]
    [field: SerializeField] public Button ResumeButton { get; private set; }
    [field: SerializeField] public Button RestartButton { get; private set; }
    [field: SerializeField] public Button OptionsButton { get; private set; }
    [field: SerializeField] public Button QuitScenarioButton { get; private set; }

    [field: Header("Bottom Bar")]
    [field: SerializeField] public Button BackButton { get; private set; }
    [field: SerializeField] public TMP_Text DescriptionText { get; private set; }
}
