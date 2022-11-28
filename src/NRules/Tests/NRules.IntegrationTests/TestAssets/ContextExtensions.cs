using NRules.RuleModel;

namespace NRules.IntegrationTests.TestAssets;

public static class ContextExtensions
{
    public static void NoOp(this IContext _)
    {
    }

    public static void NoOp<TArg>(this IContext _, TArg arg1)
    {
    }

    public static void NoOp<TArg1, TArg2>(this IContext _, TArg1 arg1, TArg2 arg2)
    {
    }

    public static void NoOp<TArg1, TArg2, TArg3>(this IContext _, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
    }
}
