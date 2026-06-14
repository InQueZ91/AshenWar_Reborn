using System.Collections.Generic;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.Entities.Lobbies;

public sealed class PlayerSlot
{
    private readonly List<DeploymentPlan> _deploymentPlans = [];
    
    public UserId UserId { get; }
    public PlayerSide Side { get; }
    public IReadOnlyList<DeploymentPlan> DeploymentPlans => _deploymentPlans;
    public bool HasSubmitted { get; private set; }

    private PlayerSlot(UserId userId, PlayerSide side)
    {
        UserId = userId;
        Side = side;
    }
    
    public static PlayerSlot Create(UserId userId, PlayerSide side) => new (userId, side);
    
    public void SubmitDeployment(List<DeploymentPlan> deploymentPlans)
    {
        if (HasSubmitted)
            throw new DomainException("Player has already submitted a deployment plan, cancel submission first.");
        
        if (deploymentPlans.Count == 0)
            throw new DomainException("Player must have at least one deployment plan");
        
        _deploymentPlans.Clear();
        _deploymentPlans.AddRange(deploymentPlans);
        
        HasSubmitted = true;
    }
    
    public void CancelDeployment()
    {
        if (!HasSubmitted)
            throw new DomainException("Player has not submitted a deployment plan.");
        
        _deploymentPlans.Clear();
        HasSubmitted = false;
    }
}