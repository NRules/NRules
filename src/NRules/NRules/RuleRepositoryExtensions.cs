namespace NRules
{
    public static class RuleRepositoryExtensions
    {
        public static ISessionFactory Compile(this IRuleRepository repository)
        {
            IRuleCompiler compiler = new RuleCompiler();
            ISessionFactory factory = compiler.Compile(repository.GetRules());
            return factory;
        }
    }
}