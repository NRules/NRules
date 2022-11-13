using NRules.AgendaFilters;
using NRules.Aggregators;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Utilities;

namespace NRules;

/// <summary>
/// Compiles rules in a canonical rule model form into an executable representation.
/// </summary>
public class RuleCompiler
{
    private readonly RuleCompilerOptions _options;
    private readonly AggregatorRegistry _aggregatorRegistry = new();
    private readonly IRuleExpressionCompiler _ruleExpressionCompiler = new RuleExpressionCompiler();

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleCompiler"/> class
    /// using the default <see cref="RuleCompilerOptions"/>.
    /// </summary>
    public RuleCompiler()
        : this(new RuleCompilerOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleCompiler"/> class
    /// using the specified <see cref="RuleCompilerOptions"/>.
    /// </summary>
    /// <param name="options"></param>
    public RuleCompiler(RuleCompilerOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Registry of custom aggregator factories.
    /// </summary>
    public AggregatorRegistry AggregatorRegistry => _aggregatorRegistry;

    /// <summary>
    /// Compiles expressions used in rules conditions and actions into executable delegates.
    /// Default implementation uses the built-in .NET expression compiler.
    /// </summary>
    public IExpressionCompiler ExpressionCompiler
    {
        get => _ruleExpressionCompiler.ExpressionCompiler;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            _ruleExpressionCompiler.ExpressionCompiler = value;
        }
    }

    /// <summary>
    /// Compiles a collection of rules into a session factory.
    /// </summary>
    /// <param name="ruleDefinitions">Rules to compile.</param>
    /// <returns>Session factory.</returns>
    /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
    /// <seealso cref="IRuleRepository"/>
    public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
    {
        return Compile(ruleDefinitions, default);
    }

    /// <summary>
    /// Compiles a collection of rules into a session factory.
    /// </summary>
    /// <param name="ruleDefinitions">Rules to compile.</param>
    /// <param name="cancellationToken">Enables cooperative cancellation of the rules compilation.</param>
    /// <returns>Session factory.</returns>
    /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
    /// <seealso cref="IRuleRepository"/>
    public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions, CancellationToken cancellationToken)
    {
        IReteBuilder reteBuilder = new ReteBuilder(_options, _aggregatorRegistry, _ruleExpressionCompiler);
        var compiledRules = new List<ICompiledRule>();
        foreach (var ruleDefinition in ruleDefinitions)
        {
            try
            {
                var compiledRule = CompileRule(reteBuilder, ruleDefinition);
                compiledRules.AddRange(compiledRule);
            }
            catch (Exception e)
            {
                throw new RuleCompilationException("Failed to compile rule", ruleDefinition.Name, e);
            }

            if (cancellationToken.IsCancellationRequested)
                break;
        }

        var network = reteBuilder.Build();
        var factory = new SessionFactory(network, compiledRules);
        return factory;
    }

    /// <summary>
    /// Compiles rules from rule sets into a session factory.
    /// </summary>
    /// <param name="ruleSets">Rule sets to compile.</param>
    /// <returns>Session factory.</returns>
    /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
    public ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets)
    {
        return Compile(ruleSets, default);
    }

    /// <summary>
    /// Compiles rules from rule sets into a session factory.
    /// </summary>
    /// <param name="ruleSets">Rule sets to compile.</param>
    /// <param name="cancellationToken">Enables cooperative cancellation of the rules compilation.</param>
    /// <returns>Session factory.</returns>
    /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
    public ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets, CancellationToken cancellationToken)
    {
        var rules = ruleSets.SelectMany(x => x.Rules);
        return Compile(rules, cancellationToken);
    }

    private IEnumerable<ICompiledRule> CompileRule(IReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
    {
        var rules = new List<ICompiledRule>();

        var transformation = new RuleTransformation();
        var transformedRule = transformation.Transform(ruleDefinition);
        var ruleDeclarations = transformedRule.LeftHandSide.Exports;

        var dependencies = transformedRule.DependencyGroup.Dependencies;
        var terminals = reteBuilder.AddRule(transformedRule);

        foreach (var terminal in terminals)
        {
            var filter = CompileFilters(transformedRule, ruleDeclarations, terminal.FactMap);

            var rightHandSide = transformedRule.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var ruleAction = _ruleExpressionCompiler.CompileAction(action, ruleDeclarations, dependencies, terminal.FactMap);
                actions.Add(ruleAction);
            }

            var rule = new CompiledRule(ruleDefinition, ruleDeclarations, actions, filter, terminal.FactMap);
            var ruleNode = new RuleNode(reteBuilder.GetNodeId(), rule);
            terminal.Source.Attach(ruleNode);
            rules.Add(rule);
        }

        return rules;
    }

    private IRuleFilter CompileFilters(IRuleDefinition ruleDefinition, IReadOnlyCollection<Declaration> ruleDeclarations, IndexMap tupleFactMap)
    {
        var conditions = new List<IActivationExpression<bool>>();
        var keySelectors = new List<IActivationExpression<object>>();
        foreach (var filter in ruleDefinition.FilterGroup.Filters)
        {
            switch (filter.FilterType)
            {
                case FilterType.Predicate:
                    var condition = _ruleExpressionCompiler.CompileActivationExpression<bool>(filter, ruleDeclarations, tupleFactMap);
                    conditions.Add(condition);
                    break;
                case FilterType.KeyChange:
                    var keySelector = _ruleExpressionCompiler.CompileActivationExpression<object>(filter, ruleDeclarations, tupleFactMap);
                    keySelectors.Add(keySelector);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized filter type. FilterType={filter.FilterType}");
            }
        }
        return new RuleFilter(conditions, keySelectors);
    }
}
