using System.Collections.Generic;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.ValueObjects;

public sealed record GlobalEvent(int TurnNumber, int Stacks, IReadOnlyList<GlobalConditionDefinitionId> Pool);