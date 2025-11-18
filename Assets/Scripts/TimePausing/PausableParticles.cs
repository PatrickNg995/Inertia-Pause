using System.Collections;
using UnityEngine;

public class PausableParticles : MonoBehaviour, IPausable
{
    [Header("Particle System Reference")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Particle System Settings")]
    [Tooltip("Enable particle effect or not.")]
    [SerializeField] private bool _enableParticles = true;

    [Tooltip("If true, the pre-pause play duration will be randomly chosen between the lower and upper bounds." +
             "If false, the duration will use the lower bound.")]
    [SerializeField] private bool _useRandomPrePausePlayDuration = true;

    [Tooltip("The lower bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationLowerBound = 0.05f;

    [Tooltip("The upper bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationUpperBound = 0.1f;

    private float _prePausePlayDuration;

    public void Awake()
    {
        if (!_enableParticles)
        {
            enabled = false;
            _particleSystem.gameObject.SetActive(false);
        }

        // Determine the pre-pause play duration, if not random use the lower bound.
        if (_useRandomPrePausePlayDuration)
        {
            _prePausePlayDuration = Random.Range(_prePausePlayDurationLowerBound, _prePausePlayDurationUpperBound);
        }
        else
        {
            _prePausePlayDuration = _prePausePlayDurationLowerBound;
        }
    }

    public void Pause()
    {
        StartCoroutine(PlayAndPauseParticleSystem(_prePausePlayDuration));
    }

    public void Unpause()
    {
        _particleSystem.Play();
    }

    public void ResetStateBeforeUnpause()
    {
        StartCoroutine(PlayAndPauseParticleSystem(_prePausePlayDuration));
    }

    private IEnumerator PlayAndPauseParticleSystem(float durationBeforePause)
    {
        // Play the particle system for a duration before pausing.
        _particleSystem.Play();
        yield return new WaitForSeconds(durationBeforePause);
        _particleSystem.Pause(); 
    }
}
