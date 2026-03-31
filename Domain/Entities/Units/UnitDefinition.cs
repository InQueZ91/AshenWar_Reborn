using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Unit;

public sealed class UnitDefinition : Entity
{
    public UnitDefinitionId Id { get; private set; }
    public VisualId VisualId { get; private set; }
    public string Name { get; private set; }
    public UnitStats BaseStats { get; private set; }

    // Constructor
    private UnitDefinition(VisualId visualId, string name, UnitStats baseStats)
    {
        Id = UnitDefinitionId.New();
        VisualId = visualId;
        Name = name;
        BaseStats = baseStats;
    }
    public static UnitDefinition Create(VisualId visualId, string name, UnitStats baseStats)
    {
        return new UnitDefinition(visualId, name, baseStats);
    }
}