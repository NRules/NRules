using NRules.Fluent;
using NRules.Integration.SimpleInjector.Tests.TestAssets;

namespace NRules.Integration.SimpleInjector.Tests;

public class MyCompiledRuleSets02 : CompiledRuleSets<MyCompiledRuleSets02>
{
    public override void DefineSpecification(RuleBulkLoadSpecBuilder builder)
    {
        builder
            .Specify("SubSet01",
                scan => scan.AssemblyOf<TestFact1>().Where(rm => rm.Namespace!.EndsWith("SubNamespace01")))
            .Specify("SubSet02",
                scan => scan.AssemblyOf<TestFact1>().Where(rm => rm.Namespace!.EndsWith("SubNamespace02")));
    }
}