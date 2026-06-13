using System.Text.Json;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Infrastructure.Definitions.Converters.Handlers;

internal sealed class FromAbilityStatHandler : IValueSourceHandler
{
    public ValueSource Read(JsonElement root) =>
        new FromAbilityStat(
            StatDefinition.FromName(root.GetProperty("stat").GetString()!),
            new AbilityDefinitionId(root.GetProperty("abilityDefinitionId").GetGuid()));

    public void Write(Utf8JsonWriter writer, ValueSource value)
    {
        var a = (FromAbilityStat)value;
        writer.WriteString("stat", a.Stat.Name);
        writer.WriteString("abilityDefinitionId", a.AbilityDefinitionId.Value.ToString());
    }
}