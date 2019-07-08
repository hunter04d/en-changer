using System;
using EnChanger.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

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
        // ReSharper disable once UnusedParameter.Global
        public IActionResult Get([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return NotFound();
            var filePath = _fileProvider.GetFileInfo("url.html").PhysicalPath;
            return PhysicalFile(filePath, "text/html");
        }
    }
}
