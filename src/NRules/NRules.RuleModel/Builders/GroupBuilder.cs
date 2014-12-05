using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Type of group element.
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// Logical AND.
        /// </summary>
        And = 0,

        /// <summary>
        /// Logical OR.
        /// </summary>
        Or = 1,

        /// <summary>
        /// Logical NOT.
        /// </summary>
        Not = 2,

        /// <summary>
        /// Existential quantifier.
        /// </summary>
        Exists = 3,

        /// <summary>
        /// Universal quantifier.
        /// </summary>
        ForAll = 4,
    }

    /// <summary>
    /// Builder to compose a group element.
    /// </summary>
    public class GroupBuilder : RuleElementBuilder, IBuilder<GroupElement>
    {
        private readonly GroupType _groupType;
        private readonly List<IBuilder<RuleLeftElement>> _nestedBuilders = new List<IBuilder<RuleLeftElement>>();

        internal GroupBuilder(SymbolTable scope, GroupType groupType) : base(scope)
        {
            _groupType = groupType;
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern as part of the current group.
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
                case GroupType.Not:
                case GroupType.ForAll:
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
            var childElements = new List<RuleLeftElement>();
            foreach (var nestedBuilder in _nestedBuilders)
            {
                RuleLeftElement childElement = nestedBuilder.Build();
                childElements.Add(childElement);
            }
            GroupElement groupElement;
            switch (_groupType)
            {
                case GroupType.And:
                    groupElement = new AndElement(childElements);
                    break;
                case GroupType.Or:
                    groupElement = new OrElement(childElements);
                    break;
                case GroupType.Not:
                    groupElement = new NotElement(childElements);
                    break;
                case GroupType.Exists:
                    groupElement = new ExistsElement(childElements);
                    break;
                case GroupType.ForAll:
                    groupElement = new ForAllElement(childElements);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unrecognized group type. GroupType={0}", _groupType));
            }
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
                    if (_nestedBuilders.Count != 1)
                    {
                        throw new InvalidOperationException("Group condition NOT requires exactly one child element");
                    }
                    break;
                case GroupType.Exists:
                    if (_nestedBuilders.Count != 1)
                    {
                        throw new InvalidOperationException("Group condition EXISTS requires exactly one child element");
                    }
                    break;
                case GroupType.ForAll:
                    if (_nestedBuilders.Count != 1)
                    {
                        throw new InvalidOperationException("Group condition FORALL requires exactly one child element");
                    }
                    break;
            }
        }
    }
}