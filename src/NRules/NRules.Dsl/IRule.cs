namespace NRules.Dsl
{
    /// <summary>
    /// Base interface for inline rule definitions.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Method called by the rules engine to get rule's definition.
        /// </summary>
        /// <param name="definition">Rule definition expression builder.</param>
        void Define(IDefinition definition);
    }
}