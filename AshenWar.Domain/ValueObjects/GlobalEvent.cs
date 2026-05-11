using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.ValueObjects;

public sealed record GlobalEvent(int TurnNumber, int Stacks, IReadOnlyList<GlobalConditionDefinitionId> Pool);