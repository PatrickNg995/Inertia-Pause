using TMPro;
using UnityEngine;

public class ObjectiveRowView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text ObjectiveNameText { get; private set; }
    [field: SerializeField] public TMP_Text ObjectiveStatusText { get; private set; }
}
