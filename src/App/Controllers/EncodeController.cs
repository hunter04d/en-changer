using System;
using EnChanger.Helpers;
using EnChanger.Infrastructure.Filters;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EnChanger.Controllers
{
    [Route("api")]
    [ApiController]
    [MonadicResultFilter]
    public class EncodeController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public EncodeController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PasswordDto), 200)]
        public IActionResult Get([FromRoute] Guid id) =>
            Ok(_passwordService.Get(id));

        [HttpPost]
        public IActionResult Post([FromBody] PasswordDto password)
        {
            var entry = _passwordService.Add(password);
            return CreatedAtAction(
                "Get",
                new {Id = entry.Id.ToBase64()},
                null
            );
        }
    }
}
