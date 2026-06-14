using System.Collections.Generic;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Actions;

public sealed record ActionContext(
    ITargetable Source,
    IReadOnlyList<ITargetable> Targets,
    IReadOnlySet<EntityTag> AbilityTags,
    MatchContext MatchContext)
{
    public ITargetable Source { get; } = Source;
    public MatchContext MatchContext { get; } = MatchContext;
}