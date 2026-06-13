using System;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Application.History.Records;

public sealed record DeploymentRecord(Guid UnitDefinitionId, Guid OwnerId, HexCoord Position);