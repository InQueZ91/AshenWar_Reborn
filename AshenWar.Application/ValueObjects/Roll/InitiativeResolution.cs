using System.Collections.Generic;
using Domain.ValueObjects;

namespace Application.ValueObjects.Roll;

public record InitiativeResolution(List<UnitOrder> FinalOrder, List<InitiativeRound> Rounds);