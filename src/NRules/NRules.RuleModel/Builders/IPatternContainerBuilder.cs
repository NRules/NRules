using System;

namespace NRules.RuleModel.Builders
{
    public interface IPatternContainerBuilder
    {
        /// <summary>
        /// Creates a pattern builder that builds the source of the element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <param name="name">Pattern name (optional).</param>
        /// <returns>Pattern builder.</returns>
        PatternBuilder Pattern(Type type, string name = null);

        /// <summary>
        /// Creates a pattern builder that builds the source of the element.
        /// </summary>
        /// <param name="declaration">Pattern declaration.</param>
        /// <returns>Pattern builder.</returns>
        PatternBuilder Pattern(Declaration declaration);
    }
}