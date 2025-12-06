public interface IPausable
{
    public const float SIMULATED_PAUSE_TIME = 1f;

    public void Pause();

    public void Unpause();

    /// <summary>
    /// Reset the object to the state it was in before unpausing.
    /// </summary>
    public void ResetStateBeforeUnpause();

    /// <summary>
    /// Simulate how the object would have behaved before pausing. Not all objects will need to implement this.
    /// </summary>
    public void SimulatePrePauseBehaviour();
}
