using System;
using System.Numerics;
using Domain.Entities.Unit.Events;
using Domain.ValueObjects;

namespace Domain.Entities.Unit
{
    public sealed class Unit : Entity
    {
        public UnitId Id { get; private set; }
        
        // Identifiers
        public PlayerId Owner { get; private set; }
        public Vector2 Coord { get; private set; }
        public string Name { get; private set; }
        
        // Stats
        public UnitStats BaseStats { get; private set; }

        #region Runtime Values

        public int Power { get; private set; }
        public int Health { get; private set; }
        public int Stamina { get; private set; }
        public int Steps { get; private set; }
        public int Vision { get; private set; }
        public int Speed { get; private set; }
        public UnitState State { get; private set; }

        #endregion

        private Unit(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
        {
            Id = UnitId.New();
            Owner = owner;
            Coord = coord;
            Name = name;
            BaseStats = baseStats;
            
            // Initialize runtime values
            Power = baseStats.Power;
            Health = baseStats.Health;
            Stamina = baseStats.Stamina;
            Steps = baseStats.Steps;
            Vision = baseStats.Vision;
            Speed = baseStats.Speed;
            State = UnitState.Ready;
        }

        public static Unit Create(
            PlayerId owner,
            Vector2 coord,
            string name,
            UnitStats baseStats)
        {
            var unit = new Unit(owner, coord, name, baseStats);

            unit.RaiseDomainEvent(new UnitCreated(unit));
            return unit;
        }

        // States
        public void Ready()
        {
            TransitionTo(UnitState.Ready, () => RaiseDomainEvent(new UnitReady(Id)));            
        }
        public void Exhaust()
        {
            TransitionTo(UnitState.Exhausted, () => RaiseDomainEvent(new UnitExhausted(Id)));
        }
        private void TransitionTo(UnitState target, Action onChanged)
        {
            if (State == UnitState.Dead) return;
            if (State == target) return;
            
            UnitStateMachine.EnsureCanTransition(State, target);
            
            State = target;

            onChanged?.Invoke();
        }
        
        public void GainStamina(int amount)
        {
            Stamina = Math.Min(Stamina + amount, BaseStats.Stamina);

            RaiseDomainEvent(new UnitStaminaUpdated(Id, Stamina, BaseStats.Stamina));
        }
        public void ConsumeStamina(int amount)
        {
            Stamina = Math.Max(Stamina - amount, 0);
            
            RaiseDomainEvent(new UnitStaminaUpdated(Id, Stamina, BaseStats.Stamina));
        }
        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            
            var previous = Health;
            
            Health = Math.Max(Health - amount, 0);
            
            RaiseDomainEvent(new UnitDamaged(Id, Health, BaseStats.Health));

            if (previous > 0 && Health == 0)
            {
                TransitionTo(UnitState.Dead, () => RaiseDomainEvent(new UnitDied(Id)));
            }
        }
        
        // Movement
        public void MoveTo(Vector2 newCoord)
        {
            if (Coord == newCoord) return;

            var from = Coord;
            Coord = newCoord;
            
            RaiseDomainEvent(new UnitMoved(Id, from, Coord));
        }
    }
}