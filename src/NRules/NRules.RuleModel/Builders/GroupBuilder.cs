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
            var builder = new GroupBuilder(Scope, groupType);
            _nestedBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        /// Creates a group builder that builds a group as a part of the current group.
        /// </summary>
        /// <param name="quantifierType">Group type.</param>
        /// <returns>Quantifier builder.</returns>
        public QuantifierBuilder Quantifier(QuantifierType quantifierType)
        {
            var builder = new QuantifierBuilder(Scope, quantifierType);
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
            }
        }
    }
}