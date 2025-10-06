using Application.Common.Interfaces;
using Application.Common.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : ICommandHandler<RegisterUserRequest, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RegisterUserHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser is not null)
                    return Result<string>.Failure(["Email already in use"]);

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                    return Result<string>.Failure(result.Errors.Select(e => e.Description));

                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole("User"));

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                    return Result<string>.Failure(roleResult.Errors.Select(e => e.Description));

                return Result<string>.SuccessResult(user.Id);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_AspNetUsers_NormalizedEmail") == true)
            {
                // treating possible concurrency problem if multiple users try to create an account with an email that doesnt exist yet at the same time
                return Result<string>.Failure(["Email already in use"]);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(["Unexpected error during registration"]);
            }
        }
    }
}
