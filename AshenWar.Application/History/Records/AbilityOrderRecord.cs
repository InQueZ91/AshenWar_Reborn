using System;
using System.Collections.Generic;

namespace AshenWar.Application.History.Records;

public sealed record AbilityOrderRecord
{
    public Guid AbilityId { get; init; }
    public IReadOnlyList<StepSelectionRecord> StepSelections { get; init; } = [];   
}