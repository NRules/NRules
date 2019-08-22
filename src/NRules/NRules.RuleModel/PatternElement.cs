using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that represents a pattern that matches facts.
    /// </summary>
    public class PatternElement : RuleLeftElement
    {
        public const string ConditionName = "Condition";

        internal PatternElement(Declaration declaration, ExpressionCollection expressions, PatternSourceElement source)
        {
            Declaration = declaration;
            ValueType = declaration.Type;
            Expressions = expressions;
            Source = source;

            AddExport(declaration);
            AddImports(expressions);
            AddImports(source);
        }

        /// <summary>
        /// Declaration that references the pattern.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Optional pattern source element.
        /// </summary>
        public PatternSourceElement Source { get; }

        /// <summary>
        /// Type of the values that the pattern matches.
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// Expressions used by the pattern to match elements.
        /// </summary>
        public ExpressionCollection Expressions { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitPattern(context, this);
        }
    }
}