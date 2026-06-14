using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Application.Contracts.Definitions;

public interface IPassiveDefinitionRepository
{
    Task<IReadOnlyList<PassiveDefinition>> GetAllPassiveAsync(CancellationToken cancellationToken = default);
    Task<PassiveDefinition?> FindPassiveAsync(PassiveDefinitionId id, CancellationToken cancellationToken = default);
}