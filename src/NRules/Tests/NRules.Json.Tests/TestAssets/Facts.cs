namespace NRules.Json.Tests.TestAssets;

public interface IFactType1
{
    public bool BooleanProperty { get; }
    public string? StringProperty { get; }
    public string? GroupKey { get; }
}

public class FactType1 : IFactType1
{
    public bool BooleanProperty { get; set; }
    public string? StringProperty { get; set; }
    public string? GroupKey { get; set; }
}

public class FactType2
{
    public FactType1? JoinProperty { get; set; }
}

public class FactType3
{
    public FactType3(string value) => StringProperty = value;

    public string StringProperty { get; }
}

public class Container<T>
{
}

public class Outer
{
    public class Inner { }
}