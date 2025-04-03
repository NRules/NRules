using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Integration.SimpleInjector;

public interface ICompiledRuleSets
{
    IRuleActivator? RuleActivator { get; set; }
    IRuleBulkLoadSpec? RuleBulkLoadSpec { get; }
    List<string> ActiveRuleSetNames { get; }
    
    void DefineSpecification(RuleBulkLoadSpecBuilder builder);

    IRuleBulkLoadSpec GetOrDefine();
    
    IRuleRepository GetOrLoad();

    ISessionFactory GetOrCompile();

    ISession GetOrCreate();
}

public interface ICompiledRuleSets<T> : ICompiledRuleSets;

public abstract class CompiledRuleSets<T> : ICompiledRuleSets<T>
{
    private RuleRepository? _ruleRepository;
    private ISessionFactory? _sessionFactory;
    private ISession? _session;

    public IRuleActivator? RuleActivator { get; set; }
    public IRuleBulkLoadSpec? RuleBulkLoadSpec { get; internal set; }
    public List<string> ActiveRuleSetNames { get; internal set; } = [];

    public abstract void DefineSpecification(RuleBulkLoadSpecBuilder builder);

    public IRuleBulkLoadSpec GetOrDefine()
    {
        if (RuleBulkLoadSpec == null)
        {
            RuleBulkLoadSpecBuilder builder = new RuleBulkLoadSpecBuilder();
            this.DefineSpecification(builder);
            this.RuleBulkLoadSpec = builder.Build();
        }
        return RuleBulkLoadSpec;
    }
    
    public IRuleRepository GetOrLoad()
    {
        if (_ruleRepository == null)
        {
            this._ruleRepository = new RuleRepository(this.RuleActivator!);
            this._ruleRepository.BulkLoad(this.GetOrDefine().Load(this.RuleActivator!));
        }

        return this._ruleRepository;
    }

    public ISessionFactory GetOrCompile()
    {
        if (this._sessionFactory == null)
        {
            var compiler = new RuleCompiler();
            var sets = this.GetOrLoad().GetRuleSets();
            if (this.ActiveRuleSetNames.Any())
            {
                var activeSets = sets.Where(x => ActiveRuleSetNames.Contains(x.Name));
                this._sessionFactory = compiler.Compile(activeSets);
            }
            else
            {
                this._sessionFactory = compiler.Compile(sets);
            }
        }

        return this._sessionFactory;
    }

    public ISession GetOrCreate()
    {
        if (this._session == null)
        {
            var sessionFactory = this.GetOrCompile();
            this._session = sessionFactory.CreateSession();
        }
        
        return this._session;
    }
}