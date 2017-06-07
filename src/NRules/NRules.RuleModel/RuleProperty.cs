using System.Diagnostics;

namespace NRules.RuleModel
{
    [DebuggerDisplay("{Name,nq} = {Value}")]
    public class RuleProperty
    {
        public RuleProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}
