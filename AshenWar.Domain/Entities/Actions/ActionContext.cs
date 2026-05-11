using System.Collections.Generic;
using Domain.Entities.Match;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Actions;

public sealed record ActionContext(
    ITargetable Source,
    IReadOnlyList<ITargetable> Targets,
    IReadOnlySet<EntityTag> AbilityTags,
    MatchContext MatchContext)
{
    public ITargetable Source { get; } = Source;
    public MatchContext MatchContext { get; } = MatchContext;
}