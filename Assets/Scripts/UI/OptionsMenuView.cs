using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuView : MonoBehaviour
{
    [field: Header("Bottom Bar")]
    [field: SerializeField] public Button BackButton { get; private set; }
}
