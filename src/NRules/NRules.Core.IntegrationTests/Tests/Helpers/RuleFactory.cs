using System;
using NRules.Core.IntegrationTests.Rules;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    //note: poor man's temporary DI container.
    internal class RuleFactory : IDIContainer
    {
        private readonly EventHandler _eventHandler;

        public RuleFactory(EventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }
        
        public object GetObjectInstance(Type type)
        {
            if (type == typeof(SimplePersonalFinancesRule))
            {
                return new SimplePersonalFinancesRule(_eventHandler);
            }

            throw new ArgumentException(string.Format("Cannot initialize type {0}", type));
        }
    }
}
