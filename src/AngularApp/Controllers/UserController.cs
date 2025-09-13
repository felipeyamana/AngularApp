using Application.Common.Dispatchers.Interfaces;
using Application.Common.Results;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, UserManager<ApplicationUser> userManager)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _userManager = userManager;
        }
        [HttpGet("getcurrent")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return BadRequest();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("auth_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"   // must match login
            });

            return Ok(new { success = true, message = "Logged out successfully" });
        }
        [HttpPost("updateuser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _commandDispatcher.Dispatch<UpdateUserRequest, Result<ApplicationUser>>(request, cancellationToken);

            if (result.Success && result.Value != null) return Ok(new UserResponseDto
            {
                Id = result.Value.Id,
                FirstName = result.Value.FirstName,
                LastName = result.Value.LastName,
                Email = result.Value.Email
            });

            return BadRequest(new { Errors = result.Errors });
        }
    }
}
