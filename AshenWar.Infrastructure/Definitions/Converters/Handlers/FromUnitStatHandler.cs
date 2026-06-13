using System.Text.Json;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Domain.Entities.Stats;

namespace AshenWar.Infrastructure.Definitions.Converters.Handlers;

internal sealed class FromUnitStatHandler : IValueSourceHandler
{
    public ValueSource Read(JsonElement root) =>
        new FromUnitStat(StatDefinition.FromName(root.GetProperty("stat").GetString()!));

    public void Write(Utf8JsonWriter writer, ValueSource value)
    {
        var u = (FromUnitStat)value;
        writer.WriteString("stat", u.Stat.Name);
    }
}