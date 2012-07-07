using System;

namespace NRules.Dsl
{
    public interface IRightHandSide
    {
        IRightHandSide Do(Action<IActionContext> action);
    }
}