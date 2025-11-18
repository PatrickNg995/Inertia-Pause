using System.Collections;
using UnityEngine;

public class PausableParticles : MonoBehaviour, IPausable
{
    [Header("Particle System Settings")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Tooltip("If true, the pre-pause play duration will be randomly chosen between the lower and upper bounds." +
             "If false, the duration will use the lower bound.")]
    [SerializeField] private bool _useRandomPrePausePlayDuration = true;

    [Tooltip("The lower bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationLowerBound = 0.1f;

    [Tooltip("The upper bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationUpperBound = 0.5f;

    private float _prePausePlayDuration;

    public void Awake()
    {
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
