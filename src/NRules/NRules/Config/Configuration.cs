namespace NRules.Config
{
    public class Configuration
    {
        private readonly IContainer _container;

        public IContainer Container
        {
            get { return _container; }
        }

        internal Configuration(IContainer container)
        {
            _container = container;
        }

        protected Configuration(Configuration config) : this(config.Container)
        {
        }
    }
}