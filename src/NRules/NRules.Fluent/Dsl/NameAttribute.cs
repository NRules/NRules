using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Sets rule's name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NameAttribute : Attribute
    {
        public NameAttribute(string value)
        {
            Value = value;
        }

        internal string Value { get; private set; }
    }
}