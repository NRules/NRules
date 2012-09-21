using System;
using System.Collections.Generic;

namespace NRules.Config
{
    public interface IContainer
    {
        object Build(Type typeToBuild);
        IEnumerable<object> BuildAll(Type typeToBuild);
        void Configure(Type component, DependencyLifecycle lifecycle);
        void Register(Type component, object instance);
        void ConfigureProperty(Type component, string property, object value);
    }
}