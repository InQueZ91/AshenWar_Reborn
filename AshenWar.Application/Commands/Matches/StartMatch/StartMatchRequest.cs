using System.Collections.Generic;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using MediatR;

namespace AshenWar.Application.Commands.Matches.StartMatch;

public sealed record StartMatchRequest(
    MatchDefinitionId MatchDefinitionId,
    MapDefinitionId MapDefinitionId,
    UserId BlueUserId,
    IReadOnlyList<DeploymentPlan> BlueDeploymentPlan,
    UserId RedUserId,
    IReadOnlyList<DeploymentPlan> RedDeploymentPlan
) : IRequest<MatchId>;