using System;

namespace NRules.Json.Utilities
{
    internal readonly struct MethodRecord
    {
        public readonly string Name;
        public readonly Type DeclaringType;

        public MethodRecord(string name, Type declaringType)
        {
            Name = name;
            DeclaringType = declaringType;
        }
    }
}