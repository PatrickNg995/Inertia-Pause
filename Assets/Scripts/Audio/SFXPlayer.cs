using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxId
{
    // UI and menus.
    UIClick,

    // Time control.
    TimePauseEnter,
    TimePauseLoop,
    TimePauseExit,
    TimeObjectUnfreeze,

    // NPC damage and death.
    NPCBodyHitGround,

    // Objects.
    BulletFire,
    BulletImpactEnv,
    BulletImpactBody,
    ExplosionDefault,
    GrenadePinPull,
    GrenadeThrow,
    GrenadeBounce,
    GlassShatter,

    // Player actions.
    Walking,
    StartInteract,
    ConfirmInteract,
    ResetInteract,
    CancelInteract,
    ClimbLadder
}

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Serializable]
    private struct SfxDefinition
    {
        public SfxId Id;
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume;
    }

    public static SFXPlayer Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private List<SfxDefinition> _soundEffects = new List<SfxDefinition>();

    [Header("Master Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float _masterVolume = 1f;

    [Header("Pitch Randomization")]
    [SerializeField] private bool _enableRandomPitch = true;

    [SerializeField] private float _minPitch = 0.9f;
    [SerializeField] private float _maxPitch = 1.1f;

    [SerializeField] private AudioSource _audioSource;

    private readonly Dictionary<SfxId, SfxDefinition> _lookup = new Dictionary<SfxId, SfxDefinition>();

    // Track all 3D/attached AudioSources so we can rescale them when master volume changes.
    private readonly List<AudioSource> _trackedSources = new List<AudioSource>();

    public float MasterVolume
    {
        get
        {
            return _masterVolume;
        }
        set
        {
            float clamped = Mathf.Clamp01(value);

            if (Mathf.Approximately(clamped, _masterVolume))
            {
                return;
            }

            float oldValue = _masterVolume;
            _masterVolume = clamped;

            ApplyMasterScaleToTrackedSources(oldValue, _masterVolume);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        BuildLookup();
    }

    public AudioClip GetClip(SfxId id)
    {
        if (_lookup.TryGetValue(id, out SfxDefinition sfx))
        {
            return sfx.Clip;
        }

        return null;
    }

    private void BuildLookup()
    {
        _lookup.Clear();

        for (int i = 0; i < _soundEffects.Count; i++)
        {
            SfxDefinition s = _soundEffects[i];

            if (s.Id < 0)
            {
                continue;
            }

            if (!_lookup.ContainsKey(s.Id))
            {
                _lookup.Add(s.Id, s);
            }
        }
    }

    private float GetRandomPitch()
    {
        if (!_enableRandomPitch)
        {
            return 1f;
        }

        if (_minPitch >= _maxPitch)
        {
            return 1f;
        }

        return UnityEngine.Random.Range(_minPitch, _maxPitch);
    }

    public void Play(SfxId id)
    {
        if (id < 0)
        {
            return;
        }

        if (_audioSource == null)
        {
            return;
        }

        if (!_lookup.TryGetValue(id, out SfxDefinition s))
        {
            return;
        }

        if (s.Clip == null)
        {
            return;
        }

        float clipVolume = Mathf.Clamp01(s.Volume);
        float finalVolume = clipVolume * _masterVolume;
        float pitch = GetRandomPitch();

        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(s.Clip, finalVolume);
    }

    public void PlayAtPosition(SfxId id, Vector3 position)
    {
        if (!_lookup.TryGetValue(id, out SfxDefinition s))
        {
            return;
        }

        if (s.Clip == null)
        {
            return;
        }

        GameObject go = new GameObject($"SFX_{id}_OneShot");
        go.transform.position = position;

        AudioSource src = go.AddComponent<AudioSource>();
        float clipVolume = Mathf.Clamp01(s.Volume);

        src.clip = s.Clip;
        src.volume = clipVolume * _masterVolume;
        src.pitch = GetRandomPitch();
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.loop = false;
        src.Play();

        RegisterTrackedSource(src);

        Destroy(go, s.Clip.length);
    }

    public AudioSource PlayAttached(SfxId id, Transform parent, bool loop = false)
    {
        if (parent == null)
        {
            return null;
        }

        if (!_lookup.TryGetValue(id, out SfxDefinition s))
        {
            return null;
        }

        if (s.Clip == null)
        {
            return null;
        }

        GameObject go = new GameObject($"SFX_{id}");
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;

        AudioSource src = go.AddComponent<AudioSource>();
        float clipVolume = Mathf.Clamp01(s.Volume);

        src.clip = s.Clip;
        src.volume = clipVolume * _masterVolume;
        src.pitch = GetRandomPitch();
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.loop = loop;
        src.Play();

        RegisterTrackedSource(src);

        if (!loop)
        {
            Destroy(go, s.Clip.length);
        }

        return src;
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
    }

    private void RegisterTrackedSource(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        if (!_trackedSources.Contains(source))
        {
            _trackedSources.Add(source);
        }
    }

    private void ApplyMasterScaleToTrackedSources(float oldValue, float newValue)
    {
        // Avoid division by zero; if old is 0, just treat factor as new.
        if (Mathf.Approximately(oldValue, 0f))
        {
            oldValue = 0.0001f;
        }

        float factor = newValue / oldValue;

        for (int i = _trackedSources.Count - 1; i >= 0; i--)
        {
            AudioSource src = _trackedSources[i];

            if (src == null)
            {
                _trackedSources.RemoveAt(i);
                continue;
            }

            src.volume *= factor;
        }
    }
}
