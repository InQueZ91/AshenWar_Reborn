using System.Text.Json;
using System.Text.Json.Serialization;
using AshenWar.Domain.ValueObjects.Identifiers;

namespace AshenWar.Infrastructure.Definitions.Converters;

public sealed class IdConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.BaseType is { IsGenericType: true } baseType 
           && baseType.GetGenericTypeDefinition() == typeof(Id<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(IdConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}