using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Units;

namespace Application.Commands.Matches.ConfirmDeploy;

public sealed record DeploymentSlot(UnitDefinitionId UnitDefinitionId, HexCoord Position);