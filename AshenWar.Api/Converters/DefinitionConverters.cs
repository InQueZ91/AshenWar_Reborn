using AshenWar.Api.Models.Responses;
using AshenWar.Api.Models.Responses.Definitions.Abilities;
using AshenWar.Api.Models.Responses.Definitions.Tiles;
using AshenWar.Api.Models.Responses.Definitions.Units;
using AshenWar.Domain.Entities.Costs;
using AshenWar.Domain.Entities.Stats;

namespace AshenWar.Api.Converters;

internal static class DefinitionConverters
{
    public static UnitStatsResponse ToResponse(UnitStats unitStats)
    {
        return new UnitStatsResponse
        {
            Power = unitStats.Get(StatDefinition.Power),
            Health = unitStats.Get(StatDefinition.Health),
            Stamina = unitStats.Get(StatDefinition.Stamina),
            Steps = unitStats.Get(StatDefinition.Steps),
            Speed = unitStats.Get(StatDefinition.Speed),
            Vision = unitStats.Get(StatDefinition.Vision),
        };
    }
    
    public static TileStatsResponse ToResponse(TileStats tileStats) 
        => new() { MovementCost = tileStats.Get(StatDefinition.MovementCost) };

    public static AbilityStatsResponse ToResponse(AbilityStats abilityStats)
    {
        return new AbilityStatsResponse
        {
            Damage = abilityStats.Get(StatDefinition.Damage),
            Cooldown = abilityStats.Get(StatDefinition.Cooldown),
            Range = abilityStats.Get(StatDefinition.Range),
        };
    }

    public static AbilityCostResponse ToResponse(CostDefinition cost) => cost switch
    {
        CostDefinition.Stamina s => new AbilityCostResponse("Stamina", s.Amount),
        CostDefinition.Steps s => new AbilityCostResponse("Steps", s.Amount),
        CostDefinition.Overload o => new AbilityCostResponse("Overload", o.RequiredStacks),
        // Compiler warning: switch is not exhaustive
        // no default arm needed - compiler tells you when you miss a case
    };
}