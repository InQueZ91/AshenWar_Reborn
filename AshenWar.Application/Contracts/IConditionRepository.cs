using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Conditions.Global;
using Domain.Entities.Conditions.Tile;
using Domain.Entities.Conditions.Unit;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Application.Contracts;

public interface IConditionRepository
{
    //CRUD
    Task<UnitConditionDefinition> GetUnitConditionAsync(UnitConditionDefinitionId id, CancellationToken cancellationToken = default);
    Task<TileConditionDefinition> GetTileConditionAsync(TileConditionDefinitionId id, CancellationToken cancellationToken = default);
    Task<GlobalConditionDefinition> GetGlobalConditionAsync(GlobalConditionDefinitionId id, CancellationToken cancellationToken = default);
}