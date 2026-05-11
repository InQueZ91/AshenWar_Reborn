using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Planning;
using Domain.Entities.Match;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Commands.Matches.ConfirmDeploy;

public sealed class ConfirmDeploymentHandler(
    IMatchRepository matchRepository,
    UnitSpawnService unitSpawnService,
    PlanningTimerService planningTimerService)
    : IRequestHandler<ConfirmDeploymentRequest>
{
    public async Task Handle(ConfirmDeploymentRequest request, CancellationToken cancellationToken)
    {
        var match = await matchRepository.GetMatchAsync(request.MatchId, cancellationToken);
        if (match is null)
            throw new DomainException("Match not found.");

        // Validate match phase
        if (match.Phase != MatchPhase.Deploy)
            throw new DomainException("Match is not in Deploy phase.");

        // Validate player belongs to match
        if (match.BlueSide.UserId != request.UserId && match.RedSide.UserId != request.UserId)
            throw new DomainException("Player does not belong to this match.");

        // Resolve player
        var player = request.UserId == match.BlueSide.UserId ? match.BlueSide : match.RedSide;
        switch (player.Side)
        {
            case PlayerSide.Blue when match.BlueDeployConfirmed:
                throw new DomainException("Blue player has already confirmed their deploy.");
            case PlayerSide.Red when match.RedDeployConfirmed:
                throw new DomainException("Red player has already confirmed their deploy.");
        }

        // Validate roster
        var roster = player.Roster;
        
        if (request.Slots.Count != roster.Count)
            throw new DomainException("Slot count must match roster count");
        
        var slotDefinitionIds = request.Slots.Select(s => s.UnitDefinitionId).ToHashSet();
        if (!roster.All(slotDefinitionIds.Contains))
            throw new DomainException("All roster units must be in the deployment slots.");

        // Validate all slots belong to this player
        ValidateSpawnPositions(request, match, player);

        // Spawn units
        foreach (var (unitDefinitionId, position) in request.Slots)
        {
            await unitSpawnService.SpawnAsync(
                match,
                player.UserId,
                unitDefinitionId,
                position,
                cancellationToken
            );
        }

        match.ConfirmDeploy(player.Side);

        if (match.BothDeployConfirmed)
        {
            match.StartMatch();
            await planningTimerService.StartPlanningTimer(match.Id, match.CurrentTurn, cancellationToken);
        }

        await matchRepository.SaveAsync(match, cancellationToken);
        
        // Broadcast MatchStarted
    }

    private void ValidateSpawnPositions(ConfirmDeploymentRequest request, Match match, Player player)
    {
        foreach (var deploymentSlot in request.Slots)
        {
            if (!match.SpawnPoints[player.Side].Contains(deploymentSlot.Position))
                throw new DomainException(
                    $"Spawn position {deploymentSlot.Position} is not in the spawn points for player {player.Side}.");

            if (!match.Board.TryGetTile(deploymentSlot.Position, out var spawnTile))
                throw new DomainException($"Tile {deploymentSlot.Position} not found.");

            if (spawnTile.IsOccupied)
                throw new DomainException($"Tile {deploymentSlot.Position} already occupied.");

            if (spawnTile.IsDestroyed)
                throw new DomainException($"Tile {deploymentSlot.Position} is destroyed.");
        }
    }
}