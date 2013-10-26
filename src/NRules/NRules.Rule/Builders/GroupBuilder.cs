using System;
using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    /// <summary>
    /// Builder to compose a group element.
    /// </summary>
    public class GroupBuilder : RuleElementBuilder, IBuilder<GroupElement>
    {
        private readonly GroupType _groupType;
        private readonly List<IBuilder<RuleElement>> _nestedBuilders = new List<IBuilder<RuleElement>>();

        internal GroupBuilder(SymbolTable scope, GroupType groupType) : base(scope)
        {
            _groupType = groupType;
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern as a part of the current group.
        /// </summary>
        /// <param name="type">Pattern type.</param>
        /// <param name="name">Pattern name (optional).</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type, string name = null)
        {
            SymbolTable scope = Scope.New();
            Declaration declaration = scope.Declare(type, name);

            var builder = new PatternBuilder(scope, declaration);
            _nestedBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        /// Creates a group builder that builds a group as a part of the current group.
        /// </summary>
        /// <param name="groupType">Group type.</param>
        /// <returns>Group builder.</returns>
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