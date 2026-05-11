using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;
using MediatR;

namespace Application.Commands.Matches.ConfirmDeploy;

public sealed record ConfirmDeploymentRequest(MatchId MatchId, UserId UserId, IReadOnlyList<DeploymentSlot> Slots) : IRequest;