using System.Linq;
using AshenWar.Domain.Events.Units;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;

namespace AshenWar.Domain.Entities.Abilities.Passives.Triggers.Entries;

public sealed record OnUnitDamaged : TriggerEntry
{
    public int? MinDamage { get; init; }
    public ITargetFilter? SubjectFilter { get; init; }

    public override bool Matches(IDomainEvent evt, ITargetable owner, IBoard board)
    {
        if (evt is not UnitDamaged dmg) return false;
        
        if (MinDamage.HasValue && dmg.Amount < MinDamage) return false;

        if (SubjectFilter is not null)
        {
            var subject = board.FindUnitById(dmg.UnitId);
            return subject is not null && SubjectFilter.Apply([subject], owner).Any();
        }
        
        return true;
    }
}