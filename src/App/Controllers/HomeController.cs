using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace EnChanger.Controllers
{
    [Route("/")]
    public class HomeController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;

        public HomeController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            var filePath = _fileProvider.GetFileInfo("index.html").PhysicalPath;
            return PhysicalFile(filePath, "text/html");
        }

        [HttpGet("url/{id}")]
        public IActionResult Get([FromRoute] string id)
        {
            return GuidConverter.FromBase64(id).Match<IActionResult>(
                guid =>
                {
                    var filePath = _fileProvider.GetFileInfo("url.html").PhysicalPath;
                    return new PhysicalFileResult(filePath, "text/html");
                },
                _ => NotFound()
            );
        }
    }
}
