using System;
using EnChanger.Helpers;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EnChanger.Controllers
{
    [Route("api")]
    [ApiController]
    public class EncodeController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public EncodeController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        // GET api/values

        [HttpGet("{id}")]
        public ActionResult<PasswordDto> Get([FromRoute] Guid id) =>
            _passwordService.Get(id).Match(
                dto => dto,
                (ActionResult<PasswordDto>) NotFound()
            );

        [HttpPost]
        public IActionResult Post([FromBody] PasswordDto password) =>
            _passwordService.Add(password).Match(
                entry => CreatedAtAction(
                    "Get",
                    new {Id = GuidConverter.ToBase64(entry.Id)},
                    null),
                (IActionResult) BadRequest()
            );
    }
}
