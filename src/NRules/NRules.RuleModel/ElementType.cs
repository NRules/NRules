namespace NRules.RuleModel
{
    /// <summary>
    /// Describes the element types for the elements of a rule definition.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// Action invoked when the rule fires.
        /// </summary>
        Action,

        /// <summary>
        /// Collection of actions associated with the rule.
        /// </summary>
        ActionGroup,

        /// <summary>
        /// Aggregation of facts into synthetic composite facts.
        /// </summary>
        Aggregate,

        /// <summary>
        /// Grouping of the match patterns that matches only when all child patterns match.
        /// </summary>
        And,

        /// <summary>
        /// Evaluates an expression and binds the result to a name.
        /// </summary>
        Binding,

        /// <summary>
        /// External service the rule depends on.
        /// </summary>
        Dependency,

        /// <summary>
        /// Collection of dependencies associated with the rule.
        /// </summary>
        DependencyGroup,

        /// <summary>
        /// Existential quantifier that matches if at least one source fact is present.
        /// </summary>
        Exists,

        /// <summary>
        /// Agenda filter applied to complete fact matches to determine if they should be placed on agenda.
        /// </summary>
        Filter,

        /// <summary>
        /// Collection of agenda filters associated with the rule.
        /// </summary>
        FilterGroup,

        /// <summary>
        /// Universal quantifier that matches if all facts matching its first pattern also match all other patterns defined in the quantifier.
        /// </summary>
        ForAll,

        /// <summary>
        /// Existential quantifier that matches if no source fact are present.
        /// </summary>
        Not,

        /// <summary>
        /// Grouping of the match patterns that matches when any child patterns matches.
        /// </summary>
        Or,

        /// <summary>
        /// A pattern that matches facts.
        /// </summary>
        Pattern,

        /// <summary>
        /// An expression evaluated by the rules engine.
        /// </summary>
        NamedExpression
    }
}