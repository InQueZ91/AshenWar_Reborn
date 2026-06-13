using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Interfaces.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace AshenWar.Application.Execution;

public sealed class ActionHandlerRegistry(IServiceScopeFactory scopeFactory)
{
    public (IActionHandler<T> Handler, IServiceScope Scope) Get<T>() where T : IActionDefinition
    {
        var scope = scopeFactory.CreateScope();
        return (scope.ServiceProvider.GetRequiredService<IActionHandler<T>>(), scope);
    }
}