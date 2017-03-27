using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a fact matched by a rule.
    /// </summary>
    public interface IFactMatch
    {
        /// <summary>
        /// Variable declaration that corresponds to the fact.
        /// </summary>
        Declaration Declaration { get; }

        /// <summary>
        /// Fact runtime type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Fact value.
        /// </summary>
        object Value { get; }
    }
}
