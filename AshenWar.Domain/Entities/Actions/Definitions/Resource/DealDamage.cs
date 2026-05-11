using Domain.Entities.Actions.ValueSources;
using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Definitions.Resource;

public sealed record DealDamage(ValueSource Amount) : IActionDefinition;