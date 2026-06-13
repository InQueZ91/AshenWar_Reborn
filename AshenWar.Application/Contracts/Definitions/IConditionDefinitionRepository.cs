using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Application.Contracts.Definitions;

public interface IConditionDefinitionRepository
{
    //CRUD
    Task<UnitConditionDefinition> GetUnitConditionAsync(UnitConditionDefinitionId id, CancellationToken cancellationToken = default);
    Task<TileConditionDefinition> GetTileConditionAsync(TileConditionDefinitionId id, CancellationToken cancellationToken = default);
    Task<GlobalConditionDefinition> GetGlobalConditionAsync(GlobalConditionDefinitionId id, CancellationToken cancellationToken = default);
}