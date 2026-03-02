using Application.Common.Results;
using Application.Users.Dtos;

namespace Application.Users.Interfaces
{
    public interface IUserService
    {
        Task<Result<List<UserResponseDto>>> GetUsersAsync(int page, int pageSize);
    }
}
