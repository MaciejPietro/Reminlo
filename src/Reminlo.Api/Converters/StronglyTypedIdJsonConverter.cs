using Reminlo.Domain.Common;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reminlo.Api.Converters;

public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsValueType &&
               typeToConvert.GetInterfaces().Contains(typeof(IGuid));
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(StronglyTypedIdJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

public class StronglyTypedIdJsonConverter<T> : JsonConverter<T> where T : struct, IGuid
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var guidString = reader.GetString();
            if (Guid.TryParse(guidString, out var guid))
            {
                // Use reflection to find and invoke the constructor
                var constructor = typeToConvert.GetConstructor(new[] { typeof(Guid) });
                if (constructor != null)
                {
                    return (T)constructor.Invoke(new object[] { guid });
                }
            }
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Cannot convert null to {typeToConvert.Name}");
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert.Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Get the Value property which contains the Guid
        var valueProperty = typeof(T).GetProperty("Value");
        if (valueProperty != null)
        {
            var guid = (Guid)valueProperty.GetValue(value)!;
            writer.WriteStringValue(guid.ToString());
        }
        else
        {
            throw new JsonException($"Type {typeof(T).Name} does not have a Value property");
        }
    }
}
