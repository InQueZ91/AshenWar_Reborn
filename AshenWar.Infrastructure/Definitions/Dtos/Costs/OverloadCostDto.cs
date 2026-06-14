using AshenWar.Domain.Entities.Costs;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Infrastructure.Definitions.Dtos.Costs;

public sealed record OverloadCostDto(int RequiredStacks) : CostDefinitionDto
{
    public override CostDefinition ToDomain() => new CostDefinition.Overload(RequiredStacks);
}