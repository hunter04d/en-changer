using EnChanger.Services;
using EnChanger.Services.Abstractions;
using LanguageExt;
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
        public ActionResult<PasswordDto> Get(string id) =>
            GuidConverter.FromBase64(id)
                .Bind(guid => _passwordService.Get(guid))
                .Match<ActionResult<PasswordDto>>(
                    dto => dto,
                    () => NotFound(),
                    e => NotFound()
                );

        [HttpPost]
        public IActionResult Post([FromBody] PasswordDto password) =>
            _passwordService.Add(password).Match<IActionResult>(
                entry => CreatedAtAction(
                    "Get",
                    new {Id = GuidConverter.ToBase64(entry.Id)},
                    null),
                e => BadRequest()
            );
    }
}
