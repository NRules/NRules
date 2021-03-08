namespace NRules.Diagnostics
{
    /// <summary>
    /// Provides the rules schema in a form of a Rete network graph, for diagnostics.
    /// </summary>
    public interface ISessionSchemaProvider
    {
        /// <summary>
        /// Returns the rules schema as a graph representing the structure of the underlying Rete network.
        /// </summary>
        /// <returns>Session schema as a Rete graph.</returns>
        ReteGraph GetSchema();
    }
}