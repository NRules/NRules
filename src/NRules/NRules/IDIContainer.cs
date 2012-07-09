using System;

namespace NRules
{
    public interface IDIContainer
    {
        object GetObjectInstance(Type type);
    }
}
