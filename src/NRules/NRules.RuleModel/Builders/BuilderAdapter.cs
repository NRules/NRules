namespace NRules.RuleModel.Builders
{
    internal static class BuilderAdapter
    {
        public static BuilderAdapter<T> Create<T>(T element)
            where T : RuleElement
        {
            return new BuilderAdapter<T>(element);
        }
    }
    
    internal class BuilderAdapter<T> : RuleElementBuilder, IBuilder<T>
        where T : RuleElement
    {
        private readonly T _element;

        public BuilderAdapter(T element)
        {
            _element = element;
        }

        public T Build()
        {
            return _element;
        }
    }
}
