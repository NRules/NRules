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
    public class GroupBuilder : RuleElementBuilder, IBuilder<GroupElement>, IPatternContainerBuilder
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
            Declaration declaration = Scope.Declare(type, name);
            return Pattern(declaration);
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern as part of the current group.
        /// </summary>
        /// <param name="declaration">Pattern declaration.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Declaration declaration)
        {
            var builder = new PatternBuilder(Scope, declaration);
            _nestedBuilders.Add(builder);
            return builder;            
        }

        /// <summary>
        /// Creates a group builder that builds a group as part of the current group.
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
        /// Creates a builder for an existential element as part of the current group.
        /// </summary>
        /// <returns>Existential builder.</returns>
        public ExistsBuilder Exists()
        {
            var builder = new ExistsBuilder(Scope);
            _nestedBuilders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Creates a builder for a negative existential element as part of the current group.
        /// </summary>
        /// <returns>Negative existential builder.</returns>
        public NotBuilder Not()
        {
            var builder = new NotBuilder(Scope);
            _nestedBuilders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Creates a builder for a forall element as part of the current group.
        /// </summary>
        /// <returns>Forall builder.</returns>
        public ForAllBuilder ForAll()
        {
            var builder = new ForAllBuilder(Scope);
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
                    groupElement = new AndElement(Scope.VisibleDeclarations, childElements);
                    break;
                case GroupType.Or:
                    groupElement = new OrElement(Scope.VisibleDeclarations, childElements);
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
                        throw new InvalidOperationException("Group element AND requires at least one child element");
                    }
                    break;
                case GroupType.Or:
                    if (_nestedBuilders.Count < 1)
                    {
                        throw new InvalidOperationException("Group element OR requires at least one child element");
                    }
                    break;
            }
        }
    }
}