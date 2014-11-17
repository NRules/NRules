using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    /// <summary>
    /// Rule metadata.
    /// </summary>
    public interface IRuleMetadata
    {
        /// <summary>
        /// Rule's .NET type.
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

        /// <summary>
        /// Tests if the rule is tagged with a given tag.
        /// </summary>
        /// <param name="tag">Tag to test.</param>
        /// <returns><code>true</code> if the rule is tagged, <code>false</code> otherwise.</returns>
        bool IsTagged(string tag);
    }

    internal class RuleMetadata : IRuleMetadata
    {
        public RuleMetadata(Type ruleType)
        {
            RuleType = ruleType;
            Name = GetAttributes<NameAttribute>().Select(a => a.Value).SingleOrDefault() ?? RuleType.FullName;
            Description = GetAttributes<DescriptionAttribute>().Select(a => a.Value).SingleOrDefault() ?? string.Empty;
            Tags = GetAttributes<TagAttribute>().Select(a => a.Value).ToArray();
        }

        public Type RuleType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }

        public bool IsTagged(string tag)
        {
            return Tags.Contains(tag);
        }

        private T[] GetAttributes<T>() where T : Attribute
        {
            return RuleType.GetCustomAttributes(true).OfType<T>().ToArray();
        }       
    }
}