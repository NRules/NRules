namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a fact matched by a rule.
    /// </summary>
    public interface IFactMatch : IFact
    {
        /// <summary>
        /// Variable declaration that corresponds to the fact.
        /// </summary>
        Declaration Declaration { get; }
    }
}
