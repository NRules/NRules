using System;
using System.Reflection;

namespace NRules.Json.Utilities;

internal class TypeName
{
    public TypeName(Type type)
    {
        FullName = type.FullName 
            ?? throw new ArgumentException($"Unable to get type name. Type={type}");
        AssemblyName = type.Assembly.GetName();
    }

    public TypeName(string typeFullName, AssemblyName? assemblyName = null)
    {
        FullName = typeFullName;
        AssemblyName = assemblyName;
    }

    public string FullName { get; }
    public AssemblyName? AssemblyName { get; }
}