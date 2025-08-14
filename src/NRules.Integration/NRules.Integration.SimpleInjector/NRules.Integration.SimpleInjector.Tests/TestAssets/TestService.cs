namespace NRules.Integration.SimpleInjector.Tests.TestAssets;

public interface ITestService
{
    public string? Status { get; }
    void DoIt();
}

public class TestService : ITestService
{
    public string? Status { get; set; }

    public void DoIt()
    {
        Status = "It's done";
    }
}