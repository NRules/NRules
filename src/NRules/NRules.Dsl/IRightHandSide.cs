using System;
using System.Linq.Expressions;

namespace NRules.Dsl
{
    public interface IRightHandSide
    {
        IRightHandSide Do(Expression<Action> action);
    }
}