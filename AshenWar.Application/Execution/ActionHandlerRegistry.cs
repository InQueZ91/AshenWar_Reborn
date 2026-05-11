using System;
using Application.Interfaces;
using Domain.Interfaces.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Execution;

public sealed class ActionHandlerRegistry(IServiceProvider sp)
{
    public IActionHandler<T> Get<T>() where T : IActionDefinition
        => sp.GetRequiredService<IActionHandler<T>>();
}