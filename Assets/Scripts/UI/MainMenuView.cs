using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [field: Header("Start Screen")]
    [field: SerializeField] public GameObject StartScreen { get; private set; }
    [field: SerializeField] public TMP_Text StartText { get; private set; }
    [field: SerializeField] public TMP_Text BuildText { get; private set; }

    [field: Header("Main Menu")]
    [field: SerializeField] public GameObject MainMenuScreen { get; private set; }
    [field: SerializeField] public TMP_Text DescriptionText { get; private set; }
    [field: SerializeField] public CustomButtonView ContinueButton { get; private set; }
    [field: SerializeField] public CustomButtonView NewGameButton { get; private set; }
    [field: SerializeField] public CustomButtonView ScenarioSelectButton { get; private set; }
    [field: SerializeField] public CustomButtonView OptionsButton { get; private set; }
    [field: SerializeField] public CustomButtonView ExitButton { get; private set; }
}
