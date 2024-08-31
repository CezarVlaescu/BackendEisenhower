using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EisenhowerWebAPI.Services
{
    public class TokenGenerationServices
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordServices _passwordServices;

        public TokenGenerationServices(IConfiguration configuration, PasswordServices passwordServices)
        {
            _configuration = configuration;
            _passwordServices = passwordServices;
        }

        public string GenerateToken(string email, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", userId),
                    new Claim(ClaimTypes.Email, email),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }

        public bool VerifyPassword(string password, string passwordSalt, string passwordHash) =>
            _passwordServices.HashPassword(password, passwordSalt) == passwordHash; 

    }
}
