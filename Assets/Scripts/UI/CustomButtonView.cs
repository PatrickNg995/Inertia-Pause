using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Action<string> OnHover;

    [field: Header("Components")]
    [field: SerializeField] public Button Button { get; private set; }
    [field: SerializeField] private TMP_Text _text;

    [field: Header("Settings")]
    [field: SerializeField] private string _hint;
    [field: SerializeField] private Color _defaultColor;
    [field: SerializeField] private Color _highlightColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _highlightColor;
        OnHover?.Invoke(_hint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _defaultColor;
        OnHover?.Invoke(_hint);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _text.color = _highlightColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _text.color = _defaultColor;
    }
}
