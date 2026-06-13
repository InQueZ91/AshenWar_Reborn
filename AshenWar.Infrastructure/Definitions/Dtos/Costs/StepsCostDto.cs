using AshenWar.Domain.Entities.Costs;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Infrastructure.Definitions.Dtos.Costs;

public sealed record StepsCostDto(int Amount) : CostDefinitionDto
{
    public override CostDefinition ToDomain() => new CostDefinition.Steps(Amount);
}