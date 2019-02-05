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
        private readonly List<IBuilder<RuleLeftElement>> _nestedBuilders = new List<IBuilder<RuleLeftElement>>();
        private GroupType _groupType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBuilder"/>.
        /// </summary>
        public GroupBuilder()
        {
        }

        /// <summary>
        /// Sets type of the group element.
        /// </summary>
        /// <param name="groupType">Group type to set.</param>
        public void GroupType(GroupType groupType)
        {
            _groupType = groupType;
        }

        /// <summary>
        /// Adds a pattern to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Pattern(PatternElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Adds a pattern builder to the group element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void Pattern(PatternBuilder builder)
        {
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern as part of the current group element.
        /// </summary>
        /// <param name="type">Pattern type.</param>
        /// <param name="name">Pattern name (optional).</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type, string name = null)
        {
            var declaration = new Declaration(type, DeclarationName(name));
            return Pattern(declaration);
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern as part of the current group element.
        /// </summary>
        /// <param name="declaration">Pattern declaration.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Declaration declaration)
        {
            var builder = new PatternBuilder(declaration);
            _nestedBuilders.Add(builder);
            return builder;            
        }

        /// <summary>
        /// Adds a nested group to this group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Group(GroupElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Adds a nested group builder to this group element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void Group(GroupBuilder builder)
        {
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a group builder that builds a group as part of the current group element.
        /// </summary>
        /// <param name="groupType">Group type.</param>
        /// <returns>Group builder.</returns>
        public GroupBuilder Group(GroupType groupType)
        {
            var builder = new GroupBuilder();
            builder.GroupType(groupType);
            _nestedBuilders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Adds an existential element to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Exists(ExistsElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Adds an existential element builder to the group element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void Exists(ExistsBuilder builder)
        {
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a builder for an existential element as part of the current group element.
        /// </summary>
        /// <returns>Existential builder.</returns>
        public ExistsBuilder Exists()
        {
            var builder = new ExistsBuilder();
            _nestedBuilders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Adds a negative existential element to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Not(NotElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Adds a negative existential element builder to the group element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void Not(NotBuilder builder)
        {
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a builder for a negative existential element as part of the current group element.
        /// </summary>
        /// <returns>Negative existential builder.</returns>
        public NotBuilder Not()
        {
            var builder = new NotBuilder();
            _nestedBuilders.Add(builder);
            return builder;
        }

        /// <summary>
        /// Adds a forall element to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void ForAll(ForAllElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Adds a forall element builder to the group element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void ForAll(ForAllBuilder builder)
        {
            _nestedBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a builder for a forall element as part of the current group element.
        /// </summary>
        /// <returns>Forall builder.</returns>
        public ForAllBuilder ForAll()
        {
            var builder = new ForAllBuilder();
            _nestedBuilders.Add(builder);
            return builder;
        }

        GroupElement IBuilder<GroupElement>.Build()
        {
            var childElements = new List<RuleLeftElement>();
            foreach (var builder in _nestedBuilders)
            {
                RuleLeftElement childElement = builder.Build();
                childElements.Add(childElement);
            }
            var groupElement = Element.Group(_groupType, childElements);
            return groupElement;
        }
    }
}