using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Definitions.Costs;

public record SpendSteps(int Amount) : ICostActionDefinition;