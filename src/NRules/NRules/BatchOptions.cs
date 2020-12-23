namespace NRules
{
    /// <summary>
    /// Defines how batch insert/update/retract of facts behaves.
    /// Any fact that already exists in the session during insert is considered failed.
    /// Similarly, a fact that does not exist in the session during update or retract is also considered failed.
    /// By default, any failed fact in a batch operation fails the whole operation, and no facts are propagated.
    /// This behavior can be changed using <c>BatchOptions</c>.
    /// </summary>
    public enum BatchOptions
    {
        /// <summary>
        /// Default behavior of batch operations, where the operation is either applied to all facts in the batch or none of them.
        /// </summary>
        AllOrNothing = 0,

        /// <summary>
        /// Changes the behavior of fact propagation, where failed facts are skipped, and the rest are propagated.
        /// </summary>
        SkipFailed = 1,
    }
}