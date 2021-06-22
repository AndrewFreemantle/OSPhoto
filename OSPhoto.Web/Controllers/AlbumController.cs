using System.IO;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OSPhoto.Services.Interfaces;
using OSPhoto.Web.Extensions;
using OSPhoto.Web.ViewModels;

namespace OSPhoto.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IMapper _mapper;
        // private readonly ILogger<HomeController> _logger;

        public AlbumController(
            IAlbumService albumService
            , IMapper mapper
            /*, ILogger<HomeController> logger */)
        {
            _albumService = albumService;
            _mapper = mapper;
            // _logger = logger;
        }
        
        [HttpGet]
        [Route("/album")]
        [Route("/album/{path?}")]
        public IActionResult Album(string path = null)
        {
            var result = _albumService.Get(string.IsNullOrEmpty(path)
                ? null
                : Path.Combine(_albumService.GetContentRootPath(), path.UrlDecode()));

            return Ok(_mapper.Map<AlbumResultViewModel>(result));
        }
        
        [HttpGet]
        [Route("/image/{path}")]
        public IActionResult Image(string path)
        {
            var image = _albumService.GetImage(WebUtility.UrlDecode(path));
            return PhysicalFile(Path.Combine(_albumService.GetContentRootPath(), image.Path), image.ContentType);
        }
    }
}


