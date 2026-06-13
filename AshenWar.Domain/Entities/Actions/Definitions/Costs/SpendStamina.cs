using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Costs;

public sealed record SpendStamina(int Amount) : ICostActionDefinition;