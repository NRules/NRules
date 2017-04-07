using System;
using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rules engine execution context.
    /// Can be used by rules to interact with the rules engine, i.e. insert, update, retract facts.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Current rule definition.
        /// </summary>
        IRuleDefinition Rule { get; }

        /// <summary>
        /// Retrieves matched facts.
        /// </summary>
        IEnumerable<IFactMatch> Facts { get; }

        /// <summary>
        /// Halts rules execution. The engine continues execution of the current rule and exits the execution cycle.
        /// </summary>
        void Halt();

        /// <summary>
        /// Inserts a new fact to the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        /// <exception cref="ArgumentException">If fact already exists in working memory.</exception>
        void Insert(object fact);
        
        /// <summary>
        /// Inserts new facts to the rules engine memory.
        /// </summary>
        /// <param name="facts">Facts to add.</param>
        /// <exception cref="ArgumentException">If any fact already exists in working memory.</exception>
        void InsertAll(IEnumerable<object> facts);

        /// <summary>
        /// Inserts a fact to the rules engine memory if the fact does not exist.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        /// <returns>Whether the fact was inserted or not.</returns>
        bool TryInsert(object fact);

        /// <summary>
        /// Updates existing fact in the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Update(object fact);

        /// <summary>
        /// Updates existing facts in the rules engine memory.
        /// </summary>
        /// <param name="facts">Facts to update.</param>
        /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
        void UpdateAll(IEnumerable<object> facts);

        /// <summary>
        /// Updates a fact in the rules engine memory if the fact exists.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        /// <returns>Whether the fact was updated or not.</returns>
        bool TryUpdate(object fact);

        /// <summary>
        /// Removes existing fact from the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to remove.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Retract(object fact);

        /// <summary>
        /// Removes existing facts from the rules engine memory.
        /// </summary>
        /// <param name="facts">Facts to remove.</param>
        /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
        void RetractAll(IEnumerable<object> facts);

        /// <summary>
        /// Removes a fact from the rules engine memory if the fact exists.
        /// </summary>
        /// <param name="fact">Fact to remove.</param>
        /// <returns>Whether the fact was retracted or not.</returns>
        bool TryRetract(object fact);

        /// <summary>
        /// Resolves a registered service (normally via an IoC container).
        /// </summary>
        TService Resolve<TService>();

        /// <summary>
        /// Resolves a registered service (normally via an IoC container).
        /// </summary>
        object Resolve(Type serviceType);
    }
}