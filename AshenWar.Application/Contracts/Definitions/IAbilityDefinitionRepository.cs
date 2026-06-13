using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Application.Contracts.Definitions;

public interface IAbilityDefinitionRepository
{
    Task<IReadOnlyList<AbilityDefinition>> GetAllAbilityAsync(CancellationToken cancellationToken = default);
    Task<AbilityDefinition?> FindAbilityAsync(AbilityDefinitionId id, CancellationToken cancellationToken = default);
}