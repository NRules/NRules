namespace NRules.Samples.MissManners.Domain
{
    public class Count
    {
        public Count(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public void Increment()
        {
            Value++;
        }

        public override string ToString()
        {
            return $"[Count={Value}]";
        }
    }
}