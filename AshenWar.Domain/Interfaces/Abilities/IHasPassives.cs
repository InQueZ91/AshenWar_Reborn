using System.Collections.Generic;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Interfaces.Abilities;

public interface IHasPassives
{
    IReadOnlyList<Passive> Passives { get; }
    Passive? GetPassiveByDefinitionId(PassiveDefinitionId definitionId);
}