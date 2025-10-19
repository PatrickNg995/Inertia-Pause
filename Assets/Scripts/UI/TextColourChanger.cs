using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextColourChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: Header("Components")]
    [field: SerializeField] private TMP_Text _text;

    [field: Header("Settings")]
    [field: SerializeField] private Color _defaultColor;
    [field: SerializeField] private Color _highlightColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _defaultColor;
    }
}
