using System;
using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    public class GroupBuilder : RuleElementBuilder, IBuilder<GroupElement>
    {
        private readonly GroupType _groupType;
        private readonly List<IBuilder<RuleElement>> _nestedBuilders = new List<IBuilder<RuleElement>>();

        internal GroupBuilder(SymbolTable scope, GroupType groupType) : base(scope)
        {
            _groupType = groupType;
        }

        public AggregateBuilder Aggregate(Type type, string name = null)
        {
            SymbolTable scope = Scope.New();
            scope.Declare(name, type);

            var builder = new AggregateBuilder(type, scope);
            _nestedBuilders.Add(builder);

            return builder;
        }

        public PatternBuilder Pattern(Type type, string name = null)
        {
            SymbolTable scope = Scope.New();
            Declaration declaration = scope.Declare(name, type);

            var builder = new PatternBuilder(scope, declaration);
            _nestedBuilders.Add(builder);

            return builder;
        }

        public GroupBuilder Group(GroupType groupType)
        {
            SymbolTable scope = Scope;
            switch (groupType)
            {
                case GroupType.Exists:
                    scope = Scope.New();
                    break;
            }

            var builder = new GroupBuilder(scope, groupType);
            _nestedBuilders.Add(builder);

            return builder;
        }

        GroupElement IBuilder<GroupElement>.Build()
        {
            Validate();
            var childElements = new List<RuleElement>();
            foreach (var nestedBuilder in _nestedBuilders)
            {
                RuleElement childElement = nestedBuilder.Build();
                childElements.Add(childElement);
            }
            var groupElement = new GroupElement(_groupType, childElements);
            return groupElement;
        }

        private void Validate()
        {
            switch (_groupType)
            {
                case GroupType.And:
                    if (_nestedBuilders.Count < 1)
                    {
                        throw new InvalidOperationException("Group condition AND requires at least one child element");
                    }
                    break;
                case GroupType.Or:
                    throw new NotSupportedException("Group condition OR is not supported");
                case GroupType.Not:
                    throw new NotSupportedException("Group condition NOT is not supported");
                case GroupType.Exists:
                    if (_nestedBuilders.Count != 1)
                    {
                        throw new NotSupportedException("Group condition EXISTS requires exactly one child element");
                    }
                    break;
            }
        }
    }
}