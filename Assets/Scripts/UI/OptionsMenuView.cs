using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuView : MonoBehaviour
{
    [field: Header("Information")]
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text DescriptionText { get; private set; }

    [field: Header("Option Buttons")]
    [field: SerializeField] public CustomOptionButton HorizontalSensitivityButton { get; private set; }
    [field: SerializeField] public CustomOptionButton VerticalSensitivityButton { get; private set; }
    [field: SerializeField] public CustomOptionButton FOVButton { get; private set; }
    [field: SerializeField] public CustomOptionButton MaxFramerateButton { get; private set; }
    [field: SerializeField] public CustomOptionButton ShowMetricsButton { get; private set; }
    [field: SerializeField] public CustomOptionButton MusicVolumeButton { get; private set; }
    [field: SerializeField] public CustomOptionButton SFXVolumeButton { get; private set; }
    [field: SerializeField] public CustomOptionButton ShowObjectTrajectoryButton { get; private set; }

    [field: Header("Individual Options")]
    [field: SerializeField] public List<Button> FramerateOptions { get; private set; }
    [field: SerializeField] public List<Button> FOVOptions { get; private set; }
    [field: SerializeField] public List<Button> SensitivityOptions { get; private set; }
    [field: SerializeField] public List<Button> VolumeOptions { get; private set; }

    [field: Header("Bottom Bar")]
    [field: SerializeField] public Button BackButton { get; private set; }
}
