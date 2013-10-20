using System;
using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    public class GroupBuilder : RuleElementBuilder, IRuleElementBuilder<GroupElement>
    {
        private GroupType _groupType = Rule.GroupType.And;
        private readonly List<IRuleElementBuilder<RuleElement>> _nestedBuilders = new List<IRuleElementBuilder<RuleElement>>(); 

        internal GroupBuilder(SymbolTable scope = null) : base(scope)
        {
            switch (_groupType)
            {
                case Rule.GroupType.Exists:
                    StartSymbolScope();
                    break;
            }
        }

        public void GroupType(GroupType groupType)
        {
            _groupType = groupType;
        }

        public AggregateBuilder Aggregate(Type type, string name = null)
        {
            var builder = new AggregateBuilder(type, Scope);
            _nestedBuilders.Add(builder);

            var declaration = builder.Declare(name, type);
            if (!declaration.IsLocal) Scope.Add(declaration);

            return builder;
        }

        public PatternBuilder Pattern(Type type, string name = null)
        {
            var builder = new PatternBuilder(Scope);
            _nestedBuilders.Add(builder);

            var declaration = builder.Declare(name, type);
            builder.Declaration = declaration;
            if (!declaration.IsLocal) Scope.Add(declaration);

            return builder;
        }

        public GroupBuilder Group(GroupType groupType)
        {
            var builder = new GroupBuilder(Scope);
            _nestedBuilders.Add(builder);

            builder.GroupType(groupType);

            return builder;
        }

        GroupElement IRuleElementBuilder<GroupElement>.Build()
        {
            Validate();
            var childElements = new List<RuleElement>();
            foreach (var nestedBuilder in _nestedBuilders)
            {
                var childElement = nestedBuilder.Build();
                childElements.Add(childElement);
            }
            var groupElement = new GroupElement(_groupType, childElements);
            return groupElement;
        }

        private void Validate()
        {
            switch (_groupType)
            {
                case Rule.GroupType.And:
                    if (_nestedBuilders.Count < 1)
                    {
                        throw new InvalidOperationException("Group condition AND requires at least one child element");
                    }
                    break;
                case Rule.GroupType.Or:
                    throw new NotSupportedException("Group condition OR is not supported");
                case Rule.GroupType.Not:
                    throw new NotSupportedException("Group condition NOT is not supported");
                case Rule.GroupType.Exists:
                    if (_nestedBuilders.Count != 1)
                    {
                        throw new NotSupportedException("Group condition EXISTS requires exactly one child element");
                    }
                    break;
            }
        }
    }
}