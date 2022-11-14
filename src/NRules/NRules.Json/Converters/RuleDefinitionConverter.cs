using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Json.Converters;

internal class RuleDefinitionConverter : JsonConverter<IRuleDefinition>
{
    public override IRuleDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();

        var name = reader.ReadStringProperty(nameof(IRuleDefinition.Name), options);
        if (!reader.TryReadStringProperty(nameof(IRuleDefinition.Description), options, out var description))
            description = string.Empty;
        if (!reader.TryReadInt32Property(nameof(IRuleDefinition.Priority), options, out var priority))
            priority = 0;
        if (!reader.TryReadEnumProperty<RuleRepeatability>(nameof(IRuleDefinition.Repeatability), options, out var repeatability))
            repeatability = RuleRepeatability.Repeatable;
        reader.TryReadArrayProperty(nameof(IRuleDefinition.Tags), options, out string[] tags);
        reader.TryReadArrayProperty<RuleProperty>(nameof(IRuleDefinition.Properties), options, out var properties);
        reader.TryReadArrayProperty<DependencyElement>(nameof(DependencyGroupElement.Dependencies), options, out var dependencies);
        var lhs = reader.ReadProperty<GroupElement>(nameof(IRuleDefinition.LeftHandSide), options);
        reader.TryReadArrayProperty<FilterElement>(nameof(FilterGroupElement.Filters), options, out var filters);
        var actions = reader.ReadArrayProperty<ActionElement>(nameof(IRuleDefinition.RightHandSide), options);
        
        var ruleDefinition = Element.RuleDefinition(name, description, priority, 
            repeatability, tags, properties, Element.DependencyGroup(dependencies),
            lhs, Element.FilterGroup(filters), Element.ActionGroup(actions));
        return ruleDefinition;
    }

    public override void Write(Utf8JsonWriter writer, IRuleDefinition value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStringProperty(nameof(IRuleDefinition.Name), value.Name, options);
        if (!string.IsNullOrEmpty(value.Description))
            writer.WriteStringProperty(nameof(IRuleDefinition.Description), value.Description, options);
        if (value.Priority != 0)
            writer.WriteNumberProperty(nameof(IRuleDefinition.Priority), value.Priority, options);
        if (value.Repeatability != RuleRepeatability.Repeatable)
            writer.WriteEnumProperty(nameof(IRuleDefinition.Repeatability), value.Repeatability, options);

        if (value.Tags.Any())
            writer.WriteArrayProperty(nameof(IRuleDefinition.Tags), value.Tags, options);

        if (value.Properties.Any())
            writer.WriteArrayProperty(nameof(IRuleDefinition.Properties), value.Properties, options);

        if (value.DependencyGroup.Dependencies.Any())
            writer.WriteArrayProperty(nameof(DependencyGroupElement.Dependencies), value.DependencyGroup.Dependencies, options);

        writer.WriteProperty(nameof(IRuleDefinition.LeftHandSide), value.LeftHandSide, options);
        
        if (value.FilterGroup.Filters.Any())
            writer.WriteArrayProperty(nameof(FilterGroupElement.Filters), value.FilterGroup.Filters, options);
        
        writer.WriteArrayProperty(nameof(IRuleDefinition.RightHandSide), value.RightHandSide.Actions, options);
        writer.WriteEndObject();
    }
}
