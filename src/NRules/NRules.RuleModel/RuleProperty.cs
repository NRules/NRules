using System.Diagnostics;

namespace NRules.RuleModel
{
    [DebuggerDisplay("{Name,nq} = {Value}")]
    public class RuleProperty
    {
        private readonly string _name;
        private readonly object _value;

        public RuleProperty(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
