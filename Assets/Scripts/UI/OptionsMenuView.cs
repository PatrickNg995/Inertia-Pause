using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuView : MonoBehaviour
{
    [field: Header("Information")]
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }

    [field: Header("Option Buttons")]

    [field: Header("Bottom Bar")]
    [field: SerializeField] public Button BackButton { get; private set; }
}
