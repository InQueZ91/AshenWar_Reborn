using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Infrastructure.Definitions.Dtos.Conditions;

public sealed record UnitConditionDefinitionDto : ConditionDefinitionBaseDto
{
    public Guid Id { get; init; }
    public IEnumerable<ModifierDto> Modifiers { get; init; } = [];

    public override ConditionDefinitionBase ToDomain()
    {
        var id = new UnitConditionDefinitionId(Id);
        var definition = UnitConditionDefinition.Load(id, Name);
        
        ApplyBase(definition);
        
        foreach (var mod in Modifiers)
            definition.AddModifier(mod.ToDomain());
        
        return definition;   
    }
}