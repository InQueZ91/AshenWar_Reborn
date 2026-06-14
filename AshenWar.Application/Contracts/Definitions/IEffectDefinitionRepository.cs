using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Effects;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;

namespace AshenWar.Application.Contracts.Definitions;

public interface IEffectDefinitionRepository
{
    Task<EffectDefinition> GetEffectAsync(EffectDefinitionId id, CancellationToken cancellationToken = default);
}