using System.Collections.Generic;
using Domain.Entities.Modifiers;

namespace Domain.Interfaces.Entities;

public interface IModifierProvider
{
    IEnumerable<(ModifierDefinition Definition, int Stacks)> GetModifierEntries();
}