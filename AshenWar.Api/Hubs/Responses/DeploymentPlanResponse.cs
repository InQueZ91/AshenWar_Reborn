namespace AshenWar.Api.Hubs.Responses;

public sealed record DeploymentPlanResponse(Guid UnitDefinitionId, int Q, int R);