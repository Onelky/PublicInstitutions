using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SB.PublicInstitutions.Domain.Exceptions;
using SB.PublicInstitutions.Services.Abstractions;
using SB.PublicInstitutions.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SB.PublicInstitutions.Service
{
    public sealed  class UsersService(IUserRepository repository, ILogger<UsersService> logger, IOptions<JwtOptions> jwtOptions) : IUsersService
    {

        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task<string> Login(User user)
        {   
            var existingUser = await repository.Login(user);

            if (existingUser is null)
            {
                throw new Unauthorized(user.Username);
            }

            logger.LogInformation($"User {user.Username} logged in");
            return GenerateToken(user);
        }

        public async Task<User> Register(User user)
        {
            var newUser = await repository.Register(user);
            logger.LogInformation($"User registered");
            return newUser;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Username.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
