using System;

namespace Application.Constants;

public static class PlanningConstants
{
    public static readonly TimeSpan PlanningDuration = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan GracePeriod = TimeSpan.Zero; // acceptable deviation from planning duration
}