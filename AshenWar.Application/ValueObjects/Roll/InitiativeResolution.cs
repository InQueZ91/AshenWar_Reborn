using System.Collections.Generic;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Orders;

namespace AshenWar.Application.ValueObjects.Roll;

public record InitiativeResolution(List<UnitOrder> FinalOrder, List<InitiativeRound> Rounds);