using System;
using System.Collections.Generic;
using System.Linq;
using Application.ValueObjects.Roll;
using Domain.Entities.Match;
using Domain.Entities.Stats;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Units;

namespace Application;

public sealed class InitiativeService
{
    public InitiativeResult Build(
        PlayerOrders blue,
        PlayerOrders red,
        Match match,
        int turnSeed,
        Random rng)
    {
        var unitOrders = blue.Orders.Concat(red.Orders).ToList();

        var unitSpeed = unitOrders.ToDictionary(
            o => o.UnitId,
            o => match.GetUnitById(o.UnitId)!.GetMaxStat(StatDefinition.Speed));

        var rounds = new List<InitiativeRound>();

        var finalOrder = unitOrders
            .OrderByDescending(o => unitSpeed[o.UnitId])
            .GroupBy(o => unitSpeed[o.UnitId])
            .OrderByDescending(g => g.Key)
            .SelectMany(g =>
            {
                if (g.Count() == 1)
                    return g.AsEnumerable();

                var resolution = Roll(g.ToList(), unitSpeed, rng, 1);
                rounds.AddRange(resolution.Rounds);
                return resolution.FinalOrder;
            }).ToList();

        var initiative = new TurnInitiative(
            match.CurrentTurn.Id,
            turnSeed,
            rounds,
            finalOrder.Select(o => o.UnitId).ToList()
        );

        return new InitiativeResult(initiative, finalOrder);
    }

    private InitiativeResolution Roll(
        List<UnitOrder> tied,
        Dictionary<UnitId, int> unitSpeed,
        Random rng,
        int round)
    {
        var rounds = new List<InitiativeRound>();

        if (round >= 5)
            return new InitiativeResolution(tied.OrderBy(o => o.UnitId.Value).ToList(), rounds);

        var range = Math.Max(2, 20 - (round * 4));

        var rolls = tied
            .Select(o => (Order: o, Roll: rng.Next(1, range + 1)))
            .Select(r => (r.Order, r.Roll, Total: unitSpeed[r.Order.UnitId] + r.Roll))
            .ToList();

        // record this round
        rounds.Add(new InitiativeRound(
            round,
            new RollRange(1, range),
            rolls.Select(r => new UnitRoll(r.Order.UnitId, unitSpeed[r.Order.UnitId], r.Roll, r.Total)).ToList()
        ));

        var order = rolls
            .GroupBy(r => r.Total)
            .OrderByDescending(g => g.Key)
            .SelectMany(g =>
            {
                if (g.Count() == 1)
                    return g.Select(r => r.Order);

                var resolution = Roll(g.Select(r => r.Order).ToList(), unitSpeed, rng, round + 1);
                rounds.AddRange(resolution.Rounds);

                return resolution.FinalOrder;
            }).ToList();

        return new InitiativeResolution(order, rounds);
    }
}