﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Factory class for rule elements.
    /// </summary>
    public static class Element
    {
        /// <summary>
        /// Creates a rule definition.
        /// </summary>
        /// <param name="name">Rule's name.</param>
        /// <param name="description">Rule's description.</param>
        /// <param name="priority">Rule's priority.</param>
        /// <param name="leftHandSide">Rule's left-hand side top group element.</param>
        /// <param name="rightHandSide">Rule's right-hand side group element.</param>
        /// <returns>Created rule definition.</returns>
        public static IRuleDefinition RuleDefinition(string name, string description, int priority,
            GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            var tags = Array.Empty<string>();
            var ruleDefinition = RuleDefinition(name, description, priority, tags, leftHandSide, rightHandSide);
            return ruleDefinition;
        }

        /// <summary>
        /// Creates a rule definition.
        /// </summary>
        /// <param name="name">Rule's name.</param>
        /// <param name="description">Rule's description.</param>
        /// <param name="priority">Rule's priority.</param>
        /// <param name="tags">Tags associated with the rule.</param>
        /// <param name="leftHandSide">Rule's left-hand side top group element.</param>
        /// <param name="rightHandSide">Rule's right-hand side group element.</param>
        /// <returns>Created rule definition.</returns>
        public static IRuleDefinition RuleDefinition(string name, string description, int priority,
            IEnumerable<string> tags, GroupElement leftHandSide, ActionGroupElement rightHandSide)
        {
            var ruleProperties = Array.Empty<RuleProperty>();
            var dependencyGroupElement = DependencyGroup();
            var filterGroupElement = FilterGroup();
            var ruleDefinition = RuleDefinition(name, description, priority, RuleRepeatability.Repeatable,
                tags, ruleProperties, dependencyGroupElement, leftHandSide, filterGroupElement, rightHandSide);
            return ruleDefinition;
        }

        /// <summary>
        /// Creates a rule definition.
        /// </summary>
        /// <param name="name">Rule's name.</param>
        /// <param name="description">Rule's description.</param>
        /// <param name="priority">Rule's priority.</param>
        /// <param name="repeatability">Rule's repeatability.</param>
        /// <param name="tags">Tags associated with the rule.</param>
        /// <param name="properties">Properties associated with the rule.</param>
        /// <param name="dependencies">Rule's dependency group element.</param>
        /// <param name="leftHandSide">Rule's left-hand side top group element.</param>
        /// <param name="filters">Rule's filter group element.</param>
        /// <param name="rightHandSide">Rule's right-hand side group element.</param>
        /// <returns>Created rule definition.</returns>
        public static IRuleDefinition RuleDefinition(string name, string description, int priority,
            RuleRepeatability repeatability, IEnumerable<string> tags, IEnumerable<RuleProperty> properties,
            DependencyGroupElement dependencies, GroupElement leftHandSide, FilterGroupElement filters, ActionGroupElement rightHandSide)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Rule name not provided", nameof(name));
            if (tags == null)
                throw new ArgumentNullException(nameof(tags), "Rule tags not provided");
            if (properties == null)
                throw new ArgumentNullException(nameof(properties), "Rule properties not provided");
            if (dependencies == null)
                throw new ArgumentNullException(nameof(dependencies), "Rule dependencies not provided");
            if (leftHandSide == null)
                throw new ArgumentNullException(nameof(leftHandSide), "Rule left-hand side not provided");
            if (filters == null)
                throw new ArgumentNullException(nameof(filters), "Rule filters not provided");
            if (rightHandSide == null)
                throw new ArgumentNullException(nameof(rightHandSide), "Rule right-hand side not provided");

            var ruleDefinition = new RuleDefinition(name, description, priority, repeatability, tags, properties, dependencies, leftHandSide, filters, rightHandSide);

            ElementValidator.ValidateUniqueDeclarations(ruleDefinition.LeftHandSide, ruleDefinition.DependencyGroup);
            ElementValidator.ValidateRuleDefinition(ruleDefinition);

            return ruleDefinition;
        }

        /// <summary>
        /// Creates a dependency group element.
        /// </summary>
        /// <param name="dependencies">Dependency elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="DependencyElement"/>
        public static DependencyGroupElement DependencyGroup(params DependencyElement[] dependencies)
        {
            var element = DependencyGroup(dependencies.AsEnumerable());
            return element;
        }

        /// <summary>
        /// Creates a dependency group element.
        /// </summary>
        /// <param name="dependencies">Dependency elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="DependencyElement"/>
        public static DependencyGroupElement DependencyGroup(IEnumerable<DependencyElement> dependencies)
        {
            if (dependencies == null)
                throw new ArgumentNullException(nameof(dependencies), "Dependencies not provided");

            var element = new DependencyGroupElement(dependencies);
            ElementValidator.ValidateUniqueDeclarations(element.Dependencies);
            return element;
        }

        /// <summary>
        /// Creates a dependency element.
        /// </summary>
        /// <param name="type">Dependency type.</param>
        /// <param name="name">Dependency name.</param>
        /// <returns>Created element.</returns>
        public static DependencyElement Dependency(Type type, string name)
        {
            var declaration = Declaration(type, name);
            var element = Dependency(declaration, type);
            return element;
        }

        /// <summary>
        /// Creates a dependency element.
        /// </summary>
        /// <param name="declaration">Declaration that references the dependency.</param>
        /// <param name="serviceType">Type of the service that the dependency represents.</param>
        /// <returns>Created element.</returns>
        public static DependencyElement Dependency(Declaration declaration, Type serviceType)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration), "Dependency declaration not provided");
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType), "Dependency type not provided");

            var element = new DependencyElement(declaration, serviceType);
            declaration.Target = element;
            return element;
        }

        /// <summary>
        /// Creates a left-hand side group element, that contains pattern elements and nested group elements.
        /// </summary>
        /// <param name="groupType">Type of the group element.</param>
        /// <param name="childElements">Child elements contained in the group.</param>
        /// <returns>Created element.</returns>
        public static GroupElement Group(GroupType groupType, IEnumerable<RuleElement> childElements)
        {
            GroupElement element = groupType switch
            {
                GroupType.And => AndGroup(childElements),
                GroupType.Or => OrGroup(childElements),
                _ => throw new ArgumentException($"Unrecognized group type. GroupType={groupType}", nameof(groupType))
            };

            return element;
        }

        /// <summary>
        /// Creates a left-hand side group element that combines contained elements using an AND operator.
        /// </summary>
        /// <param name="childElements">Child elements contained in the group.</param>
        /// <returns>Created element.</returns>
        /// <see cref="RuleElement"/>
        public static AndElement AndGroup(params RuleElement[] childElements)
        {
            var element = AndGroup(childElements.AsEnumerable());
            return element;
        }

        /// <summary>
        /// Creates a left-hand side group element that combines contained elements using an AND operator.
        /// </summary>
        /// <param name="childElements">Child elements contained in the group.</param>
        /// <returns>Created element.</returns>
        /// <see cref="RuleElement"/>
        public static AndElement AndGroup(IEnumerable<RuleElement> childElements)
        {
            if (childElements == null)
                throw new ArgumentNullException(nameof(childElements), "Child elements not provided");

            var element = new AndElement(childElements);
            ElementValidator.ValidateGroup(element);
            ElementValidator.ValidateUniqueDeclarations(element.ChildElements);
            return element;
        }

        /// <summary>
        /// Creates a left-hand side group element that combines contained elements using an OR operator.
        /// </summary>
        /// <param name="childElements">Child elements contained in the group.</param>
        /// <returns>Created element.</returns>
        /// <see cref="RuleElement"/>
        public static OrElement OrGroup(params RuleElement[] childElements)
        {
            var element = OrGroup(childElements.AsEnumerable());
            return element;
        }

        /// <summary>
        /// Creates a left-hand side group element that combines contained elements using an OR operator.
        /// </summary>
        /// <param name="childElements">Child elements contained in the group.</param>
        /// <returns>Created element.</returns>
        /// <see cref="RuleElement"/>
        public static OrElement OrGroup(IEnumerable<RuleElement> childElements)
        {
            if (childElements == null)
                throw new ArgumentNullException(nameof(childElements), "Child elements not provided");

            var element = new OrElement(childElements);
            ElementValidator.ValidateGroup(element);
            return element;
        }

        /// <summary>
        /// Creates an element that represents an existential quantifier.
        /// </summary>
        /// <param name="source">Source element to apply the existential quantifier to.</param>
        /// <returns>Created element.</returns>
        /// <see cref="RuleElement"/>
        public static ExistsElement Exists(RuleElement source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Source element not provided");

            var element = new ExistsElement(source);
            ElementValidator.ValidateExists(element);
            return element;
        }

        /// <summary>
        /// Creates an element that represents a negative existential quantifier.
        /// </summary>
        /// <param name="source">Source element to apply the negative existential quantifier to.</param>
        /// <returns>Created element.</returns>
        public static NotElement Not(RuleElement source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Source element not provided");

            var element = new NotElement(source);
            ElementValidator.ValidateNot(element);
            return element;
        }

        /// <summary>
        /// Creates an element that represents a universal quantifier.
        /// Facts that match the <c>basePattern</c> must also match all other <c>patterns</c>.
        /// </summary>
        /// <param name="basePattern">Base patterns of the universal quantifier that defines the universe of facts to consider.</param>
        /// <param name="patterns">Additional patterns of the universal quantifier that the fact matched by the base pattern must also satisfy.</param>
        /// <returns>Created element.</returns>
        public static ForAllElement ForAll(PatternElement basePattern, IEnumerable<PatternElement> patterns)
        {
            if (basePattern == null)
                throw new ArgumentNullException(nameof(basePattern), "Base pattern not provided");
            if (patterns == null)
                throw new ArgumentNullException(nameof(patterns), "Patterns not provided");

            var element = new ForAllElement(basePattern, patterns);
            ElementValidator.ValidateForAll(element);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match of facts in rules engine's working memory.
        /// </summary>
        /// <param name="type">Type of facts matched by the pattern.</param>
        /// <param name="name">Pattern name.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Type type, string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
        {
            var element = Pattern(type, name, expressions, null);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match of facts in rules engine's working memory.
        /// </summary>
        /// <param name="declaration">Declaration that references the pattern.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Declaration declaration, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
        {
            var element = Pattern(declaration, expressions, null);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match over results of the source element.
        /// </summary>
        /// <param name="type">Type of elements matched by the pattern.</param>
        /// <param name="name">Pattern name.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <param name="source">Source of the elements matched by the pattern. If it's <c>null</c>, the pattern matches facts in rules engine's working memory.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Type type, string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, RuleElement source)
        {
            var declaration = Declaration(type, name);
            var element = Pattern(declaration, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match over results of the source element.
        /// </summary>
        /// <param name="declaration">Declaration that references the pattern.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <param name="source">Source of the elements matched by the pattern. If it's <c>null</c>, the pattern matches facts in rules engine's working memory.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Declaration declaration, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, RuleElement source)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions), "Pattern expressions not provided");

            var expressionElements = expressions.Select(x => new NamedExpressionElement(x.Key, x.Value));
            var element = Pattern(declaration, expressionElements, source);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match over results of the source element.
        /// </summary>
        /// <param name="type">Type of elements matched by the pattern.</param>
        /// <param name="name">Pattern name.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <param name="source">Source of the elements matched by the pattern. If it's <c>null</c>, the pattern matches facts in rules engine's working memory.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Type type, string name, IEnumerable<NamedExpressionElement> expressions, RuleElement source)
        {
            var declaration = Declaration(type, name);
            var element = Pattern(declaration, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates a pattern element that represents a match over results of the source element.
        /// </summary>
        /// <param name="declaration">Declaration that references the pattern.</param>
        /// <param name="expressions">Expressions used by the pattern to match elements.</param>
        /// <param name="source">Source of the elements matched by the pattern. If it's <c>null</c>, the pattern matches facts in rules engine's working memory.</param>
        /// <returns>Created element.</returns>
        public static PatternElement Pattern(Declaration declaration, IEnumerable<NamedExpressionElement> expressions, RuleElement source)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration), "Pattern declaration not provided");
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions), "Pattern expressions not provided");

            var expressionCollection = new ExpressionCollection(expressions);
            var element = new PatternElement(declaration, expressionCollection, source);
            declaration.Target = element;
            ElementValidator.ValidatePattern(element);
            return element;
        }

        /// <summary>
        /// Creates a condition element that represents a condition applied to elements matched by a pattern.
        /// </summary>
        /// <param name="expression">Condition expression. It must have <c>Boolean</c> as its return type.</param>
        /// <returns>Created element.</returns>
        public static NamedExpressionElement Condition(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Condition expression not provided");
            if (expression.ReturnType != typeof(bool))
                throw new ArgumentException($"Pattern condition must return a Boolean result. Condition={expression}");

            var element = new NamedExpressionElement(PatternElement.ConditionName, expression);
            return element;
        }

        /// <summary>
        /// Creates a named expression element that can be used by other rule elements.
        /// </summary>
        /// <param name="name">Name of the expression.</param>
        /// <param name="expression">The actual expression that this element represents.</param>
        /// <returns>Created element.</returns>
        public static NamedExpressionElement Expression(string name, LambdaExpression expression)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Expression name not provided");
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Expression not provided");

            var element = new NamedExpressionElement(name, expression);
            return element;
        }

        /// <summary>
        /// Creates a declaration with a given name and type to be used in other rule elements.
        /// </summary>
        /// <param name="type">Declaration type.</param>
        /// <param name="name">Declaration name.</param>
        /// <returns>Created declaration.</returns>
        public static Declaration Declaration(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "Declaration type not provided");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "Declaration name not provided");

            var declaration = new Declaration(type, name);
            return declaration;
        }

        /// <summary>
        /// Creates an element that represents results of an expression evaluation.
        /// </summary>
        /// <param name="resultType">Type of the expression result.</param>
        /// <param name="expression">Binding expression.</param>
        /// <returns>Created element.</returns>
        public static BindingElement Binding(Type resultType, LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Binding expression not provided");
            if (resultType == null)
                throw new ArgumentNullException(nameof(resultType), "Binding result type not provided");

            var element = new BindingElement(resultType, expression);
            ElementValidator.ValidateBinding(element);
            return element;
        }

        /// <summary>
        /// Creates an element that represents results of an expression evaluation.
        /// </summary>
        /// <param name="expression">Binding expression.</param>
        /// <returns>Created element.</returns>
        public static BindingElement Binding(LambdaExpression expression)
        {
            var element = Binding(expression.ReturnType, expression);
            return element;
        }

        /// <summary>
        /// Creates an element that represents an aggregation of facts.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="name">Aggregate name.</param>
        /// <param name="expressions">Expressions used to construct aggregates from individual facts.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Aggregate(Type resultType, string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, PatternElement source)
        {
            return Aggregate(resultType, name, expressions, source, null);
        }

        /// <summary>
        /// Creates an element that represents an aggregation of facts.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="name">Aggregate name.</param>
        /// <param name="expressions">Expressions used to construct aggregates from individual facts.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <param name="customFactoryType">Factory type used construct aggregators for this aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Aggregate(Type resultType, string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, PatternElement source, Type customFactoryType)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions), "Aggregate expressions not provided");

            var expressionElements = expressions.Select(x => new NamedExpressionElement(x.Key, x.Value));
            var element = Aggregate(resultType, name, expressionElements, source, customFactoryType);
            return element;
        }

        /// <summary>
        /// Creates an element that represents an aggregation of facts.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="name">Aggregate name.</param>
        /// <param name="expressions">Expressions used to construct aggregates from individual facts.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Aggregate(Type resultType, string name, IEnumerable<NamedExpressionElement> expressions, PatternElement source)
        {
            return Aggregate(resultType, name, expressions, source, null);
        }

        /// <summary>
        /// Creates an element that represents an aggregation of facts.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="name">Aggregate name.</param>
        /// <param name="expressions">Expressions used to construct aggregates from individual facts.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <param name="customFactoryType">Factory type used construct aggregators for this aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Aggregate(Type resultType, string name, IEnumerable<NamedExpressionElement> expressions, PatternElement source, Type customFactoryType)
        {
            if (resultType == null)
                throw new ArgumentNullException(nameof(resultType), "Aggregate result type not provided");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "Aggregate name not provided");
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions), "Aggregate expressions not provided");
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Aggregate source pattern not provided");

            var expressionCollection = new ExpressionCollection(expressions);
            var element = new AggregateElement(resultType, name, expressionCollection, source, customFactoryType);
            ElementValidator.ValidateAggregate(element);
            return element;
        }

        /// <summary>
        /// Creates an element that aggregates matching facts into a collection.
        /// </summary>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Collect(PatternElement source)
        {
            var element = Collect(null, source);
            return element;
        }

        /// <summary>
        /// Creates an element that aggregates matching facts into a collection.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Collect(Type resultType, PatternElement source)
        {
            if (resultType == null)
            {
                var enumerableType = typeof(IEnumerable<>);
                resultType = enumerableType.MakeGenericType(source.ValueType);
            }

            var expressions = Array.Empty<KeyValuePair<string, LambdaExpression>>();
            var element = Aggregate(resultType, AggregateElement.CollectName, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates an element that aggregates matching facts into groups.
        /// </summary>
        /// <param name="keySelector">Expression that extracts grouping keys from source element.</param>
        /// <param name="elementSelector">Expression that extracts elements to put into resulting groups.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement GroupBy(LambdaExpression keySelector, LambdaExpression elementSelector, PatternElement source)
        {
            var element = GroupBy(null, keySelector, elementSelector, source);
            return element;
        }

        /// <summary>
        /// Creates an element that aggregates matching facts into groups.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="keySelector">Expression that extracts grouping keys from source element.</param>
        /// <param name="elementSelector">Expression that extracts elements to put into resulting groups.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement GroupBy(Type resultType, LambdaExpression keySelector, LambdaExpression elementSelector, PatternElement source)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector), "GroupBy key selector not provided");
            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector), "GroupBy element selector not provided");

            if (resultType == null)
            {
                var groupingType = typeof(IGrouping<,>);
                resultType = groupingType.MakeGenericType(keySelector.ReturnType, elementSelector.ReturnType);
            }

            var expressions = new List<KeyValuePair<string, LambdaExpression>>
            {
                new(AggregateElement.KeySelectorName, keySelector),
                new(AggregateElement.ElementSelectorName, elementSelector)
            };
            var element = Aggregate(resultType, AggregateElement.GroupByName, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates an element that projects matching facts into different elements.
        /// </summary>
        /// <param name="selector">Expression that translates a matching element into a different element.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Project(LambdaExpression selector, PatternElement source)
        {
            var element = Project(null, selector, source);
            return element;
        }

        /// <summary>
        /// Creates an element that projects matching facts into different elements.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="selector">Expression that translates a matching element into a different element.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Project(Type resultType, LambdaExpression selector, PatternElement source)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector), "Projection selector not provided");

            if (resultType == null)
            {
                resultType = selector.ReturnType;
            }

            var expressions = new List<KeyValuePair<string, LambdaExpression>>
            {
                new(AggregateElement.SelectorName, selector)
            };
            var element = Aggregate(resultType, AggregateElement.ProjectName, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates an element that flattens collections of elements from matching facts into a single set of facts.
        /// </summary>
        /// <param name="resultType">Type of the aggregate result.</param>
        /// <param name="selector">Expression that selects a collection of elements from a matching fact.</param>
        /// <param name="source">Pattern that matches facts for aggregation.</param>
        /// <returns>Created element.</returns>
        public static AggregateElement Flatten(Type resultType, LambdaExpression selector, PatternElement source)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector), "Flattening selector not provided");

            var expressions = new List<KeyValuePair<string, LambdaExpression>>
            {
                new(AggregateElement.SelectorName, selector)
            };
            var element = Aggregate(resultType, AggregateElement.FlattenName, expressions, source);
            return element;
        }

        /// <summary>
        /// Creates an agenda filter group element.
        /// </summary>
        /// <param name="filters">Agenda filter elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="FilterElement"/>
        public static FilterGroupElement FilterGroup(params FilterElement[] filters)
        {
            var element = FilterGroup(filters.AsEnumerable());
            return element;
        }

        /// <summary>
        /// Creates an agenda filter group element.
        /// </summary>
        /// <param name="filters">Agenda filter elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="FilterElement"/>
        public static FilterGroupElement FilterGroup(IEnumerable<FilterElement> filters)
        {
            var element = new FilterGroupElement(filters);
            return element;
        }

        /// <summary>
        /// Creates an agenda filter element.
        /// </summary>
        /// <param name="filterType">Type of agenda filter.</param>
        /// <param name="expression">Filter expression.</param>
        /// <returns>Created element.</returns>
        public static FilterElement Filter(FilterType filterType, LambdaExpression expression)
        {
            var element = new FilterElement(filterType, expression);
            return element;
        }

        /// <summary>
        /// Creates an action group element.
        /// </summary>
        /// <param name="actions">Action elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="ActionElement"/>
        public static ActionGroupElement ActionGroup(params ActionElement[] actions)
        {
            var element = ActionGroup(actions.AsEnumerable());
            return element;
        }

        /// <summary>
        /// Creates an action group element.
        /// </summary>
        /// <param name="actions">Action elements in the group.</param>
        /// <returns>Created element.</returns>
        /// <seealso cref="ActionElement"/>
        public static ActionGroupElement ActionGroup(IEnumerable<ActionElement> actions)
        {
            var element = new ActionGroupElement(actions);
            var insertActions = element.Actions.Where(x => x.ActionTrigger.HasFlag(ActionTrigger.Activated)).ToList();
            if (insertActions.Count == 0)
            {
                throw new ArgumentException("Rule must have at least one match action");
            }
            return element;
        }

        /// <summary>
        /// Creates an action element that represents an action taken by the engine when the rule fires.
        /// </summary>
        /// <param name="expression">Action expression. It must have <see cref="IContext"/> as it's first parameter.</param>
        /// <param name="actionTrigger">Action trigger that indicates when the action should execute.</param>
        /// <returns>Created element.</returns>
        public static ActionElement Action(LambdaExpression expression, ActionTrigger actionTrigger)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Action expression not provided");
            if (actionTrigger == ActionTrigger.None)
                throw new ArgumentException("Action trigger not provided");
            if (expression.Parameters.Count == 0 ||
                expression.Parameters.First().Type != typeof(IContext))
                throw new ArgumentException($"Action expression must have {typeof(IContext)} as its first parameter. Action={expression}");

            var element = new ActionElement(expression, actionTrigger);
            return element;
        }

        /// <summary>
        /// Creates an action element that represents an action taken by the engine when the rule fires.
        /// The action element is created with the default trigger, which executes the action when rule
        /// is <see cref="ActionTrigger.Activated"/> or <see cref="ActionTrigger.Reactivated"/>.
        /// </summary>
        /// <param name="expression">Action expression. It must have <see cref="IContext"/> as it's first parameter.</param>
        /// <returns>Created element.</returns>
        public static ActionElement Action(LambdaExpression expression)
        {
            var defaultTrigger = ActionTrigger.Activated | ActionTrigger.Reactivated;
            var element = Action(expression, defaultTrigger);
            return element;
        }
    }
}