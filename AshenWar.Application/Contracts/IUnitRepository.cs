using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Units;
using Domain.ValueObjects.Identifiers.Units;

namespace Application.Contracts;

public interface IUnitRepository
{
    Task<UnitDefinition> GetUnitAsync(UnitDefinitionId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitDefinition>> GetManyAsync(IReadOnlyList<UnitDefinitionId> ids, CancellationToken cancellationToken = default);
}