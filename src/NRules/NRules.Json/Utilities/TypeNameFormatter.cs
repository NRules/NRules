using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRules.Json.Utilities
{
    internal static class TypeNameFormatter
    {
        private static readonly HashSet<string> SystemAssemblyNames = new() { "mscorlib", "System.Core", "System.Private.CoreLib" };

        public static string ConstructAssemblyQualifiedTypeName(TypeName typeName)
        {
            if (typeName.FullName == null ||
                typeName.AssemblyName == null ||
                SystemAssemblyNames.Contains(typeName.AssemblyName.Name)) return typeName.FullName;
            return Assembly.CreateQualifiedName(typeName.AssemblyName.FullName, typeName.FullName);
        }
        
        public static TypeName ConstructGenericTypeName(TypeName definitionName, TypeName[] typeArgumentNames)
        {
            var sb = new StringBuilder();
            sb.Append(definitionName.FullName);
            sb.Append('[');
            for (var i = 0; i < typeArgumentNames.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('[');
                sb.Append(ConstructAssemblyQualifiedTypeName(typeArgumentNames[i]));
                sb.Append(']');
            }
            sb.Append(']');
            return new TypeName(sb.ToString(), definitionName.AssemblyName);
        }
    }
}