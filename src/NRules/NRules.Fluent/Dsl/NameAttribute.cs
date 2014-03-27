using System;

namespace NRules.Fluent.Dsl
{
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