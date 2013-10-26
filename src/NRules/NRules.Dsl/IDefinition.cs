namespace NRules.Dsl
{
    /// <summary>
    /// Rule definition expression builder.
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// Returns expression builder for rule's left hand side (conditions).
        /// </summary>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide When();

        /// <summary>
        /// Returns expression builder for rule's right hand side (actions).
        /// </summary>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSide Then();
    }
}