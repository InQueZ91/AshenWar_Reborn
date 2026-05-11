using Microsoft.Extensions.DependencyInjection;

namespace Application;

// ApplicationServicesRegistration.cs
public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR for application commands
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssembly(
                typeof(ApplicationServicesRegistration).Assembly));

        // Register all action handlers
        // ...
        
        // services.AddSingleton<ActionHandlerRegistry>();
        // services.AddSingleton<ActionExecutor>();

        return services;
    }
}