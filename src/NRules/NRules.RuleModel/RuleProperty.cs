using System.Diagnostics;

namespace NRules.RuleModel
{
    /// <summary>
    /// Arbitrary value associated with a rule.
    /// </summary>
    [DebuggerDisplay("{Name,nq} = {Value}")]
    public class RuleProperty
    {
        /// <summary>
        /// Creates a new rule property.
        /// </summary>
        /// <param name="name">Rule property name.</param>
        /// <param name="value">Rule property value.</param>
        public RuleProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Rule property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Rule property value.
        /// </summary>
        public object Value { get; }
    }
}
