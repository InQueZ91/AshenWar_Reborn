using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Infrastructure.Definitions;
using AshenWar.Infrastructure.Definitions.Repositories;
using AshenWar.Infrastructure.JWT;
using AshenWar.Infrastructure.Persistence.Mongo;
using AshenWar.Infrastructure.Persistence.Mongo.Serializers;
using AshenWar.Infrastructure.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AshenWar.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddJwt(config);
        services.AddPostgres(config);
        services.AddDefinitions();
        services.AddMongo(config);
    }

    private static void AddJwt(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection(nameof(JwtSettings)));
        services.AddSingleton<ITokenService, TokenService>();
    }

    private static void AddMongo(this IServiceCollection services, IConfiguration config)
    {
        MongoSerializerConfig.Register();
        
        var connectionString = config.GetConnectionString("MongoDB")
                               ?? throw new InvalidOperationException("MongoDB connection string not configured.");

        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var database = client.GetDatabase(mongoUrl.DatabaseName ?? "ashenwar");

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton(database);
        services.AddSingleton<MatchMapper>();
        
        services.AddScoped<IMatchRepository, MongoMatchRepository>();
        services.AddScoped<IMatchHistoryRepository, NullMongoMatchHistoryRepository>();
    }

    private static void AddPostgres(this IServiceCollection services, IConfiguration config)
    {
        var connectionString =
            config.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string not configured.");
            
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            
        // Repositories - scoped because they touch the database
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<IRefreshTokenRepository, PostgresRefreshTokenRepository>();
    }

    private static void AddDefinitions(this IServiceCollection services)
    {
        // DefinitionStore is a singleton cache - life for the app lifetime
        services.AddSingleton<DefinitionStore>();
        services.AddSingleton<DefinitionLoader>();
        
        // InMemory repositories wrap the singleton store
        services.AddSingleton<IUnitDefinitionRepository, InMemoryUnitDefinitionRepository>();
        services.AddSingleton<ITileDefinitionRepository, InMemoryTileDefinitionRepository>();
        services.AddSingleton<IConditionDefinitionRepository, InMemoryConditionDefinitionRepository>();
        services.AddSingleton<IAbilityDefinitionRepository, InMemoryAbilityDefinitionRepository>();
        services.AddSingleton<IPassiveDefinitionRepository, InMemoryPassiveDefinitionRepository>();
        services.AddSingleton<IEffectDefinitionRepository, InMemoryEffectDefinitionRepository>();
        services.AddSingleton<IMapDefinitionRepository, InMemoryMapDefinitionRepository>();
        services.AddSingleton<IMatchDefinitionRepository, InMemoryMatchDefinitionRepository>();
    }
}