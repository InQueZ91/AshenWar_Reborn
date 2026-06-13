using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.Contracts.Definitions;

public interface IUnitDefinitionRepository
{
    Task<IReadOnlyList<UnitDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UnitDefinition?> FindAsync(UnitDefinitionId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitDefinition>> GetManyAsync(IReadOnlyList<UnitDefinitionId> ids, CancellationToken cancellationToken = default);
}