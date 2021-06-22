using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OSPhoto.Web.ViewModels
{
    public class AlbumResultViewModel
    {
        public IEnumerable<LocationViewModel> Location { get; set; }
        public IEnumerable<ItemBaseViewModel> Contents { get; set; }
    }
}