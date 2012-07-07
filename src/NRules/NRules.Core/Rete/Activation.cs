namespace NRules.Core.Rete
{
    internal class Activation
    {
        public Activation(string ruleHandle, Tuple tuple)
        {
            RuleHandle = ruleHandle;
            Tuple = tuple;
        }

        public string RuleHandle { get; private set; }
        public Tuple Tuple { get; private set; }
    }
}