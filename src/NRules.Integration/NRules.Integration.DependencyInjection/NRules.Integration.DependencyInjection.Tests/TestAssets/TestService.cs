namespace NRules.Integration.DependencyInjection.Tests.TestAssets;

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