using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomLevelSelectButtonView : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler
{
    public Action<ScenarioInfo, ScenarioInfo> OnHoverLevel;
    public Action<ScenarioInfo, ScenarioInfo> OnConfirmLevel;

    [field: Header("Components")]
    [field: SerializeField] public Button Button { get; private set; }

    [Header("Scenario Info")]
    [SerializeField] private ScenarioInfo _normalScenarioInfo;
    [SerializeField] private ScenarioInfo _hardScenarioInfo;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverLevel?.Invoke(_normalScenarioInfo, _hardScenarioInfo);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnHoverLevel?.Invoke(_normalScenarioInfo, _hardScenarioInfo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnConfirmLevel?.Invoke(_normalScenarioInfo, _hardScenarioInfo);
    }
}
