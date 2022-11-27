using System;
using System.Reflection;

namespace NRules.Json.Utilities;

internal class TypeName
{
    public TypeName(Type type)
    {
        FullName = type.FullName;
        AssemblyName = type.Assembly.GetName();
    }

    public TypeName(string typeFullName)
        : this(typeFullName, null)
    {
    }

    public TypeName(string typeFullName, AssemblyName? assemblyName)
    {
        FullName = typeFullName;
        AssemblyName = assemblyName;
    }

    public string FullName { get; }

    public AssemblyName? AssemblyName { get; }
}