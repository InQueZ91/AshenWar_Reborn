using System.Text.Json;
using AshenWar.Domain.Entities.Actions;

namespace AshenWar.Infrastructure.Definitions.Converters.Handlers;

internal interface IValueSourceHandler
{
    ValueSource Read(JsonElement root);
    void Write(Utf8JsonWriter writer, ValueSource value);
}