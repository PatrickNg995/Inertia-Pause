using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures exactly one AudioListener is always active in the scene.
///
/// Lives on a persistent GameObject (AdditiveSceneManager). Adds an AudioListener
/// component to itself at runtime, kept disabled while the PlayerCapsule (or any
/// other) AudioListener is present. When scenes unload and no listener exists,
/// this one enables itself so SFX keep playing during scene transitions and the
/// "no audio listeners" warning never fires.
/// </summary>
[DisallowMultipleComponent]
public class FallbackAudioListener : MonoBehaviour
{
    private AudioListener _fallback;

    private void Awake()
    {
        _fallback = GetComponent<AudioListener>();
        if (_fallback == null)
        {
            _fallback = gameObject.AddComponent<AudioListener>();
        }

        _fallback.enabled = false;
        Refresh();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Refresh();
    private void OnSceneUnloaded(Scene scene) => Refresh();

    // Cheap safety net — handles the frame between Destroy() of a listener
    // and the next scene event firing.
    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_fallback == null) return;

        AudioListener[] listeners = FindObjectsByType<AudioListener>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        bool otherActive = false;
        for (int i = 0; i < listeners.Length; i++)
        {
            AudioListener l = listeners[i];
            if (l == _fallback) continue;
            if (l.isActiveAndEnabled)
            {
                otherActive = true;
                break;
            }
        }

        // Enable fallback only when no other listener is present.
        if (_fallback.enabled == otherActive)
        {
            _fallback.enabled = !otherActive;
        }
    }
}
