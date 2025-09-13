using Application.Common.Interfaces;
using Application.Common.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : ICommandHandler<UpdateUserRequest, Result<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UpdateUserHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result<ApplicationUser>.Failure("Internal error while updating the user");
            }

            return Result<ApplicationUser>.SuccessResult(user);
        }

    }
}
