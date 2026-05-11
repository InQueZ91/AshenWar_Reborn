using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.ValueObjects;

public sealed record HexCoord(int Q, int R)
{
    public int S => -Q - R;
    
    public static readonly HexCoord Zero = new(0, 0);

    private static readonly HexCoord[] Directions =
    [
        new (1, 0), new  (1, -1), new (0,-1),
        new (-1, 0), new (-1, 1), new (0, 1)
    ];

    public HexCoord Neighbor(int direction) 
        => this + Directions[direction % 6];

    public IEnumerable<HexCoord> Neighbors() 
        => Directions.Select(d => this + d);

    public int DistanceTo(HexCoord other) 
        => (Math.Abs(Q - other.Q) + Math.Abs(R - other.R) + Math.Abs(S - other.S)) / 2;
    
    public bool IsAdjacentTo(HexCoord other)
        => DistanceTo(other) == 1;
    
    public static HexCoord operator +(HexCoord a, HexCoord b)
        => new(a.Q + b.Q, a.R + b.R);
    
    public static HexCoord operator -(HexCoord a, HexCoord b)
        => new(a.Q - b.Q, a.R - b.R);
    
    public override string ToString() => $"({Q}, {R}, {S})";
}