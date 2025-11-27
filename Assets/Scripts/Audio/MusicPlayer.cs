using System;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Tracks")]
    // TODO: implement music for level opening, or remove if not needed.
    [Tooltip("The music track to play during the opening to the level (currently unused).")]
    [SerializeField] private MusicTrack _openingTrack;

    [Tooltip("The music track to play during gameplay.")]
    [SerializeField] private MusicTrack _gameplayTrack;

    [Header("Settings")]
    [Tooltip("The master volume for the current music playback, used as a modifier for the current track's volume.")]
    [SerializeField] [Range(0f, 1f)] private float _masterVolume = 1f;

    [Header("Audio Source")]
    [SerializeField]  private AudioSource _activeSource;

    private MusicTrack _currentTrack;

    [Serializable]
    public struct MusicTrack
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float BaseVolume;
    }

    public static MusicPlayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Start playing the gameplay track by default.
        PlayTrack(_gameplayTrack);
    }

    public void PlayTrack(MusicTrack track)
    {
        // Set the current audio source and track.
        ApplyTrackToSource(_activeSource, track);
        _currentTrack = track;

        // Update the volume and play the track.
        UpdateVolume();
        _activeSource.Play();
    }

    public void PauseMusic()
    {
        _activeSource.Pause();
    }

    public void UnpauseMusic()
    {
        if (!_activeSource.isPlaying)
        {
            _activeSource.UnPause();
        }
    }

    public void StopMusic()
    {
        _activeSource.Stop();
        _activeSource.clip = null;
    }

    public void SetMasterVolume(float value)
    {
        // Set the master volume, then update the active source's volume.
        _masterVolume = Mathf.Clamp01(value);
        UpdateVolume();
    }

    private void ApplyTrackToSource(AudioSource source, MusicTrack track)
    {
        source.clip = track.Clip;
        source.loop = true;
        source.volume = track.BaseVolume * _masterVolume;
    }

    private void UpdateVolume()
    {
        // Update the active source's volume based on the current track's base volume and master volume.
        _activeSource.volume = _currentTrack.BaseVolume * _masterVolume;
    }
}
