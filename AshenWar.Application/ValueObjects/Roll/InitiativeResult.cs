using System.Collections.Generic;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Orders;

namespace AshenWar.Application.ValueObjects.Roll;

public record InitiativeResult(TurnInitiative Initiative, List<UnitOrder> OrderedUnits);