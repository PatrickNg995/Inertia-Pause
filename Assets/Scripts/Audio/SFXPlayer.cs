using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global sound effect player for the game.
///
/// HOW TO SET UP IN SCENE:
/// 1. Create an empty GameObject in your main scene (e.g. "SFXPlayer").
/// 2. Add this SFXPlayer script to it.
/// 3. Unity will auto-add an AudioSource because of [RequireComponent].
/// 4. In the Inspector, drag the AudioSource into the "_audioSource" field.
/// 5. In the "_soundEffects" list, add entries for each SfxId you want
///    to use and assign an AudioClip + Volume.
/// 6. Make sure there is only ONE SFXPlayer in the game. This script
///    calls DontDestroyOnLoad, so it will persist across scenes.
///
/// HOW TO USE IN OTHER SCRIPTS:
/// - For UI or non-positional sounds (2D):
///       SFXPlayer.Instance.Play(SfxId.UiClick);
///
/// - For one-shot sounds at a position in the world (3D):
///       SFXPlayer.Instance.PlayAtPosition(SfxId.GrenadeExplosion, transform.position);
///
/// - For sounds that should follow a moving object (3D, optionally looped):
///       AudioSource engineAudio = SFXPlayer.Instance.PlayAttached(SfxId.MissileLaunch, transform, true);
///       // Later, when destroying the object:
///       if (engineAudio != null)
///       {
///           engineAudio.Stop();
///           Destroy(engineAudio.gameObject);
///       }
///
/// NOTES:
/// - The AudioListener should typically be on the player camera.
/// - Only add SfxId entries you actually use in gameplay.
/// - Volume is per-effect and gets multiplied by the AudioSource volume.
/// </summary>
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
    /// <summary>
    /// Defines a single sound effect entry:
    /// - Id: which SfxId this represents
    /// - Clip: the AudioClip to play
    /// - Volume: per-clip volume (0..1)
    ///
    /// These entries are configured in the Inspector on the SFXPlayer object.
    /// </summary>
    [Serializable]
    private struct SfxDefinition
    {
        public SfxId Id;
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume;
    }

    /// <summary>
    /// Global access to the SFXPlayer instance.
    /// Usage:
    ///     SFXPlayer.Instance.Play(SfxId.BulletFire);
    /// </summary>
    public static SFXPlayer Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField]
    private List<SfxDefinition> _soundEffects = new List<SfxDefinition>();

    // Cached lookup table: SfxId -> SfxDefinition for fast access
    private readonly Dictionary<SfxId, SfxDefinition> _lookup =
        new Dictionary<SfxId, SfxDefinition>();

    // Cached AudioSource reference on the SFXPlayer GameObject.
    // This is used for 2D / non-positional sounds.
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        // Enforce singleton: if another SFXPlayer exists, destroy this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object across scene loads so audio can be reused globally.
        DontDestroyOnLoad(gameObject);

        // Build internal lookup table from the list configured in the Inspector.
        BuildLookup();
    }

    /// <summary>
    /// Builds the dictionary from SfxId to SfxDefinition.
    /// Called once on Awake.
    /// </summary>
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

    /// <summary>
    /// Plays a sound effect using the central AudioSource on this object.
    /// This is best for:
    /// - UI sounds (click, hover)
    /// - Player-local sounds that should not be positional (2D)
    ///
    /// Example:
    ///     SFXPlayer.Instance.Play(SfxId.UiClick);
    /// </summary>
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

    /// <summary>
    /// Plays a one-shot sound at a specific world position.
    /// This creates a temporary 3D AudioSource at the given position.
    ///
    /// Use this for:
    /// - Explosions
    /// - Impacts
    /// - NPC death thuds
    ///
    /// Example:
    ///     SFXPlayer.Instance.PlayAtPosition(SfxId.GrenadeExplosion, transform.position);
    /// </summary>
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

    /// <summary>
    /// Plays a sound that is attached to a specific Transform.
    /// A new child GameObject with a 3D AudioSource is created and parented
    /// under "parent" so it follows the parent's movement.
    ///
    /// Use this for:
    /// - Missile engine loops
    /// - Moving hums / fields
    /// - Any sound that should move with an object
    ///
    /// Parameters:
    ///     id     - which sound to play
    ///     parent - transform to attach the sound to
    ///     loop   - true to loop the sound, false for one-shot
    ///
    /// Returns:
    ///     The AudioSource created for this sound (so callers can stop/destroy it),
    ///     or null if something failed (no definition or no clip).
    ///
    /// Example (missile):
    ///     private AudioSource _engineAudio;
    ///
    ///     void Start()
    ///     {
    ///         _engineAudio = SFXPlayer.Instance.PlayAttached(SfxId.MissileLaunch, transform, true);
    ///     }
    ///
    ///     void OnDestroy()
    ///     {
    ///         if (_engineAudio != null)
    ///         {
    ///             _engineAudio.Stop();
    ///             Destroy(_engineAudio.gameObject);
    ///         }
    ///     }
    /// </summary>
    public AudioSource PlayAttached(SfxId id, Transform parent, bool loop = false)
    {
        if (parent == null)
        {
            return null;
        }

        if (!_lookup.TryGetValue(id, out SfxDefinition sfx))
        {
            return null;
        }

        if (sfx.Clip == null)
        {
            return null;
        }

        // Create a child GameObject with its own 3D AudioSource
        GameObject go = new GameObject($"SFX_{id}");
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = sfx.Clip;
        src.volume = sfx.Volume;

        // 3D / positional setup
        src.spatialBlend = 1f;      // 1 = fully 3D
        src.minDistance = 1f;
        src.maxDistance = 25f;      // tweak to taste or expose in Inspector if needed
        src.rolloffMode = AudioRolloffMode.Linear;

        src.loop = loop;
        src.Play();

        // For non-looping sounds, clean up automatically after the clip finishes.
        if (!loop)
        {
            Destroy(go, sfx.Clip.length);
        }

        return src;
    }
}
