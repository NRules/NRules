using NRules.Config;

namespace NRules.Core.Config
{
    public static class ConfigurationExtensions
    {
        public static EngineConfiguration DefaultSettings(this Configuration config)
        {
            return new EngineConfiguration(config);
        }
    }
}