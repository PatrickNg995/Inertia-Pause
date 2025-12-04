using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SfxTestPanelGenerator : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Parent under which the generated buttons will be placed (e.g., a Panel with VerticalLayoutGroup).")]
    [SerializeField] private RectTransform _buttonsParent;

    [Tooltip("Button prefab that will be cloned for each SfxId. Must have a Text (or TMP) child for the label.")]
    [SerializeField] private Button _buttonPrefab;

    [Header("SFX Player")]
    [Tooltip("Optional explicit reference to your global SFXPlayer. If left null, will try SFXPlayer.Instance.")]
    [SerializeField] private SFXPlayer _sfxPlayer;

    [Header("Options")]
    [Tooltip("Clear all existing children under Buttons Parent before generating.")]
    [SerializeField] private bool _clearExistingChildren = true;

    private void Awake()
    {
        // Try to auto-wire to the singleton if not assigned.
        if (_sfxPlayer == null)
        {
            _sfxPlayer = SFXPlayer.Instance;
        }
    }

    private void Start()
    {
        if (_buttonsParent == null)
        {
            Debug.LogError("[SfxTestPanelGenerator] Buttons Parent is not assigned.");
            return;
        }

        if (_buttonPrefab == null)
        {
            Debug.LogError("[SfxTestPanelGenerator] Button Prefab is not assigned.");
            return;
        }

        if (_sfxPlayer == null)
        {
            Debug.LogError("[SfxTestPanelGenerator] SFXPlayer reference is null and Instance is not available.");
            return;
        }

        if (_clearExistingChildren)
        {
            ClearChildren(_buttonsParent);
        }

        GenerateButtons();
    }

    private void ClearChildren(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
#endif
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void GenerateButtons()
    {
        Array values = Enum.GetValues(typeof(SfxId));

        foreach (var raw in values)
        {
            SfxId id = (SfxId)raw;

            // Skip None
            if (id == SfxId.None)
            {
                continue;
            }

            CreateButtonForSfx(id);
        }
    }

    private void CreateButtonForSfx(SfxId id)
    {
        Button btn = Instantiate(_buttonPrefab, _buttonsParent);
        btn.name = $"Button_{id}";

        // Try normal UnityEngine.UI.Text
        var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = id.ToString();
        }


        // (Optional) If you're using TextMeshProUGUI instead, you can add:
        // var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        // if (tmp != null) tmp.text = id.ToString();

        SfxId capturedId = id;
        btn.onClick.AddListener(() => OnButtonClicked(capturedId));
    }

    private void OnButtonClicked(SfxId id)
    {
        if (_sfxPlayer == null)
        {
            Debug.LogWarning("[SfxTestPanelGenerator] SFXPlayer is null when trying to play: " + id);
            return;
        }

        // Uses your global SFXPlayer 2D / non-positional Play method:
        //   public void Play(SfxId id)
        _sfxPlayer.Play(id);

        Debug.Log($"[SfxTestPanelGenerator] Played SFX: {id}");
    }
}
