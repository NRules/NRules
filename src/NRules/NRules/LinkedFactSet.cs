using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules
{
    /// <summary>
    /// Action taken on the linked fact.
    /// </summary>
    public enum LinkedFactAction
    {
        /// <summary>
        /// Linked fact is inserted into the session.
        /// </summary>
        Insert,

        /// <summary>
        /// Linked fact is updated in the session.
        /// </summary>
        Update,

        /// <summary>
        /// Linked fact is retracted from the session.
        /// </summary>
        Retract,
    }

    /// <summary>
    /// Collection of linked facts propagated as a set.
    /// </summary>
    public interface ILinkedFactSet
    {
        /// <summary>
        /// Action taken on the linked fact.
        /// </summary>
        LinkedFactAction Action { get; }

        /// <summary>
        /// Linked facts in the set.
        /// </summary>
        IEnumerable<IFact> Facts { get; }

        /// <summary>
        /// Number of linked facts in the set.
        /// </summary>
        int FactCount { get; }
    }

    internal struct LinkedFactSet : ILinkedFactSet
    {
        public LinkedFactAction Action { get; }
        IEnumerable<IFact> ILinkedFactSet.Facts => Facts;
        int ILinkedFactSet.FactCount => Facts.Count;

        public List<Fact> Facts { get; }

        public LinkedFactSet(LinkedFactAction action)
        {
            Action = action;
            Facts = new List<Fact>();
        }
    }
}