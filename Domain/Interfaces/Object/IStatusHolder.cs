using System;
using Domain.Entities.Effects.Status;

namespace Domain.Interfaces.Object;

public interface IStatusHolder
{
    void AddStatus(StatusEffect status);
    void RemoveStatus(Guid statusId);
}