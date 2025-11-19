using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectView : MonoBehaviour
{
    [field: SerializeField] public Image BackgroundImage { get; private set; }
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text LevelDescriptionText { get; private set; }

    [field: SerializeField] public List<CustomLevelSelectButtonView> LevelSelectButtons { get; private set; }
    [field: SerializeField] public CustomButtonView BackButton { get; private set; }

    [field: SerializeField] public LevelSelectObjectiveRowView OptionalObjectivesPrefab { get; private set; }

    [field: SerializeField] public TMP_Text NormalBestRecordText { get; private set; }
    [field: SerializeField] public GameObject NormalOptionalObjectivesContainer { get; private set; }

    [field: SerializeField] public TMP_Text HardBestRecordText { get; private set; }
    [field: SerializeField] public GameObject HardOptionalObjectivesContainer { get; private set; }

    [field: Header("Difficulty Popup")]
    [field: SerializeField] public GameObject DifficultyPopup { get; private set; }
    [field: SerializeField] public CustomButtonView NormalDifficultyButton { get; private set; }
    [field: SerializeField] public CustomButtonView HardDifficultyButton { get; private set; }
    [field: SerializeField] public CustomButtonView DifficultyBackButton { get; private set; }
}
