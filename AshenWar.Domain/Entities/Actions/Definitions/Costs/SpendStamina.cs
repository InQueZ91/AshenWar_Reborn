using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Definitions.Costs;

public record SpendStamina(int Amount) : ICostActionDefinition;