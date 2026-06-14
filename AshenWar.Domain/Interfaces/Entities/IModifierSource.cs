using System.Collections.Generic;
using AshenWar.Domain.Entities.Modifiers;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IModifierSource
{
    IEnumerable<(Modifier Modifier, int Stacks)> GetModifiers();
}