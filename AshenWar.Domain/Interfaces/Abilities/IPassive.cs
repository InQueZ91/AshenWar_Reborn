using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Interfaces.Abilities;

public interface IPassive : IHasPassives
{
    void AddPassive(Passive ability);
    void RemovePassive(PassiveId passiveId);
}