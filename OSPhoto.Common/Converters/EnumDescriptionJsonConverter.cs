using System.Text.Json;

namespace OSPhoto.Common.Converters;

public class EnumDescriptionJsonConverter : JsonConverter<Enum>
{
    public override Enum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
