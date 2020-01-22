using System;
using EnChanger.Helpers;
using EnChanger.Infrastructure.Filters;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using EnChanger.Services.Models;
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
        public IActionResult Post([FromBody] PasswordInput password)
        {
            var id = _passwordService.Add(password);
            return CreatedAtAction(
                "Get",
                new {Id = id.ToBase64()},
                null
            );
        }
    }
}
