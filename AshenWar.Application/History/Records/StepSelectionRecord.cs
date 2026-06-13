using System;
using AshenWar.Domain.Enums;

namespace AshenWar.Application.History.Records;

public sealed record StepSelectionRecord(Guid AbilityStepId, TargetType Type, Guid? TargetId);