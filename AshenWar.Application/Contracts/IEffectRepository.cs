using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Effects;
using Domain.ValueObjects.Identifiers.Effects;

namespace Application.Contracts;

public interface IEffectRepository
{
    Task<EffectDefinition> GetEffectAsync(EffectDefinitionId id, CancellationToken cancellationToken = default);
}