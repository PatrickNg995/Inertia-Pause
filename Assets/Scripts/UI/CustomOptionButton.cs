using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomOptionButton : CustomButtonView
{
    [field: Header("Option Button Settings")]
    [field: SerializeField] public TMP_Text OptionText { get; private set; }
    [field: SerializeField] public GameObject Foldout { get; private set; }

    private bool _isFoldoutOpen = false;

    public void OpenFoldout()
    {
        if (Foldout != null)
        {
            Foldout.SetActive(true);
            _isFoldoutOpen = true;
        }
        _text.color = _highlightColor;
        OptionText.color = _highlightColor;
    }

    public void CloseFoldout()
    {
        if (Foldout != null)
        {
            Foldout.SetActive(false);
            _isFoldoutOpen = false;
        }
        _text.color = _defaultColor;
        OptionText.color = _defaultColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OptionText.color = _highlightColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_isFoldoutOpen)
        {
            base.OnPointerExit(eventData);
            OptionText.color = _defaultColor;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OptionText.color = _highlightColor;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (!_isFoldoutOpen)
        {
            base.OnDeselect(eventData);
            OptionText.color = _defaultColor;
        }
    }
}
