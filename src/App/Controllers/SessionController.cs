using System;
using EnChanger.Database;
using EnChanger.Infrastructure.Filters;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EnChanger.Controllers
{
    [Route("api/session")]
    [ApiController]
    [MonadicResultFilter]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost]
        public IActionResult NewSession()
        {
            var session = _sessionService.NewSession();
            return CreatedAtAction("Get", new {session.Id}, session);
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] Guid id) =>
            Ok(_sessionService.Get(id));


        [HttpGet("{id}/entries")]
        public IActionResult AssociatedEntriesIds([FromRoute] Guid id) =>
            Ok(_sessionService.GetAssociatedEntriesIds(id));
    }
}
