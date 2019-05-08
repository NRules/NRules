using System;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Adds a tag to rule's metadata.
    /// A rule class can have multiple tag attributes, and also inherits tag attributes from its parent classes.
    /// Tags can be used to filter rules when loading them through fluent load specification.
    /// </summary>
    /// <remarks>
    /// A custom tag attribute class could be inherited from the <see cref="TagAttribute"/> to provide strongly-typed version of a tag.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TagAttribute : Attribute
    {
        public TagAttribute(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }
}