using System.Globalization;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AshenWar.Infrastructure.Persistence.Mongo.Serializers;

public sealed class DateTimeOffsetSerializer : SerializerBase<DateTimeOffset>
{
    public override DateTimeOffset Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var str = context.Reader.ReadString();
        return DateTimeOffset.Parse(str, null, DateTimeStyles.RoundtripKind);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value)
    {
        context.Writer.WriteString(value.ToString("O")); // ISO 8601 round-trip format
    }
}