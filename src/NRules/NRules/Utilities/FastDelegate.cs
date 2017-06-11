namespace NRules.Utilities
{
    internal class FastDelegate<TDelegate> where TDelegate : class
    {
        public TDelegate Delegate { get; }
        public int ArrayArgumentCount { get; }

        internal FastDelegate(TDelegate @delegate, int arrayArgumentCount)
        {
            Delegate = @delegate;
            ArrayArgumentCount = arrayArgumentCount;
        }
    }
}