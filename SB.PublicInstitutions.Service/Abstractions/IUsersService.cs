namespace SB.PublicInstitutions.Services.Abstractions
{
    public interface IUsersService 
    {
        Task<string> Login(User user);
        Task<User> Register(User user);
    }
}
