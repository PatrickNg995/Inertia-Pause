public interface IPausable
{
    public void Pause();

    public void Unpause();

    /// <summary>
    /// Reset the object to the state it was in before unpausing.
    /// </summary>
    public void ResetStateBeforeUnpause();

    /// <summary>
    /// Simulate how the object would have behaved before the initial level pause. Not all objects will need to implement this.
    /// </summary>
    public void SimulatePrePauseBehaviour(float simulationDuration);
}
