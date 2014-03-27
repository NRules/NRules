using System.Collections.Generic;

namespace NRules.RuleModel
{
    internal class RuleMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> Tags { get; set; }
    }
}