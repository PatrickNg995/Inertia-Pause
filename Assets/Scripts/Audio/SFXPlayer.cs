using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxId
{
    None = 0,

    // ============================================================
    // UI / MENUS
    // ============================================================
    UiHover = 10,
    UiClick = 11,
    UiBack = 12,
    UiMenuOpen = 13,
    UiMenuClose = 14,

    // ============================================================
    // TIME CONTROL MECHANICS
    // ============================================================
    TimePauseEnter = 20,
    TimePauseLoop = 21,
    TimePauseExit = 22,
    TimeObjectUnfreeze = 23,

    // ============================================================
    // NPC DAMAGE & DEATH (NPCs do not move)
    // ============================================================
    NpcDamage = 30,
    NpcDeath = 31,
    NpcBodyHitGround = 32,

    // ============================================================
    // GUNFIRE / PROJECTILES
    // ============================================================
    BulletFire = 40,
    BulletImpactWood = 41,
    BulletImpactConcrete = 42,
    BulletImpactBody = 43,
    BulletRicochet = 44,
    BulletWhizBy = 45, // optional but very good for atmosphere

    // ============================================================
    // EXPLOSIONS & DEBRIS
    // ============================================================
    ExplosionDefault = 60,
    ExplosionDebris = 61,

    // ============================================================
    // EXPLOSIVE BARRELS
    // ============================================================
    BarrelIgnite = 70,
    BarrelExplosion = 71,

    // ============================================================
    // GRENADE
    // ============================================================
    GrenadePinPull = 80,
    GrenadeThrow = 81,
    GrenadeBounce = 82,
    GrenadeExplosion = 83,

    // ============================================================
    // MISSILES
    // ============================================================
    MissileLaunch = 90,
    MissileImpact = 91,

    // ============================================================
    // MINES
    // ============================================================
    MineArm = 100,
    MineTrigger = 101,
    MineExplosion = 102,

    // ============================================================
    // ENVIRONMENT / PROPS
    // ============================================================
    PropImpact = 120,
    WoodBoxHit = 121,
    WoodBoxSlide = 122,
    WoodBoxBreak = 123,

    // ============================================================
    // AMBIENCE
    // ============================================================
    AmbienceIndoor = 150
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

    // Cached lookup table – no GetComponent required
    private readonly Dictionary<SfxId, SfxDefinition> _lookup =
        new Dictionary<SfxId, SfxDefinition>();

    // Cached reference (only GetComponent call!)
    [SerializeField] private AudioSource _audioSource;

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

    private void BuildLookup()
    {
        _lookup.Clear();

        for (int i = 0; i < _soundEffects.Count; i++)
        {
            SfxDefinition sfx = _soundEffects[i];

            if (sfx.Id == SfxId.None)
            {
                continue;
            }

            if (!_lookup.ContainsKey(sfx.Id))
            {
                _lookup.Add(sfx.Id, sfx);
            }
        }
    }

    public void Play(SfxId id)
    {
        if (id == SfxId.None)
        {
            return;
        }

        if (!_lookup.TryGetValue(id, out SfxDefinition sfx))
        {
            return;
        }

        if (sfx.Clip == null)
        {
            return;
        }

        // No GetComponent – uses cached AudioSource
        _audioSource.PlayOneShot(sfx.Clip, sfx.Volume);
    }

    public void PlayAtPosition(SfxId id, Vector3 position)
    {
        if (!_lookup.TryGetValue(id, out SfxDefinition sfx))
        {
            return;
        }

        if (sfx.Clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(sfx.Clip, position, sfx.Volume);
    }
}
