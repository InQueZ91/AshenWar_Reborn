using System.Collections.Generic;
using Domain.Enums;

namespace Domain.ValueObjects
{
    public sealed record UnitStats
    {
        public int Power;
        public int Health;
        public int Stamina;
        public int Steps;
        public int Speed;
        public int Vision;
    }
}