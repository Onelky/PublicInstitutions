using Shared.DTOs;

namespace SB.PublicInstitutions.Services.Abstractions
{
    public interface IUsersService 
    {
        Task<string> Login(User user);
        Task<RegisterUserResponse> Register(User user);
    }
}
