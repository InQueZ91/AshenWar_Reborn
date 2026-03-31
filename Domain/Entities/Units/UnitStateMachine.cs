using System;
using Domain.Enums;

namespace Domain.Entities.Units;

public static class UnitStateMachine
{
    public static bool CanTransition(UnitState from, UnitState to)
        => (from, to) switch
        {
            (UnitState.Ready,    UnitState.Exhausted)  => true,
            (UnitState.Exhausted, UnitState.Ready)    => true,

            // Allow anything -> Dead (optional rule)
            (_, UnitState.Dead) => true,

            // Same-state is allowed as "no-op"
            _ when from == to => true,

            _ => false
        };

    public static void EnsureCanTransition(UnitState from, UnitState to)
    {
        if (!CanTransition(from, to))
            throw new InvalidOperationException($"Invalid transition: {from} -> {to}");
    }
}