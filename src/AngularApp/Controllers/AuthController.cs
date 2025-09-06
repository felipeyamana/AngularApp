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
        private readonly ICommandHandler<RegisterUserRequest, Result<string>> _registerHandler;
        private readonly IQueryHandler<UserLoginRequest, Result<string>> _loginHandler;

        public AuthController(
            ICommandHandler<RegisterUserRequest, Result<string>> registerHandler,
            IQueryHandler<UserLoginRequest, Result<string>> loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            var result = await _registerHandler.Handle(model);
            if (result.Success)
                return Ok(new { Message = "User registered successfully!", UserId = result.Value });

            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            var result = await _loginHandler.Handle(model);
            if (!result.Success)
                return Unauthorized(new { Errors = result.Errors });

            return Ok(new { Token = result.Value });
        }
    }
}
