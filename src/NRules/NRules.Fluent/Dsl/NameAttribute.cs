using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Adds name to the rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string value)
        {
            Value = value;
        }

        internal string Value { get; private set; }
    }
}