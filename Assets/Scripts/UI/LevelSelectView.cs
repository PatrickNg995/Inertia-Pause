using TMPro;
using UnityEngine;

public class LevelSelectView : MonoBehaviour
{
    [field: SerializeField] private TMP_Text LevelNameText;
    [field: SerializeField] private TMP_Text LevelDescriptionText;

    [field: SerializeField] private LevelSelectObjectiveRowView OptionalObjectivesTemplate;

    [field: SerializeField] private TMP_Text NormalBestRecordText;
    [field: SerializeField] private GameObject NormalOptionalObjectivesContainer;

    [field: SerializeField] private TMP_Text HardBestRecordText;
    [field: SerializeField] private GameObject HardOptionalObjectivesContainer;
}
