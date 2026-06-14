using System.Text.Json;
using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Infrastructure.Definitions.Converters.Handlers;

namespace AshenWar.Infrastructure.Definitions.Converters;

public sealed class ValueSourceConverter : JsonConverter<ValueSource>
{
    private static readonly Dictionary<string, IValueSourceHandler> ByDiscriminator = new()
    {
        ["Fixed"] = new FixedHandler(),
        ["FromUnitStat"] = new FromUnitStatHandler(),
        ["ScaledFromUnitStat"] = new ScaledFromUnitStatHandler(),
        ["FromAbilityStat"] = new FromAbilityStatHandler(),
    };

    private static readonly Dictionary<Type, (string Discriminator, IValueSourceHandler Handler)> ByType = new()
    {
        [typeof(Fixed)] = ("Fixed", new FixedHandler()),
        [typeof(FromUnitStat)] = ("FromUnitStat", new FromUnitStatHandler()),
        [typeof(ScaledFromUnitStat)] = ("ScaledFromUnitStat", new ScaledFromUnitStatHandler()),
        [typeof(FromAbilityStat)] = ("FromAbilityStat", new FromAbilityStatHandler()),
    };

    public override ValueSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var root = JsonDocument.ParseValue(ref reader).RootElement;
        var type = root.GetProperty("type").GetString()
            ?? throw new JsonException("Missing 'type' discriminator in ValueSource");
        
        if (!ByDiscriminator.TryGetValue(type, out var handler))
            throw new JsonException($"Unknown ValueSource type: '{type}'");
        
        return handler.Read(root);
    }

    public override void Write(Utf8JsonWriter writer, ValueSource value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        if (!ByType.TryGetValue(type, out var entry))
            throw new JsonException($"Unregistered ValueSource type: '{type.Name}'");
        
        writer.WriteStartObject();
        writer.WriteString("type", entry.Discriminator);
        entry.Handler.Write(writer, value);
        writer.WriteEndObject();
    }
}