using System;
using System.Collections.Generic;
using AshenWar.Application.Enums;

namespace AshenWar.Application.Exceptions;

public sealed class DeploymentValidatorException(IReadOnlyList<DeploymentFailure> failures) : Exception("Deployment validation failed.")
{
    public IReadOnlyList<DeploymentFailure> Failures { get; } = failures;
}