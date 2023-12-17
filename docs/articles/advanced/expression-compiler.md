# Custom Expression Compiler

In NRules, rules conditions and other expressions are represented as lambda expression trees, which are then compiled to executable delegates by the `RuleCompiler`.
By default, `RuleCompiler` uses the standard .NET `Expression.Compile` method. But it also exposes an extensibility point to customize the expression compilation process. To use a custom expression compiler, implement `IExpressionCompiler` interface and assign its instance to the `RuleCompiler.ExpressionCompiler` property.

Example of using [FastExpressionCompiler](https://github.com/dadhi/FastExpressionCompiler) with NRules:
```c#
using FastExpressionCompiler;

public class FastExpressionCompiler : NRules.Extensibility.IExpressionCompiler
{
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression) where TDelegate : Delegate
    {
        return expression.CompileFast();
    }
}
```

And use the created expression compiler for rules compilation:
```c#
var repository = new RuleRepository();
//Load rules

var compiler = new RuleCompiler();
compiler.ExpressionCompiler = new FastExpressionCompiler();
var factory = compiler.Compile(repository.GetRuleSets());
```