using AngularApp.Realtime.Publishers;
using Application.Common.Dispatchers.Interfaces;
using Application.Common.Results;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignalRNotificationPublisher _notificationPublisher;
        public UserController(IUserService userService, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, UserManager<ApplicationUser> userManager, SignalRNotificationPublisher notificationPublisher)
        {
            _userService = userService;
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _userManager = userManager;
            _notificationPublisher = notificationPublisher;
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
        [HttpGet("getusers")]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1)
        {
            var usersResult = await _userService.GetUsersAsync(page, pageSize: 20);

            if (usersResult.Success) return Ok(usersResult.Value);

            return BadRequest();
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

            if (result.Success && result.Value != null)
            {
                await _notificationPublisher.NotifyAllUsersAsync(
                    type: "test",
                    payload: new { message = $"User {result.Value.Id} was updated" });

                return Ok(new UserResponseDto
                {
                    Id = result.Value.Id,
                    FirstName = result.Value.FirstName,
                    LastName = result.Value.LastName,
                    Email = result.Value.Email
                });
            }

            return BadRequest(new { Errors = result.Errors });
        }
    }
}
