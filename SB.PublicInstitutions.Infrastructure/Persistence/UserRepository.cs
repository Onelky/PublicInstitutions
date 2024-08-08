using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SB.PublicInstitutions.Domain.Exceptions;
using Microsoft.Extensions.Options;
using SB.PublicInstitutions.Infrastructure.Models;
using SB.PublicInstitutions.Infrastructure.Utils;
using static BCrypt.Net.BCrypt;

public sealed class UsersRepository(ILogger<UsersRepository> logger, IOptions<DatabasePaths> options) : IUserRepository
{
    private readonly string filePath = options.Value.Users;

    public async Task<User> Register(User user)
    {
        try
        {            
            var existingUser = await GetUserByUsername(user.Username);

            if (existingUser is not null) {
                logger.LogError("Attempted to create user with an existing username");
                throw new DuplicatedUsername(user.Username);
            }

            user.Password = HashPassword(user.Password);

            var line = JsonConvert.SerializeObject(user);
            await File.AppendAllLinesAsync(filePath, new[] { line });
            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering users");
            throw new Exception("Error registering users", ex);
        }
    }

    public async Task<User> Login(User user)
    {
        try
        {
            var existingUser =  await GetUserByUsername(user.Username) ?? throw new NotFoundException("User not found");

            if (!Verify(user.Password, existingUser.Password)) throw new Unauthorized(user.Username); 

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unauthorized user");
            throw new Exception("Unauthorized user", ex);
        }
    }

    private async Task<User> GetUserByUsername(string username)
    {
        var users = await FileUtils.GetDeserializedItems<User>(filePath);
        return users.FirstOrDefault(u => u.Username == username);
    }
    
}
