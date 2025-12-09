using System.Collections.Generic;
using UnityEngine;

public class CreditsView : MonoBehaviour
{
    [field: SerializeField] public List<CanvasGroup> Slides { get; private set; }
    [field: SerializeField] public CustomButtonView BackButton { get; private set; }
}
