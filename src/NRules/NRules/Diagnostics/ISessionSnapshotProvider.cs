namespace NRules.Diagnostics
{
    /// <summary>
    /// Provides a snapshot of rules session state.
    /// </summary>
    public interface ISessionSnapshotProvider
    {
        /// <summary>
        /// Returns a snapshot of session state for diagnostics.
        /// Session state is a graph representing the structure of the underlying Rete network and location of facts in memory nodes.
        /// </summary>
        /// <returns>Session snapshot.</returns>
        SessionSnapshot GetSnapshot();
    }
}