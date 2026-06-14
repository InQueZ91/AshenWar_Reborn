using System.Threading.Channels;
using AshenWar.Application.Auth;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Application.Execution;
using AshenWar.Application.History;
using AshenWar.Application.Planning;
using AshenWar.Application.Services;
using AshenWar.Application.Translation;
using AshenWar.Application.Validators;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using Microsoft.Extensions.DependencyInjection;

namespace AshenWar.Application;

public static class ApplicationServicesRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssembly(
                typeof(ApplicationServicesRegistration).Assembly));
        
        // Services - explicit, intentional
        services.AddScoped<AuthService>();
        services.AddScoped<UnitSpawnService>();
        services.AddScoped<InitiativeService>();
        services.AddScoped<ResolutionService>();
        services.AddScoped<PlanningService>();
        
        services.AddSingleton<LobbyService>();
        
        // Execution pipeline - explicit
        services.AddScoped<ActionExecutor>();
        services.AddScoped<AbilityExecutor>();
        services.AddScoped<ConditionExecutor<GlobalCondition>>();
        services.AddScoped<ConditionExecutor<TileCondition>>();
        services.AddScoped<ConditionExecutor<UnitCondition>>();
        services.AddScoped<EffectExecutor>();
        services.AddScoped<PassiveTriggerExecutor>();
        services.AddScoped<TriggerChain>();
        services.AddScoped<ResolutionEventTranslatorRegistry>();
        
        services.AddSingleton<ActionHandlerRegistry>();
        
        // Validator
        services.AddScoped<DeploymentValidator>();
        
        // Match History
        services.AddScoped<MatchHistoryFactory>();
        services.AddScoped<TurnRecordFactory>();

        // Action handlers - scanned automatically
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(ApplicationServicesRegistration))
            .AddClasses(classes => classes
                .AssignableTo(typeof(IActionHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        // Timer channel
        var timerChannel = Channel.CreateUnbounded<TimerMessage>();
        services.AddSingleton(timerChannel.Writer);
        services.AddSingleton(timerChannel.Reader);

        // Background services
        services.AddHostedService<PlanningTimerService>();
    }
}