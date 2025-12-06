using TMPro;
using UnityEngine;

public class ReviewPanelView : MonoBehaviour
{
    [field: Header("Information")]
    [field: SerializeField] public TMP_Text LevelNameText { get; private set; }
    [field: SerializeField] public TMP_Text CurrentCameraText { get; private set; }

    [field: Header("Buttons")]
    [field: SerializeField] public CustomButtonView PreviousButton { get; private set; }
    [field: SerializeField] public CustomButtonView NextButton { get; private set; }
    [field: SerializeField] public CustomButtonView BackButton { get; private set; }

}
