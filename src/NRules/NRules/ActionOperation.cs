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
}