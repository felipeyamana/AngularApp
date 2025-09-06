using Application.Common.Interfaces;
using Application.Common.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : ICommandHandler<RegisterUserRequest, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUserHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(RegisterUserRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
                return Result<string>.SuccessResult(user.Id);

            return Result<string>.Failure(result.Errors.Select(e => e.Description));
        }
    }
}
