using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Resource;

public sealed record OperateStamina(ValueSource Amount) : IActionDefinition;