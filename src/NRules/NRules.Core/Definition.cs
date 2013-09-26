using System;
using System.Linq.Expressions;
using NRules.Dsl;
using NRules.Rule;

namespace NRules.Core
{
    internal class Definition : IDefinition, ILeftHandSide, IRightHandSide
    {
        private readonly IRuleBuilder _builder;

        public Definition(IRuleBuilder builder, RuleMetadata metadata)
        {
            _builder = builder;

            if (metadata.Priority.HasValue)
                _builder.Priority(metadata.Priority.Value);
        }

        public ILeftHandSide If<T>(Expression<Func<T, bool>> condition)
        {
            _builder.Condition(condition);
            return this;
        }

        public ILeftHandSide If<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            _builder.Condition(condition);
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<T, bool>> itemCondition)
        {
            _builder.Collect(itemCondition);
            return this;
        }

        public ILeftHandSide Exists<T>(Expression<Func<T, bool>> condition)
        {
            _builder.Exists(condition);
            return this;
        }

        public IRightHandSide Do(Expression<Action<IActionContext>> action)
        {
            _builder.Action(action);
            return this;
        }

        public ILeftHandSide When()
        {
            return this;
        }

        public IRightHandSide Then()
        {
            return this;
        }
    }
}