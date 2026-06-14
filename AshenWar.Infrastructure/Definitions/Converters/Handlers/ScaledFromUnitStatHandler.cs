using System.Text.Json;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Domain.Entities.Stats;

namespace AshenWar.Infrastructure.Definitions.Converters.Handlers;

internal sealed class ScaledFromUnitStatHandler : IValueSourceHandler
{
    public ValueSource Read(JsonElement root) =>
        new ScaledFromUnitStat(
            StatDefinition.FromName(root.GetProperty("stat").GetString()!),
            root.GetProperty("multiplier").GetSingle());

    public void Write(Utf8JsonWriter writer, ValueSource value)
    {
        var su = (ScaledFromUnitStat)value;
        writer.WriteString("stat", su.Stat.Name);
        writer.WriteNumber("multiplier", su.Multiplier);
    }
}