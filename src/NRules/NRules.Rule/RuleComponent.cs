using System;
using System.Linq;
using System.Text;

namespace NRules.Rule
{
    public enum RuleComponentTypes
    {
        Conditional = 0,
        Aggregate = 1,
        Existential = 2,
    }

    public abstract class RuleComponent
    {
        protected RuleComponent(RuleComponentTypes componentType)
        {
            ComponentType = componentType;
        }

        public RuleComponentTypes ComponentType { get; private set; }
        public Type ObjectType { get; internal set; }
        public RuleComponent Source { get; internal set; }
    }
}
