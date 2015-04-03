using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    internal class ActionContext : IContext
    {
        public ActionContext(IRuleDefinition rule)
        {
            Rule = rule;
            IsHalted = false;
            Operations = new Queue<ActionOperation>();
        }

        public IRuleDefinition Rule { get; private set; }
        public bool IsHalted { get; private set; }
        public Queue<ActionOperation> Operations { get; set; } 

        public void Insert(object fact)
        {
            Operations.Enqueue(new ActionOperation(fact, ActionOperationType.Insert));
        }

        public void Update(object fact)
        {
            Operations.Enqueue(new ActionOperation(fact, ActionOperationType.Update));
        }

        public void Retract(object fact)
        {
            Operations.Enqueue(new ActionOperation(fact, ActionOperationType.Retract));
        }

        public void Halt()
        {
            IsHalted = true;
        }
    }
}