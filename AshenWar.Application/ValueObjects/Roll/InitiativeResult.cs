using System.Collections.Generic;
using Domain.ValueObjects;

namespace Application.ValueObjects.Roll;

public record InitiativeResult(TurnInitiative Initiative, List<UnitOrder> OrderedUnits);