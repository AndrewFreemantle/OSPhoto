using AutoMapper;
using OSPhoto.Services;
using OSPhoto.Web.Extensions;

namespace OSPhoto.Web.ViewModels
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<AlbumResult, AlbumResultViewModel>();

            CreateMap<Location, LocationViewModel>()
                .ForMember(d => d.Path, o => o.MapFrom(s => s.Path.UrlEncode()));

            CreateMap<ItemBase, ItemBaseViewModel>()
                .ForMember(d => d.Path, o => o.MapFrom(s => s.Path.UrlEncode()));

            CreateMap<Directory, DirectoryViewModel>()
                .IncludeBase<ItemBase, ItemBaseViewModel>();
            CreateMap<File, FileViewModel>()
                .IncludeBase<ItemBase, ItemBaseViewModel>();
            CreateMap<Image, ImageViewModel>()
                .IncludeBase<ItemBase, ItemBaseViewModel>();
        }
    }
}