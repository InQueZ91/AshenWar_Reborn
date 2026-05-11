using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;
using MediatR;

namespace Application.Commands.Matches.CreateMatch;

public sealed record CreateMatchRequest(
    MatchDefinitionId MatchDefinitionId,
    MapDefinitionId MapDefinitionId,
    UserId BlueUserId,
    IReadOnlyList<UnitDefinitionId> BlueRoster,
    UserId RedUserId,
    IReadOnlyList<UnitDefinitionId> RedRoster
) : IRequest<MatchId>;