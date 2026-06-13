using System;
using System.Collections.Generic;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Orders;

namespace AshenWar.Domain.Entities.Match;

public sealed class PlayerOrders
{
    private readonly List<UnitOrder> _orders = [];
    
    public UserId Owner { get; }
    public IReadOnlyList<UnitOrder> Orders => _orders.AsReadOnly();
    
    private PlayerOrders(UserId owner) => Owner = owner;
    public static PlayerOrders Create(UserId owner, IEnumerable<UnitOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(owner);
        
        var playerOrders = new PlayerOrders(owner);
        foreach (var order in orders)
            playerOrders.AddOrder(order);
        
        return playerOrders;
    }
    public static PlayerOrders Empty(UserId owner) => new(owner);

    public void AddOrder(UnitOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _orders.Add(order);
    }
}