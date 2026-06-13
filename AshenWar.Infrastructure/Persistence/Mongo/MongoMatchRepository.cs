using AshenWar.Application.Contracts;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos;
using MongoDB.Driver;

namespace AshenWar.Infrastructure.Persistence.Mongo;

public sealed class MongoMatchRepository(IMongoDatabase database, MatchMapper mapper) : IMatchRepository
{
    private readonly IMongoCollection<MatchDto> _collection = database.GetCollection<MatchDto>("matches");
    
    public async Task<Match?> FindAsync(MatchId id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<MatchDto>.Filter.Eq(m => m.Id, id.Value);
        var dto = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (dto is null) return null;
        
        return await mapper.FromDto(dto);
    }

    public async Task<List<Match>> GetActivePlanningMatchesAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<MatchDto>.Filter.Eq(m => m.Phase, MatchPhase.Planning);
        var dtos = await _collection.Find(filter).ToListAsync(cancellationToken);
        
        var matches = await Task.WhenAll(dtos.Select(mapper.FromDto));
        return matches.ToList();
    }

    public async Task SaveAsync(Match match, CancellationToken cancellationToken = default)
    {
        var dto = mapper.ToDto(match);
        var filter = Builders<MatchDto>.Filter.Eq(m => m.Id, match.Id.Value);
        var options = new ReplaceOptions { IsUpsert = true };
        
        await _collection.ReplaceOneAsync(filter, dto, options, cancellationToken);
    }
}