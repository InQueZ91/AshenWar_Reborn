using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Abilities.Active;
using Domain.Entities.Abilities.Passive;
using Domain.ValueObjects.Identifiers.Abilities;

namespace Application.Contracts;

public interface IAbilityRepository
{
    Task<ActiveAbilityDefinition> GetActiveAbilityAsync(ActiveAbilityDefinitionId id, CancellationToken cancellationToken = default);
    Task<PassiveAbilityDefinition> GetPassiveAbilityAsync(PassiveAbilityDefinitionId id, CancellationToken cancellationToken = default);
}