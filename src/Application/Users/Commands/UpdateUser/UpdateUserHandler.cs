using Application.Common.Interfaces;
using Application.Common.Results;
using Domain.Entities;
using Domain.Entities.Logs;
using Domain.Factories;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : ICommandHandler<UpdateUserRequest, Result<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UpdateUserHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<ApplicationUser>> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user is null)
            {
                return Result<ApplicationUser>.Failure("User not found");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;

            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var updateResult = await _userManager.UpdateAsync(user);

            Log log;
            if (updateResult.Succeeded)
            {
                log = LogFactory.CreateUserProfileUpdateSuccess(user, ipAddress);
            }
            else
            {
                var errorMessages = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                log = LogFactory.CreateUserProfileUpdateFailure(user, ipAddress, errorMessages);
            }

            _context.Logs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            if (!updateResult.Succeeded)
            {
                return Result<ApplicationUser>.Failure("Internal error while updating the user");
            }

            return Result<ApplicationUser>.SuccessResult(user);
        }
    }
}
