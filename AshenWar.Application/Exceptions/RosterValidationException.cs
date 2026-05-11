using System;
using System.Collections.Generic;
using Application.Enums;

namespace Application.Exceptions;

public sealed class RosterValidationException(IReadOnlyList<RosterFailure> failures) : Exception("Roster validation failed.")
{
    public IReadOnlyList<RosterFailure> Failures { get; } = failures;
}