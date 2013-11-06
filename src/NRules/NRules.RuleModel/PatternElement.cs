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

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions)
        {
            Declaration = declaration;
            ValueType = declaration.Type;
            _conditions = new List<ConditionElement>(conditions);
        }

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions, PatternSourceElement source)
            : this(declaration, conditions)
        {
            Source = source;
        }

        /// <summary>
        /// Declaration that references the pattern.
        /// </summary>
        public Declaration Declaration { get; private set; }

        /// <summary>
        /// Optional pattern source element.
        /// </summary>
        public PatternSourceElement Source { get; private set; }

        /// <summary>
        /// Type of the values that the pattern matches.
        /// </summary>
        public Type ValueType { get; private set; }

        /// <summary>
        /// List of conditions the pattern checks.
        /// </summary>
        public IEnumerable<ConditionElement> Conditions
        {
            get { return _conditions; }
        }

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitPattern(this);
        }
    }
}