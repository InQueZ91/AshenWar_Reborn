using System;
using System.Collections.Generic;
using AshenWar.Application.Enums;

namespace AshenWar.Application.Exceptions;

public sealed class RosterValidationException(IReadOnlyList<RosterFailure> failures) : Exception("Roster validation failed.")
{
    public IReadOnlyList<RosterFailure> Failures { get; } = failures;
}