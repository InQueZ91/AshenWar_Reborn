using System.Collections.Generic;
using Domain.Interfaces.Effect;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Abilities;

public class AbilityDefinition
{
    // Identifier
    public AbilityDefinitionId Id { get; }
    public string Name { get; private set;}
    
    // References
    public List<IEffect> Effects { get; private set; }= new List<IEffect>();
    
    // Properties
    public AbilityStats Stats { get; private set; }
    
    // Ability steps
    
    private AbilityDefinition (
        string name,
        IEnumerable<IEffect> effects,
        AbilityStats stats)
    {
        Id = AbilityDefinitionId.New();
        Name = name;
        Effects.AddRange(effects);
        Stats = stats;
    }

    public static AbilityDefinition Create(
        string name,
        IEnumerable<IEffect> effects,
        AbilityStats stats)
    {
        return new AbilityDefinition(name, effects, stats);
    }
    
    // Methods
    public void ChangeName(string newName) => Name = newName;
    
    public void AddEffect(IEffect effect) => Effects.Add(effect);
    public void RemoveEffect(EffectId effectId) => Effects.RemoveAll(e => e.Id == effectId);
}