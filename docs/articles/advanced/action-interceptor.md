# AOP with Action Interceptor

NRules allows extending rules actions through aspect-oriented programming via an action interceptor. Normally, when a rule fires, NRules simply executes its actions. But if action interceptor is provided to NRules then it instead gives control to the interceptor, which can add pre- and post-processing to the actions or can even choose to not execute actions at all.

When action interceptor receives control, it has access to the firing rule's definition, facts matched by the rule and has ability to invoke the actions. To create an action interceptor, create a class that implements [IActionInterceptor](xref:NRules.Extensibility.IActionInterceptor) interface and assign its instance to the [ISessionFactory.ActionInterceptor](xref:NRules.ISessionFactory.ActionInterceptor) or to the [ISession.ActionInterceptor](xref:NRules.ISession.ActionInterceptor) property.

Example of an action interceptor that prints out the name of the firing rule and values of the matched facts, as well as adds error handling:
```c#
public class ActionInterceptor : IActionInterceptor
{
    public void Intercept(IContext context, IReadOnlyCollection<IActionInvocation> actions)
    {
        Console.WriteLine($"Firing rule. Name={context.Rule.Name}");
        Console.WriteLine($"Matched facts. Facts={string.Join(",", context.Match.Facts.Select(x => x.Value))}");

        try
        {
            foreach (var action in actions)
            {
                action.Invoke();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Rule execution failed. Name={context.Rule.Name} Exception={e.Message}");
        }
    }
}
```

Then use the created action interceptor with the rules session:
```c#
var session = factory.CreateSession();
session.ActionInterceptor = new ActionInterceptor();
```
