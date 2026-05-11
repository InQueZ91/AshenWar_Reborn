using Domain.Entities.Tiles;
using Domain.Entities.Units;
using Domain.ValueObjects;

namespace Domain.Interfaces.Match;

public interface IBoardCommand : IBoard
{
    // Mutation
    void PlaceTile(Tile tile, HexCoord position); // construction only
    Unit? RemoveTile(Tile tile);
    void PlaceUnit(Unit unit, HexCoord position); // spatial only
    void RemoveUnit(Unit unit); // spatial only
}