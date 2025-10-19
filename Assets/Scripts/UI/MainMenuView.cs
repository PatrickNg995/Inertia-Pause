using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text BuildText { get; private set; }
    [field: SerializeField] public Button ScenarioAButton { get; private set; }
    [field: SerializeField] public Button ScenarioBButton { get; private set; }
    [field: SerializeField] public Button ScenarioCButton { get; private set; }
    [field: SerializeField] public Button ExitButton { get; private set; }
}
