namespace NRules.Rete
{
    internal class TupleMask
    {
        private readonly int[] _mask;

        public TupleMask(int[] mask)
        {
            _mask = mask;
        }

        public void SetAtIndex(ref object[] target, int index, object value)
        {
            SetAtIndex(ref target, index, 0, value);
        }

        public void SetAtIndex(ref object[] target, int index, int offset, object value)
        {
            int position = _mask[index];
            if (position != -1)
            {
                target[position + offset] = value;
            }
        }
    }
}
