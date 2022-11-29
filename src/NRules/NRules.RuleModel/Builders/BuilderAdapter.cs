namespace NRules.RuleModel.Builders;

internal static class BuilderAdapter
{
    public static IBuilder<T> Create<T>(T element)
        where T : RuleElement
    {
        return new Adapter<T>(element);
    }

    private class Adapter<T> : RuleElementBuilder, IBuilder<T>
        where T : RuleElement
    {
        private readonly T _element;

        public Adapter(T element)
        {
            _element = element;
        }

        public T Build()
        {
            return _element;
        }
    }
}
