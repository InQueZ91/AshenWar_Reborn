using System.Numerics;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Unit;

public sealed class Unit
{
    // References
    public PlayerId Owner { get; private set; }
    public Vector2 Coord { get; private set; }
    public string Name { get; private set; }
    
    // Stats
    public UnitStats BaseStats { get; private set;}
    public int Power { get; private set;}
    public int Health { get; private set;}
    public int Stamina { get; private set;}
    public int Steps { get; private set;}
    public int Speed { get; private set;}
    public int Vision { get; private set;}

    // State
    public UnitState State { get; private set; }

    private Unit(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
    {
        // References
        Owner = owner;
        Coord = coord;
        Name = name;
        
        // Stats
        BaseStats = baseStats;
        Power = baseStats.Power;
        Health = baseStats.Health;
        Stamina = baseStats.Stamina;
        Steps = baseStats.Steps;
        Speed = baseStats.Speed;
        Vision = baseStats.Vision;
    }
    public static Unit Create(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
    {
        return new Unit(owner, coord, name, baseStats);
    }
}