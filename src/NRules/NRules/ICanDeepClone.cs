namespace NRules;

public interface ICanDeepClone<T>
    where T : ICanDeepClone<T>
{
    T DeepClone();
}
