using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    // ---------- Singleton ----------
    public static MusicPlayer Instance { get; private set; }

    [Header("Tracks")]
    [Tooltip("Define all background music tracks here, referenced by an ID (e.g. 'office', 'bridge', 'city_center').")]
    [SerializeField] private List<MusicTrack> _tracks = new List<MusicTrack>();

    [Header("Crossfade Settings")]
    [SerializeField, Tooltip("Default fade duration when crossfading between tracks.")]
    private float _defaultFadeTime = 1.5f;

    [SerializeField, Tooltip("Master volume for all music.")]
    [Range(0f, 1f)] private float _masterVolume = 1f;

    [Header("Scene to Track Mapping (Optional)")]
    [Tooltip("If enabled, this will automatically pick a track when a scene is loaded.")]
    [SerializeField] private bool _autoPlayOnSceneLoad = false;

    [SerializeField, Tooltip("Map scene names to track IDs here.")]
    private List<SceneTrackMapping> _sceneTrackMappings = new List<SceneTrackMapping>();

    private AudioSource _activeSource;
    private AudioSource _fadeSource;

    private Coroutine _fadeCoroutine;

    // ---------- Data Types ----------
    [Serializable]
    private struct MusicTrack
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Serializable]
    private struct SceneTrackMapping
    {
        public string sceneName;
        public string trackId;
    }

    // ---------- Unity Lifecycle ----------
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup sources
        _activeSource = GetComponent<AudioSource>();
        _activeSource.playOnAwake = false;
        _activeSource.loop = true;

        _fadeSource = gameObject.AddComponent<AudioSource>();
        _fadeSource.playOnAwake = false;
        _fadeSource.loop = true;

        _activeSource.volume = 0f;
        _fadeSource.volume = 0f;

        if (_autoPlayOnSceneLoad)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_autoPlayOnSceneLoad) return;

        string sceneName = scene.name;
        string trackId = GetTrackIdForScene(sceneName);

        if (!string.IsNullOrEmpty(trackId))
        {
            CrossfadeTo(trackId, _defaultFadeTime);
        }
    }

    // ---------- Public API ----------
    public void PlayImmediate(string trackId)
    {
        MusicTrack? track = FindTrack(trackId);
        if (track == null)
        {
            Debug.LogWarning("[MusicPlayer] Track ID '" + trackId + "' not found.");
            return;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        ApplyTrackToSource(_activeSource, track.Value);
        _fadeSource.Stop();
        _fadeSource.clip = null;

        _activeSource.volume = track.Value.volume * _masterVolume;
        _activeSource.Play();
    }

    public void CrossfadeTo(string trackId, float fadeTime = -1f)
    {
        MusicTrack? track = FindTrack(trackId);
        if (track == null)
        {
            Debug.LogWarning("[MusicPlayer] Track ID '" + trackId + "' not found.");
            return;
        }

        if (fadeTime < 0f) fadeTime = _defaultFadeTime;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(CrossfadeCoroutine(track.Value, fadeTime));
    }

    public void StopMusic(float fadeTime = 0.5f)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        if (fadeTime <= 0f)
        {
            _activeSource.Stop();
            _fadeSource.Stop();
            _activeSource.clip = null;
            _fadeSource.clip = null;
            return;
        }

        _fadeCoroutine = StartCoroutine(FadeOutAllCoroutine(fadeTime));
    }

    public void SetMasterVolume(float value)
    {
        _masterVolume = Mathf.Clamp01(value);

        _activeSource.volume *= _masterVolume;
        _fadeSource.volume *= _masterVolume;
    }

    // ---------- Helpers ----------
    private MusicTrack? FindTrack(string trackId)
    {
        foreach (var track in _tracks)
        {
            if (string.Equals(track.id, trackId, StringComparison.OrdinalIgnoreCase))
                return track;
        }
        return null;
    }

    private void ApplyTrackToSource(AudioSource source, MusicTrack track)
    {
        source.clip = track.clip;
        source.loop = true;
        source.volume = track.volume * _masterVolume;
    }

    private string GetTrackIdForScene(string sceneName)
    {
        foreach (var mapping in _sceneTrackMappings)
        {
            if (string.Equals(mapping.sceneName, sceneName, StringComparison.OrdinalIgnoreCase))
                return mapping.trackId;
        }
        return null;
    }

    private IEnumerator CrossfadeCoroutine(MusicTrack nextTrack, float duration)
    {
        AudioSource oldSource = _activeSource;
        AudioSource newSource = _fadeSource;

        _activeSource = newSource;
        _fadeSource = oldSource;

        ApplyTrackToSource(_activeSource, nextTrack);
        _activeSource.volume = 0f;
        _activeSource.Play();

        float startOldVolume = _fadeSource.volume;
        float targetNewVolume = nextTrack.volume * _masterVolume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            _fadeSource.volume = Mathf.Lerp(startOldVolume, 0f, t);
            _activeSource.volume = Mathf.Lerp(0f, targetNewVolume, t);

            yield return null;
        }

        _fadeSource.Stop();
        _fadeSource.clip = null;

        _activeSource.volume = targetNewVolume;
        _fadeCoroutine = null;
    }

    private IEnumerator FadeOutAllCoroutine(float duration)
    {
        float startVolA = _activeSource.volume;
        float startVolB = _fadeSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            _activeSource.volume = Mathf.Lerp(startVolA, 0f, t);
            _fadeSource.volume = Mathf.Lerp(startVolB, 0f, t);

            yield return null;
        }

        _activeSource.Stop();
        _fadeSource.Stop();
        _activeSource.clip = null;
        _fadeSource.clip = null;

        _fadeCoroutine = null;
    }
}
