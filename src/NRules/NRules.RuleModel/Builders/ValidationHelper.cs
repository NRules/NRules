using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Builders
{
    internal static class ValidationHelper
    {
        public static void AssertUniqueDeclarations(params RuleElement[] elements)
        {
            AssertUniqueDeclarations(elements.AsEnumerable());
        }

        public static void AssertUniqueDeclarations(IEnumerable<RuleElement> elements)
        {
            var duplicates = elements.SelectMany(x => x.Exports)
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .ToArray();
            if (duplicates.Any())
            {
                var declarations = string.Join(",", duplicates.Select(x => x.Key));
                throw new InvalidOperationException($"Duplicate declarations. Declaration={declarations}");
            }
        }
    }
}