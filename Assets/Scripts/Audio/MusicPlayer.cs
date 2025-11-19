using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    #region Constants

    private const float FADE_TIME_USE_DEFAULT = -1f;
    private const float STOP_FADE_TIME_DEFAULT = 0.5f;

    #endregion

    #region Singleton

    public static MusicPlayer Instance { get; private set; }

    #endregion

    #region Serialized Fields

    [Header("Tracks")]
    [SerializeField]
    private List<MusicTrack> _tracks = new List<MusicTrack>();

    [Header("Crossfade Settings")]
    [SerializeField]
    private float _defaultFadeTime = 1.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _masterVolume = 1f;

    [Header("Audio Sources (Assign in Inspector)")]
    [SerializeField]
    private AudioSource _activeSource;

    [SerializeField]
    private AudioSource _fadeSource;

    #endregion

    #region Private Fields

    private Coroutine _fadeCoroutine;

    #endregion

    #region Data Types

    [Serializable]
    private struct MusicTrack
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Public API

    public void PlayImmediate(int trackIndex)
    {
        if (!IsValidTrackIndex(trackIndex))
        {
            Debug.LogWarning("[MusicPlayer] Invalid track Index: " + trackIndex);
            return;
        }

        MusicTrack track = _tracks[trackIndex];

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        ApplyTrackToSource(_activeSource, track);
        _fadeSource.Stop();
        _fadeSource.clip = null;

        _activeSource.volume = track.volume * _masterVolume;
        _activeSource.Play();
    }

    public void CrossfadeTo(int trackIndex, float fadeTime = FADE_TIME_USE_DEFAULT)
    {
        if (!IsValidTrackIndex(trackIndex))
        {
            Debug.LogWarning("[MusicPlayer] Invalid track Index: " + trackIndex);
            return;
        }

        if (fadeTime == FADE_TIME_USE_DEFAULT)
        {
            fadeTime = _defaultFadeTime;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        MusicTrack track = _tracks[trackIndex];
        _fadeCoroutine = StartCoroutine(CrossfadeCoroutine(track, fadeTime));
    }

    public void StopMusic(float fadeTime = STOP_FADE_TIME_DEFAULT)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        if (fadeTime <= 0)
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

    #endregion

    #region Helpers

    private bool IsValidTrackIndex(int index)
    {
        return index >= 0 && index < _tracks.Count;
    }

    private void ApplyTrackToSource(AudioSource source, MusicTrack track)
    {
        source.clip = track.clip;
        source.loop = true;
        source.volume = track.volume * _masterVolume;
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

    #endregion
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_activeSource == _fadeSource)
        {
            Debug.LogError("[MusicPlayer] ActiveSource and FadeSource cannot reference the same AudioSource!");
        }

        if (_activeSource != null && _activeSource.clip != null)
        {
            Debug.LogWarning("[MusicPlayer] ActiveSource should not have a clip assigned in the Inspector.");
        }

        if (_fadeSource != null && _fadeSource.clip != null)
        {
            Debug.LogWarning("[MusicPlayer] FadeSource should not have a clip assigned in the Inspector.");
        }
    }
#endif
}
