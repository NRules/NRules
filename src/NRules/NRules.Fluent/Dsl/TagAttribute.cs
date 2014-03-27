using System;

namespace NRules.Fluent.Dsl
{
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