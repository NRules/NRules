using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

/// <summary>
/// Creates a <see href="https://en.wikipedia.org/wiki/DGML">DGML</see> document writer
/// that can be used to serialize a Rete graph for a given rules session into XML.
/// </summary>
public class DgmlWriter
{
    private readonly ReteGraph _schema;

    private ICollection<string>? _ruleNameFilter;
    private IMetricsProvider? _metricsProvider;

    /// <summary>
    /// Sets a filter for Rete graph nodes, such that only nodes that belong to the given
    /// set of rules is serialized, along with the connecting graph edges.
    /// </summary>
    /// <param name="ruleNames">Set of rules to use as a filter, or <c>null</c> to remove the filter.</param>
    public void SetRuleFilter(IEnumerable<string>? ruleNames)
    {
        _ruleNameFilter = ruleNames == null ? null : new HashSet<string>(ruleNames);
    }

    /// <summary>
    /// Sets the <see cref="IMetricsProvider"/> to retrieve performance metrics for serialized nodes,
    /// so that performance metrics are included in the output.
    /// </summary>
    /// <param name="metricsProvider">Performance metrics provider or <c>null</c> to exclude performance metrics from the output.</param>
    public void SetMetricsProvider(IMetricsProvider metricsProvider)
    {
        _metricsProvider = metricsProvider;
    }

    /// <summary>
    /// Creates an instance of a <c>DgmlWriter</c> for a given session schema.
    /// </summary>
    /// <param name="schema">Rules session schema.</param>
    public DgmlWriter(ReteGraph schema)
    {
        _schema = schema;
    }

    /// <summary>
    /// Writes DGML graph representing a given rules session to a file.
    /// </summary>
    /// <param name="fileName">File to write the session to.</param>
    public void WriteAllText(string fileName)
    {
        var contents = GetContents();
        File.WriteAllText(fileName, contents);
    }

    /// <summary>
    /// Writes DGML graph representing a given rules session to the provided
    /// <see cref="XmlWriter"/>.
    /// </summary>
    /// <param name="writer"><see cref="XmlWriter"/> to write the session to.</param>
    public void WriteXml(XmlWriter writer)
    {
        var graph = _schema.ConvertToDirectedGraph(_ruleNameFilter, _metricsProvider);

        writer.WriteStartDocument();
        graph.WriteXml(writer);
        writer.WriteEndDocument();
    }

    /// <summary>
    /// Retrieves a serialized DGML graph as an XML string.
    /// </summary>
    /// <returns>Contents of the serialized DGML graph as an XML string.</returns>
    public string GetContents()
    {
        using var stringWriter = new Utf8StringWriter();
        var xmlWriter = new XmlTextWriter(stringWriter)
        {
            Formatting = Formatting.Indented
        };
        WriteXml(xmlWriter);
        var contents = stringWriter.ToString();
        return contents;
    }

    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
