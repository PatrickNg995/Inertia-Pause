using UnityEngine;

public class PausableParticles : MonoBehaviour, IPausable
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    public void Pause()
    {
        ps.Stop();
    }

    public void Unpause()
    {
        ps.Play();
    }
}
