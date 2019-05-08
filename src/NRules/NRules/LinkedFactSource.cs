using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    /// <summary>
    /// Fact source for linked facts.
    /// </summary>
    public interface ILinkedFactSource : IFactSource
    {
        /// <summary>
        /// Rule that generated the linked fact.
        /// </summary>
        IRuleDefinition Rule { get; }
    }

    internal class LinkedFactSource : ILinkedFactSource
    {
        private readonly Activation _activation;

        public LinkedFactSource(Activation activation)
        {
            _activation = activation;
        }

        public FactSourceType SourceType => FactSourceType.Linked;
        public IEnumerable<IFact> Facts => _activation.Facts;
        public IRuleDefinition Rule => _activation.Rule;
    }
}
