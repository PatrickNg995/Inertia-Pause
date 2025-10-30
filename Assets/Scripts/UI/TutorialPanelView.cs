using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text Header { get; private set; }
    [field: SerializeField] public TMP_Text ContentText { get; private set; }
    [field: SerializeField] public Image ContentImage { get; private set; }
    [field: SerializeField] public TMP_Text PageText { get; private set; }
    [field: SerializeField] public Button NextPageButton { get; private set; }
    [field: SerializeField] public Button PrevPageButton { get; private set; }
    [field: SerializeField] public Button BackButton { get; private set; }
}
