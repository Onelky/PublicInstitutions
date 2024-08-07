public interface IUserRepository
{
    Task<User> Login(User user);
    Task<User> Register(User user);
}

