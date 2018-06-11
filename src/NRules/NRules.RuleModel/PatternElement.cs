using System;
using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that represents a pattern that matches facts.
    /// </summary>
    public class PatternElement : RuleLeftElement
    {
        private readonly List<ConditionElement> _conditions;

        internal PatternElement(Declaration declaration, IEnumerable<Declaration> declarations, IEnumerable<ConditionElement> conditions)
            : base(declarations)
        {
            Declaration = declaration;
            ValueType = declaration.Type;
            _conditions = new List<ConditionElement>(conditions);
        }

        internal PatternElement(Declaration declaration, IEnumerable<Declaration> declarations, IEnumerable<ConditionElement> conditions, RuleLeftElement source)
            : this(declaration, declarations, conditions)
        {
            Source = source;
        }

        /// <summary>
        /// Declaration that references the pattern.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Optional pattern source element.
        /// </summary>
        public RuleLeftElement Source { get; }

        /// <summary>
        /// Type of the values that the pattern matches.
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// List of conditions the pattern checks.
        /// </summary>
        public IEnumerable<ConditionElement> Conditions => _conditions;

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitPattern(context, this);
        }
    }
}