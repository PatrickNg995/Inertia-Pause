using System.Collections;
using UnityEngine;

public class PausableParticles : MonoBehaviour, IPausable
{
    [Header("Particle System Reference")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Particle System Settings")]
    [Tooltip("Enable particle effect or not.")]
    [SerializeField] private bool _isParticlesEnabled = true;

    [Tooltip("Enable prepause simulation or not.")]
    [SerializeField] private bool _isPrepauseSimulationEnabled = true;

    [Tooltip("If true, the pre-pause play duration will be randomly chosen between the lower and upper bounds." +
             "If false, the duration will use the lower bound.")]
    [SerializeField] private bool _useRandomPrePausePlayDuration = true;

    [Tooltip("The lower bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationLowerBound = 0.05f;

    [Tooltip("The upper bound for the pre-pause play duration (in seconds).")]
    [SerializeField] private float _prePausePlayDurationUpperBound = 0.1f;

    // The duration the particle system simulates playing before pausing.
    private float _prePausePlayDuration;

    public void Awake()
    {
        // Disable the script and particle system if particles are not enabled. This is mostly to make it easier to disable
        // effects like muzzle flashes without needing to dig into the prefab every timne.
        if (!_isParticlesEnabled)
        {
            enabled = false;
            _particleSystem.gameObject.SetActive(false);
        }

        // Determine the pre-pause play duration. If not set to random, use the lower bound.
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
        _particleSystem.Simulate(_prePausePlayDuration, true, true);
    }

    public void Unpause()
    {
        _particleSystem.Play();
    }

    public void ResetStateBeforeUnpause()
    {
        _particleSystem.Simulate(_prePausePlayDuration, true, true);

    }

    public void SimulatePrePauseBehaviour(float simulationDuration)
    {
        if (_isPrepauseSimulationEnabled)
        {
            StartCoroutine(PlayThenPauseParticles(simulationDuration));
        }
    }

    private IEnumerator PlayThenPauseParticles(float simulationDuration)
    {
        // Start particle system from the beginning.
        _particleSystem.Stop();
        _particleSystem.Clear();
        _particleSystem.Play();

        // Wait for the specified duration.
        yield return new WaitForSeconds(simulationDuration);

        // Pause the particle system to set it to the correct state.
        Pause();
    }
}
