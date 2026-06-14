namespace AshenWar.Infrastructure.Definitions.Dtos.Maps;

public sealed record GlobalEventDto(int TurnNumber, int Stacks, List<Guid> Pool);