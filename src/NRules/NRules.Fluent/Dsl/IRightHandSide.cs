using System;

namespace NRules.Fluent.Dsl
{
    public interface IRightHandSide
    {
        IRightHandSide Do(Action<IActionContext> action);
    }
}