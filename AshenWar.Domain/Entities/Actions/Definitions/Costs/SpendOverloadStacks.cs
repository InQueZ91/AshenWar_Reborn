using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Costs;

public sealed record SpendOverloadStacks(int Stacks) : ICostActionDefinition;