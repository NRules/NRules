namespace NRules.Dsl
{
    /// <summary>
    /// Rules engine execution context.
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// Inserts a new fact into the rules engine's memory.
        /// </summary>
        /// <param name="fact">New fact to insert.</param>
        public static void Insert(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }

        /// <summary>
        /// Updates an existing fact in the rules engine's memory.
        /// </summary>
        /// <param name="fact">Existing fact to update.</param>
        public static void Update(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }

        /// <summary>
        /// Removes an existing fact from the rules engine's memory.
        /// </summary>
        /// <param name="fact">Existing fact to remove.</param>
        public static void Retract(object fact)
        {
            //This is a marker method to use in expressions; it does not do anything
        }
    }
}