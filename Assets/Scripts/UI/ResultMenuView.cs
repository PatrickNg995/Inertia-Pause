using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultMenuView : MonoBehaviour
{
    [field: Header("Prefabs")]
    [field: SerializeField] public ObjectiveRowView ObjectiveRowPrefab { get; private set; }

    [field: Header("Information")]
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text CompletionText { get; private set; }

    [field: Header("Results")]
    [field: SerializeField] public TMP_Text ActionCountText { get; private set; }
    [field: SerializeField] public GameObject NewRecord { get; private set; }
    [field: SerializeField] public GameObject ObjectiveRowContainer { get; private set; }


    [field: Header("Main Buttons")]
    [field: SerializeField] public CustomButtonView NextButton { get; private set; }
    [field: SerializeField] public CustomButtonView RewindButton { get; private set; }
    [field: SerializeField] public CustomButtonView RestartButton { get; private set; }
    [field: SerializeField] public CustomButtonView MainMenuButton { get; private set; }

    [field: Header("Bottom Bar")]
    [field: SerializeField] public TMP_Text DescriptionText { get; private set; }
}
