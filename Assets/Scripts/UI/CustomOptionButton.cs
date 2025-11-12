using TMPro;
using UnityEngine;

public class CustomOptionButton : CustomButtonView
{
    [field: Header("Option Button Settings")]
    [field: SerializeField] public TMP_Text OptionText { get; private set; }
    [field: SerializeField] public GameObject Foldout { get; private set; }

    public void OpenFoldout()
    {
        if (Foldout != null)
        {
            Foldout.SetActive(true);
        }
        _text.color = _highlightColor;
    }

    public void CloseFoldout()
    {
        if (Foldout != null)
        {
            Foldout.SetActive(false);
        }
        _text.color = _defaultColor;
    }
}
