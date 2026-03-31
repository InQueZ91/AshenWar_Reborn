using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Object;

public interface IStatHolder<T> where T : Enum 
{
    int GetStat(T stat);
    int GetBaseStat(T stat);
}