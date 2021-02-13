using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Metadata associated with a rule defined using internal DSL.
    /// </summary>
    public interface IRuleMetadata
    {
        /// <summary>
        /// Rule's CLR type.
        /// </summary>
        Type RuleType { get; }

        /// <summary>
        /// Rule's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Rule's description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        string[] Tags { get; }
    }

    /// <summary>
    /// Metadata associated with a rule defined using internal DSL.
    /// </summary>
    public class RuleMetadata : IRuleMetadata
    {
        public RuleMetadata(Type ruleType)
        {
            RuleType = ruleType;
            Name = GetAttributes<NameAttribute>().Select(a => a.Value).SingleOrDefault() ?? RuleType.FullName;
            Description = GetAttributes<DescriptionAttribute>().Select(a => a.Value).SingleOrDefault() ?? string.Empty;
            Tags = GetAttributes<TagAttribute>().Select(a => a.Value).ToArray();
            Priority = GetAttributes<PriorityAttribute>().SingleNullable(a => a.Value);
            Repeatability = GetAttributes<RepeatabilityAttribute>().SingleNullable(a => a.Value);
        }

        /// <summary>
        /// Rule's CLR type.
        /// </summary>
        public Type RuleType { get; }

        /// <summary>
        /// Rule's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Rule's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Tags applied to the rule.
        /// </summary>
        public string[] Tags { get; }

        /// <summary>
        /// Rule's priority.
        /// </summary>
        public int? Priority { get; }

        /// <summary>
        /// Rule's repeatability.
        /// </summary>
        public RuleRepeatability? Repeatability { get; }

        private T[] GetAttributes<T>() where T : Attribute
        {
            return RuleType.GetCustomAttributes(true).OfType<T>().ToArray();
        }
    }
}