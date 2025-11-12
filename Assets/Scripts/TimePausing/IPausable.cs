public interface IPausable
{
    public void Pause();
    public void Unpause();

    /// <summary>
    /// Reset the object to the state it was in before unpausing.
    /// </summary>
    public void ResetStateBeforeUnpause();
}
