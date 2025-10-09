using UnityEngine;

// Get Particle System, then Pause & Play them depending
// on game state
public class PausableParticles : MonoBehaviour, IPausable
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        GetComponent<IPausable>().AddToTimePause(this);
    }
    public void Pause()
    {
        ps.Pause();
    }

    public void Unpause()
    {
        ps.Play();
    }
}
