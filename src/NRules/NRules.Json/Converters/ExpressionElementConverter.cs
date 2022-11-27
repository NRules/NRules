using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Json.Converters;

internal class NamedExpressionElementConverter : JsonConverter<NamedExpressionElement>
{
    public override NamedExpressionElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        var name = reader.ReadStringProperty(nameof(NamedExpressionElement.Name), options)
            ?? throw new JsonException($"Property '{nameof(NamedExpressionElement.Name)}' should have not null value");
        var expression = reader.ReadProperty<LambdaExpression>(nameof(NamedExpressionElement.Expression), options)
            ?? throw new JsonException($"Property '{nameof(NamedExpressionElement.Expression)}' should have not null value");
        return Element.Expression(name, expression);
    }

    public override void Write(Utf8JsonWriter writer, NamedExpressionElement value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStringProperty(nameof(NamedExpressionElement.Name), value.Name, options);
        writer.WriteProperty(nameof(NamedExpressionElement.Expression), value.Expression, options);
        writer.WriteEndObject();
    }
}

internal class ActionElementConverter : JsonConverter<ActionElement>
{
    public override ActionElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        if (!reader.TryReadEnumProperty<ActionTrigger>(nameof(ActionElement.ActionTrigger), options, out var trigger))
            trigger = ActionElement.DefaultTrigger;
        var expression = reader.ReadProperty<LambdaExpression>(nameof(ActionElement.Expression), options)
            ?? throw new JsonException($"Property '{nameof(ActionElement.Expression)}' should have not null value");
        return Element.Action(expression, trigger);
    }

    public override void Write(Utf8JsonWriter writer, ActionElement value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.ActionTrigger != ActionElement.DefaultTrigger)
            writer.WriteEnumProperty(nameof(ActionElement.ActionTrigger), value.ActionTrigger, options);
        writer.WriteProperty(nameof(ActionElement.Expression), value.Expression, options);
        writer.WriteEndObject();
    }
}
