using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Infrastructure.Definitions.Dtos.Conditions;

public sealed record GlobalConditionDefinitionDto : ConditionDefinitionBaseDto
{
    public Guid Id { get; init; }

    public override ConditionDefinitionBase ToDomain()
    {
        var id = new GlobalConditionDefinitionId(Id);
        var definition = GlobalConditionDefinition.Load(id, Name);
        
        ApplyBase(definition);
        
        return definition;
    }
}