using System;

namespace NRules
{
    public interface IContainer
    {
        object GetObjectInstance(Type type);
    }
}