using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxId
{
    None = 0,

    // UI and menus.
    UIClick = 10,

    // Time control.
    TimePauseEnter = 20,
    TimePauseLoop = 21,
    TimePauseExit = 22,
    TimeObjectUnfreeze = 23,

    // NPC damage and death.
    NPCBodyHitGround = 30,

    // Objects.
    BulletFire = 40,
    BulletImpactEnv = 41,
    BulletImpactBody = 42,
    ExplosionDefault = 43,
    GrenadePinPull = 44,
    GrenadeThrow = 45,
    GrenadeBounce = 46,
    GlassShatter = 47,

    // Player actions.
    Walking = 50,
    Interact = 51
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
    [SerializeField]
    private List<SfxDefinition> _soundEffects = new List<SfxDefinition>();

    [Header("Master Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _masterVolume = 1f;

    [Header("Pitch Randomization")]
    [SerializeField]
    private bool _enableRandomPitch = true;

    [SerializeField]
    private float _minPitch = 0.9f;

    [SerializeField]
    private float _maxPitch = 1.1f;

    private readonly Dictionary<SfxId, SfxDefinition> _lookup =
        new Dictionary<SfxId, SfxDefinition>();

    [SerializeField]
    private AudioSource _audioSource;

    public float MasterVolume
    {
        get => _masterVolume;
        set => _masterVolume = Mathf.Clamp01(value);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildLookup();
    }

    public AudioClip GetClip(SfxId id)
    {
        if (_lookup.TryGetValue(id, out var sfx))
            return sfx.Clip;

        return null;
    }

    private void BuildLookup()
    {
        _lookup.Clear();

        for (int i = 0; i < _soundEffects.Count; i++)
        {
            SfxDefinition s = _soundEffects[i];

            if (s.Id == SfxId.None)
                continue;

            if (!_lookup.ContainsKey(s.Id))
                _lookup.Add(s.Id, s);
        }
    }

    private float GetRandomPitch()
    {
        if (!_enableRandomPitch)
            return 1f;

        if (_minPitch >= _maxPitch)
            return 1f;

        return UnityEngine.Random.Range(_minPitch, _maxPitch);
    }

    public void Play(SfxId id)
    {
        if (id == SfxId.None)
            return;

        if (!_lookup.TryGetValue(id, out SfxDefinition s))
            return;

        if (s.Clip == null)
            return;

        float v = s.Volume * _masterVolume;
        float pitch = GetRandomPitch();

        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(s.Clip, v);
    }

    public void PlayAtPosition(SfxId id, Vector3 position)
    {
        if (!_lookup.TryGetValue(id, out SfxDefinition s))
            return;

        if (s.Clip == null)
            return;

        GameObject go = new GameObject($"SFX_{id}_OneShot");
        go.transform.position = position;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = s.Clip;
        src.volume = s.Volume * _masterVolume;
        src.pitch = GetRandomPitch();
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.loop = false;
        src.Play();

        Destroy(go, s.Clip.length);
    }

    public AudioSource PlayAttached(SfxId id, Transform parent, bool loop = false)
    {
        if (parent == null)
            return null;

        if (!_lookup.TryGetValue(id, out SfxDefinition s))
            return null;

        if (s.Clip == null)
            return null;

        GameObject go = new GameObject($"SFX_{id}");
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = s.Clip;
        src.volume = s.Volume * _masterVolume;
        src.pitch = GetRandomPitch();
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.loop = loop;
        src.Play();

        if (!loop)
            Destroy(go, s.Clip.length);

        return src;
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
    }
}
