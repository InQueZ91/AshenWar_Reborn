using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Enums;
using AshenWar.Application.Exceptions;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.Validators;

public sealed class DeploymentValidator(
    IUnitDefinitionRepository unitDefinitionRepository,
    IMatchDefinitionRepository matchDefinitionRepository,
    IMapDefinitionRepository mapDefinitionRepository)
{
    public async Task Validate(
        IReadOnlyList<DeploymentPlan> deploymentPlans,
        MatchDefinitionId matchDefinitionId,
        MapDefinitionId mapDefinitionId, 
        PlayerSide playerSide,
        CancellationToken ct)
    {
        var unitDefinitionIds = deploymentPlans.Select(u => u.UnitDefinitionId).ToList();
        await ValidateRoster(unitDefinitionIds, matchDefinitionId, ct);
        
        var deploymentPositions = deploymentPlans.Select(u => u.Position).ToList();
        await ValidateDeployment(deploymentPositions, mapDefinitionId, playerSide, ct);
    }
    
    private async Task ValidateRoster(
        IReadOnlyList<UnitDefinitionId> unitDefinitionIds,
        MatchDefinitionId matchDefinitionId,
        CancellationToken ct)
    {
        var failures = new List<RosterFailure>();
        var unitDefinitions = await unitDefinitionRepository.GetManyAsync(unitDefinitionIds, ct);
        var matchDefinition = await matchDefinitionRepository.FindAsync(matchDefinitionId, ct);

        var sumOfUnitPower = unitDefinitions.Select(u => u.BaseStats.Get(StatDefinition.Power)).Sum();
        if (sumOfUnitPower > matchDefinition.PowerLimit) 
            failures.Add(RosterFailure.PowerLimitExceeded);

        var blacklistSet = matchDefinition.BlacklistedUnits.ToHashSet();
        if (unitDefinitionIds.Any(blacklistSet.Contains))
            failures.Add(RosterFailure.UnitOnBlacklist);
        
        if (failures.Count > 0)
            throw new RosterValidationException(failures);
    }

    private async Task ValidateDeployment(
        IReadOnlyList<HexCoord> deploymentPositions,
        MapDefinitionId mapDefinitionId, 
        PlayerSide playerSide,
        CancellationToken ct)
    {
        var mapDefinition = await mapDefinitionRepository.FindAsync(mapDefinitionId, ct);
        
        var failures = new List<DeploymentFailure>();

        if (deploymentPositions.Count != deploymentPositions.Distinct().Count())
            failures.Add(DeploymentFailure.OverlapDeployment);

        if (!mapDefinition.DeploymentPoints[playerSide].Any(deploymentPositions.Contains))
            failures.Add(DeploymentFailure.OutOfDeploymentArea);
        
        if (failures.Count > 0)
            throw new DeploymentValidatorException(failures);
    }
}