using System.Text.Json.Serialization;
using OSPhoto.Web.Converters;

namespace OSPhoto.Web.ViewModels
{
    [JsonConverter(typeof(ItemBaseViewModelConverter))]
    public class ItemBaseViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}