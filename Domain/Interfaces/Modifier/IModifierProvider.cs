using System.Collections.Generic;

namespace Domain.Interfaces.Modifier;

public interface IModifierProvider<T>
{
    IEnumerable<T> GetModifiers();
}