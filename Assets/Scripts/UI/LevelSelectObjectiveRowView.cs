using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectObjectiveRowView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text ObjectiveNameText { get; private set; }
    [field: SerializeField] public Image Check { get; private set; }
    [field: SerializeField] public Image Cross { get; private set; }
}
