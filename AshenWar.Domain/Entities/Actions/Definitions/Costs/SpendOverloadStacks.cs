using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Definitions.Costs;

public record SpendOverloadStacks(int Stacks) : ICostActionDefinition;