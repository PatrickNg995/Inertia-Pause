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
    [field: SerializeField] protected TMP_Text _text;

    [field: Header("Settings")]
    [field: SerializeField] private string _hint;
    [field: SerializeField] protected Color _defaultColor;
    [field: SerializeField] protected Color _highlightColor;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _highlightColor;
        OnHover?.Invoke(_hint);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _defaultColor;
        OnHover?.Invoke(_hint);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        _text.color = _highlightColor;
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        _text.color = _defaultColor;
    }
}
