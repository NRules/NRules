using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRules.Json.Utilities;

internal static class TypeNameFormatter
{
    private static readonly HashSet<string> SystemAssemblyNames = new() { "mscorlib", "System.Core", "System.Private.CoreLib" };

    public static string ConstructAssemblyQualifiedTypeName(TypeName typeName)
    {
        if (typeName.AssemblyName is null || SystemAssemblyNames.Contains(typeName.AssemblyName.Name))
            return typeName.FullName;

        return Assembly.CreateQualifiedName(typeName.AssemblyName.FullName, typeName.FullName);
    }

    public static TypeName ConstructGenericTypeName(TypeName definitionTypeName, IReadOnlyList<TypeName> typeArgumentNames)
    {
        var sb = new StringBuilder(definitionTypeName.FullName);
        sb.Append('[');
        for (var i = 0; i < typeArgumentNames.Count; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append('[');
            sb.Append(ConstructAssemblyQualifiedTypeName(typeArgumentNames[i]));
            sb.Append(']');
        }

        sb.Append(']');
        return new TypeName(sb.ToString(), definitionTypeName.AssemblyName);
    }

    public static TypeName ConstructArrayTypeName(TypeName elementTypeName, int arrayRank)
    {
        var sb = new StringBuilder(elementTypeName.FullName);
        sb.Append('[');
        sb.Append(',', arrayRank - 1);
        sb.Append(']');
        return new TypeName(sb.ToString(), elementTypeName.AssemblyName);
    }
}