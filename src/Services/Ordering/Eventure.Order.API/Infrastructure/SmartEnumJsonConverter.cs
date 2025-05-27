using Ardalis.SmartEnum;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eventure.Order.API.Infrastructure;

public class SmartEnumJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : SmartEnum<TEnum>
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Unexpected token parsing SmartEnum. Expected String, got {reader.TokenType}.");

        var name = reader.GetString();
        return SmartEnum<TEnum>.FromName(name!, true);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
