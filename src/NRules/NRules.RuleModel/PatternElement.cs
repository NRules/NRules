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
        private readonly Declaration _declaration;
        private readonly PatternSourceElement _source;
        private readonly Type _valueType;

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions)
        {
            _declaration = declaration;
            _valueType = declaration.Type;
            _conditions = new List<ConditionElement>(conditions);
        }

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions, PatternSourceElement source)
            : this(declaration, conditions)
        {
            _source = source;
        }

        /// <summary>
        /// Declaration that references the pattern.
        /// </summary>
        public Declaration Declaration
        {
            get { return _declaration; }
        }

        /// <summary>
        /// Optional pattern source element.
        /// </summary>
        public PatternSourceElement Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Type of the values that the pattern matches.
        /// </summary>
        public Type ValueType
        {
            get { return _valueType; }
        }

        /// <summary>
        /// List of conditions the pattern checks.
        /// </summary>
        public IEnumerable<ConditionElement> Conditions
        {
            get { return _conditions; }
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitPattern(context, this);
        }
    }
}