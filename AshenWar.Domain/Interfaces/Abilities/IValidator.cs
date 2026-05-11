using Domain.Entities.Actions;

namespace Domain.Interfaces.Abilities;

public interface IValidator
{
    bool Check(ActionContext context);
}