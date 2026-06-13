using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshenWar.Infrastructure.Definitions.Converters;

public sealed class IdConverter<TId> : JsonConverter<TId>
{
    public override TId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guid = reader.GetGuid();
        return (TId)Activator.CreateInstance(typeof(TId), guid)!;
    }

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        
        writer.WriteStringValue(value.ToString());
    }
}