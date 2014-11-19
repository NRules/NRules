using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Adds description to the rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string value)
        {
            Value = value;
        }

        internal string Value { get; private set; }
    }
}