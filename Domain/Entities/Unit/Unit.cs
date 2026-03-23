using System;
using System.Numerics;
using Domain.Entities.Unit.Events;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.Unit
{
    public sealed class Unit : Entity
    {
        // Identifiers
        public UnitId Id { get; private set; }
        public string Name { get; private set; }
        
        // References
        public PlayerId Owner { get; private set; }
        public Vector2 Coord { get; private set; }
        
        // Definitions
        public UnitStats BaseStats { get; private set; }

        #region Runtime values

        public int Power { get; private set; }
        public int Health { get; private set; }
        public int Stamina { get; private set; }
        public int Steps { get; private set; }
        public int Vision { get; private set; }
        public int Speed { get; private set; }
        public UnitState State { get; private set; }

        #endregion
        
        // Constructor
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
        public static Unit Create(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
        {
            var unit = new Unit(owner, coord, name, baseStats);

            unit.RaiseDomainEvent(new UnitCreated(unit));
            return unit;
        }
        
        #region States

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

        #endregion
        
        #region Actions

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
            // Validation
            if (amount <= 0) return;
            
            // Action
            var previousHealth = Health;
            Health = Math.Max(Health - amount, 0);
            
            // Raise events
            RaiseDomainEvent(new UnitDamaged(Id, Health, BaseStats.Health));
            
            // Transition to dead if health is zero
            if (previousHealth > 0 && Health == 0)
            {
                TransitionTo(UnitState.Dead, () => RaiseDomainEvent(new UnitDied(Id)));
            }
        }
        public void MoveTo(Vector2 newCoord)
        {
            // Validation
            if (Coord == newCoord) return;

            // Action
            var from = Coord;
            Coord = newCoord;
            
            // Raise events
            RaiseDomainEvent(new UnitMoved(Id, from, Coord));
        }

        #endregion
    }
}