using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomLevelSelectButtonView : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public Action<ScenarioInfo, ScenarioInfo> OnHoverLevel;
    public Action<ScenarioInfo, ScenarioInfo> OnSelectLevel;

    [SerializeField] private ScenarioInfo _normalScenarioInfo;
    [SerializeField] private ScenarioInfo _hardScenarioInfo;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverLevel?.Invoke(_normalScenarioInfo, _hardScenarioInfo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelectLevel?.Invoke(_normalScenarioInfo, _hardScenarioInfo);
    }
}
