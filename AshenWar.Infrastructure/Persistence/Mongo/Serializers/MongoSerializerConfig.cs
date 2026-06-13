using AshenWar.Domain.ValueObjects;
using AshenWar.Infrastructure.Definitions.Dtos.Maps;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace AshenWar.Infrastructure.Persistence.Mongo.Serializers;

public static class MongoSerializerConfig
{
    public static void Register()
    {
        // Convention — ignore extra fields on all types
        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("AshenWarConventions", pack, _ => true);
        
        // Scalar serializers
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer());
        
        // HexCoord — positional record, explicit creator required
        BsonClassMap.RegisterClassMap<HexCoord>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(h => new HexCoord(h.Q, h.R));
        });
        
        // GlobalEventDto — positional record, explicit creator required
        BsonClassMap.RegisterClassMap<GlobalEventDto>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(g => new GlobalEventDto(g.TurnNumber, g.Stacks, g.Pool));
        });
        
        // All Mongo DTOs — sealed records with init properties, must be explicit in driver v3
        BsonClassMap.RegisterClassMap<MatchDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<PlayerDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<BoardDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<TurnDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<SpawnPointsDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<PlayerOrdersDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<UnitDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<TileDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<UnitOrderDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<StepSelectionDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<ConditionDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<AbilityDto>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<PassiveDto>(cm => cm.AutoMap());
    }
}