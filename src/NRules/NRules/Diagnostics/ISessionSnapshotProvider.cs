namespace NRules.Diagnostics
{
    /// <summary>
    /// Provider of a snapshot of session state.
    /// </summary>
    public interface ISessionSnapshotProvider
    {
        /// <summary>
        /// Returns a snapshot of session state for diagnostics.
        /// </summary>
        /// <returns>Session snapshot.</returns>
        SessionSnapshot GetSnapshot();
    }
}