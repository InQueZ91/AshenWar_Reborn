using System.Text.Json;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.ValueSources;

namespace AshenWar.Infrastructure.Definitions.Converters.Handlers;

internal sealed class FixedHandler : IValueSourceHandler
{
    public ValueSource Read(JsonElement root) =>
        new Fixed(root.GetProperty("value").GetSingle());

    public void Write(Utf8JsonWriter writer, ValueSource value)
    {
        var f = (Fixed)value;
        writer.WriteNumber("value", f.Value);
    }
}