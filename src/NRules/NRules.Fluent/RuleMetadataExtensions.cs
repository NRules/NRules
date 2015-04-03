using System.Linq;

namespace NRules.Fluent
{
    public static class RuleMetadataExtensions
    {
        /// <summary>
        /// Tests if the rule is tagged with a given tag.
        /// </summary>
        /// <param name="metadata">Rule metadata instance.</param>
        /// <param name="tag">Tag to test.</param>
        /// <returns><c>true</c> if the rule is tagged, <c>false</c> otherwise.</returns>
        public static bool IsTagged(this IRuleMetadata metadata, string tag)
        {
            return metadata.Tags.Contains(tag);
        }
    }
}