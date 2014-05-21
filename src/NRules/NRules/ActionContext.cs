using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    internal enum ActionOperationType
    {
        Insert,
        Update,
        Retract
    }

    internal class ActionOperation
    {
        public object Fact { get; private set; }
        public ActionOperationType OperationType { get; private set; }

        public ActionOperation(object fact, ActionOperationType operationType)
        {
            Fact = fact;
            OperationType = operationType;
        }
    }

    internal class ActionContext : IContext
    {
        public ActionContext()
        {
            IsHalted = false;
            Operations = new Queue<ActionOperation>();
        }

        public bool IsHalted { get; set; }
        public Queue<ActionOperation> Operations { get; set; } 

        public void Insert(object fact)
        {
            Operations.Enqueue(new ActionOperation(fact, ActionOperationType.Insert));
        }

        public void Update(object fact)
        {
            Operations.Enqueue(new ActionOperation(fact, ActionOperationType.Update));
        }

        public void Update<T>(T fact, Action<T> updateAction)
        {
            updateAction(fact);
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