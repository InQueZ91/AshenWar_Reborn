using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Infrastructure.Definitions.Dtos.Conditions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UnitConditionDefinitionDto), "UnitCondition")]
[JsonDerivedType(typeof(TileConditionDefinitionDto), "TileCondition")]
[JsonDerivedType(typeof(GlobalConditionDefinitionDto), "GlobalCondition")]
public abstract record ConditionDefinitionBaseDto
{
    public string Name { get; init; } = "";
    public int BaseDuration { get; init; }
    public IStackingBehavior? StackingBehavior { get; init; }
    public int MaxStacks { get; init; }
    public ITargetFilter? Filter { get; init; }
    public IValidator? Guard { get; init; }
    public IEnumerable<ConditionTag> ConditionTags { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
    public IEnumerable<ConditionEffectDto> OnApply { get; init; } = [];
    public IEnumerable<ConditionEffectDto> OnTick { get; init; } = [];
    public IEnumerable<ConditionEffectDto> OnExpire { get; init; } = [];

    public abstract ConditionDefinitionBase ToDomain();

    protected void ApplyBase(ConditionDefinitionBase definition)
    {
        definition.SetBaseDuration(BaseDuration);
        if (StackingBehavior != null) definition.SetStackingBehavior(StackingBehavior);
        if (MaxStacks > 0) definition.SetMaxStacks(MaxStacks);
        if (Filter is not null) definition.SetFilter(Filter);
        if (Guard is not null) definition.SetGuard(Guard);
        
        foreach (var tag in ConditionTags) definition.AddConditionTag(tag);
        foreach (var tag in Tags) definition.AddTag(EntityTag.FromName(tag));
        foreach (var effect in OnApply) definition.AddOnApply(BuildConditionEffect(effect));
        foreach (var effect in OnTick) definition.AddOnTick(BuildConditionEffect(effect));
        foreach (var effect in OnExpire) definition.AddOnExpire(BuildConditionEffect(effect));
    }
    
    private ConditionEffect BuildConditionEffect(ConditionEffectDto dto)
    {
        var conditionEffect = ConditionEffect.Create();
        conditionEffect.SetFilter(dto.Filter);
        conditionEffect.SetGuard(dto.Guard);
        foreach (var action in dto.Actions) conditionEffect.AddAction(action);
        
        return conditionEffect;
    }
}