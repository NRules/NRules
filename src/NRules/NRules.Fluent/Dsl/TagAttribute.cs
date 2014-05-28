using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Tags the rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TagAttribute : Attribute
    {
        public TagAttribute(string value)
        {
            Value = value;
        }

        internal string Value { get; private set; }
    }
}