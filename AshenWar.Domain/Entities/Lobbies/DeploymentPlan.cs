using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Lobbies;

public sealed record DeploymentPlan(UnitDefinitionId UnitDefinitionId, HexCoord Position);