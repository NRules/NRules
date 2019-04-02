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
        /// Current rule match.
        /// </summary>
        IMatch Match { get; }

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
        /// Retrieves keys of facts linked to the current rule activation.
        /// </summary>
        /// <returns>Keys for linked facts.</returns>
        IEnumerable<object> GetLinkedKeys();

        /// <summary>
        /// Retrieves a fact linked to the current rule activation by key.
        /// </summary>
        /// <param name="key">Key for the linked fact.</param>
        /// <returns>Linked fact if it exists, <c>null</c> otherwise.</returns>
        object GetLinked(object key);

        /// <summary>
        /// Inserts a new fact and links it to the current rule activation.
        /// The fact will be automatically retracted if this activation is removed.
        /// </summary>
        /// <param name="key">Key for the linked fact. Must be unique for a given rule.</param>
        /// <param name="fact">Fact to insert.</param>
        void InsertLinked(object key, object fact);

        /// <summary>
        /// Inserts new facts and links them to the current rule activation.
        /// The facts will be automatically retracted if this activation is removed.
        /// </summary>
        /// <param name="keyedFacts">Keyed facts to insert. Keys must be unique for a given rule.</param>
        void InsertAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts);

        /// <summary>
        /// Updates existing fact that's linked to the current rule activation.
        /// </summary>
        /// <param name="key">Key for the linked fact. Must be unique for a given rule.</param>
        /// <param name="fact">Fact to update.</param>
        void UpdateLinked(object key, object fact);

        /// <summary>
        /// Updates existing facts that are linked to the current rule activation.
        /// </summary>
        /// <param name="keyedFacts">Keyed facts to update. Keys must be unique for a given rule.</param>
        void UpdateAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts);

        /// <summary>
        /// Retracts existing fact that's linked to the current rule activation.
        /// </summary>
        /// <remarks>Linked facts are retracted automatically, when activation is deleted, but 
        /// this method can be used in complex scenarios, when linked facts need to be retracted explicitly,
        /// prior to activation getting deleted.
        /// </remarks>
        /// <param name="key">Key for the linked fact. Must be unique for a given rule.</param>
        /// <param name="fact">Fact to retract.</param>
        void RetractLinked(object key, object fact);

        /// <summary>
        /// Retracts existing facts that are linked to the current rule activation.
        /// </summary>
        /// <remarks>Linked facts are retracted automatically, when activation is deleted, but 
        /// this method can be used in complex scenarios, when linked facts need to be retracted explicitly,
        /// prior to activation getting deleted.
        /// </remarks>
        /// <param name="keyedFacts">Keyed facts to retract. Keys must be unique for a given rule.</param>
        void RetractAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts);

        /// <summary>
        /// Resolves a registered service (normally via an IoC container).
        /// </summary>
        /// <param name="serviceType">Type of service to resolve.</param>
        /// <returns>Service instance.</returns>
        object Resolve(Type serviceType);
    }
}