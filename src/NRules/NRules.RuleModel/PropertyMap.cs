using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.RuleModel
{
    /// <summary>
    /// Readonly map of rule properties.
    /// </summary>
    public class PropertyMap : IEnumerable<RuleProperty>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly Dictionary<string, RuleProperty> _properties;

        public PropertyMap(IEnumerable<RuleProperty> properties)
        {
            _properties = new Dictionary<string, RuleProperty>(properties.ToDictionary(x => x.Name));
        }

        /// <summary>
        /// Number of properties in the map.
        /// </summary>
        public int Count
        {
            get { return _properties.Count; }
        }

        /// <summary>
        /// Retrieves property by name.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>Matching property value.</returns>
        public object this[string name]
        {
            get
            {
                RuleProperty result;
                var found = _properties.TryGetValue(name, out result);
                if (!found)
                {
                    throw new ArgumentException(
                        string.Format("Property with the given name not found. Name={0}", name), "name");
                }
                return result.Value;
            }
        }

        /// <summary>
        /// Retrieves property by name if it exists.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="property">Matching property if found.</param>
        /// <returns>If found <c>true</c>, otherwise <c>false</c>.</returns>
        public bool TryGetProperty(string name, out RuleProperty property)
        {
            var found = _properties.TryGetValue(name, out property);
            return found;
        }

        public IEnumerator<RuleProperty> GetEnumerator()
        {
            return _properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}