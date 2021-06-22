using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OSPhoto.Web.ViewModels;

namespace OSPhoto.Web.Converters
{
    public class ItemBaseViewModelConverter : JsonConverter<ItemBaseViewModel>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ItemBaseViewModel).IsAssignableFrom(typeToConvert);
        }

        public override ItemBaseViewModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // we don't need to read the JSON
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ItemBaseViewModel value, JsonSerializerOptions options)
        {
            var opt = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            
            if (value is ImageViewModel ivm)
                JsonSerializer.Serialize(writer, ivm, opt);
            else if (value is DirectoryViewModel dvm)
                JsonSerializer.Serialize(writer, dvm, opt);
            else if (value is FileViewModel fvm)
                JsonSerializer.Serialize(writer, fvm, opt);
            else
                JsonSerializer.Serialize(writer, value, opt);
        }
    }
}