namespace NRules.Rule
{
    /// <summary>
    /// Rules engine execution context.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Halts rules execution. The engine continues execution of the current rule and exists the execution cycle.
        /// </summary>
        void Halt();

        /// <summary>
        /// Inserts a new fact into the rules engine's memory.
        /// </summary>
        /// <param name="fact">New fact to insert.</param>
        void Insert(object fact);

        /// <summary>
        /// Updates an existing fact in the rules engine's memory.
        /// </summary>
        /// <param name="fact">Existing fact to update.</param>
        void Update(object fact);

        /// <summary>
        /// Removes an existing fact from the rules engine's memory.
        /// </summary>
        /// <param name="fact">Existing fact to remove.</param>
        void Retract(object fact);
    }
}