using Application.Auth.Dtos;
using Application.Common.Attributes;
using Application.Common.Dispatchers.Interfaces;
using Application.Common.Results;
using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.ExternalLogin;
using Application.Users.Queries.UserLogin;
using Google.Apis.Auth;
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
        private readonly IConfiguration _configuration;
        public AuthController(
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            IConfiguration configuration)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _configuration = configuration;
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
            if (!result.Success || string.IsNullOrEmpty(result.Value))
                return Unauthorized(new { Errors = result.Errors });

            Response.Cookies.Append("auth_token", result.Value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                // since everything is HTTPS
                SameSite = SameSiteMode.Strict, // same-origin, so Strict is fine
                Expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]))
            });

            return Ok(new { Token = result.Value });
        }
        [HttpPost("google")]
        [AllowAnonymous]
        [UsesHandler(typeof(ExternalLoginHandler))]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest("Token is required");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                    _configuration["Google:ClientId"]
                }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);

            var loginRequest = new ExternalLoginRequest
            {
                Email = payload.Email,
                Provider = "Google",
                ProviderUserId = payload.Subject
            };

            var result = await _queryDispatcher.Dispatch<ExternalLoginRequest, Result<string>>(loginRequest, cancellationToken);

            if (!result.Success || string.IsNullOrEmpty(result.Value))
                return Unauthorized(new { Errors = result.Errors });

            Response.Cookies.Append("auth_token", result.Value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]))
            });

            return Ok(new { Token = result.Value });
        }
    }
}
