using Application.Common.Dispatchers.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Results;
using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.UserLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public AuthController(
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model, CancellationToken cancellationToken)
        {
            var result = await _commandDispatcher.Dispatch<RegisterUserRequest, Result<string>>(model, cancellationToken);
            if (result.Success)
                return Ok(new { Message = "User registered successfully!", UserId = result.Value });

            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model, CancellationToken cancellationToken)
        {
            var result = await _queryDispatcher.Dispatch<UserLoginRequest, Result<string>>(model, cancellationToken);
            if (!result.Success)
                return Unauthorized(new { Errors = result.Errors });

            return Ok(new { Token = result.Value });
        }
    }
}
