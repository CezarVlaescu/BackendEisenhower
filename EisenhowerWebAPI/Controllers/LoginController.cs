using EisenhowerWebAPI.Dto;
using EisenhowerWebAPI.MongoContext;
using EisenhowerWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EisenhowerWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly MongoConnectionContext _connectionContext;
        private readonly TokenGenerationServices _tokenGenerationService;
        public LoginController(MongoConnectionContext connectionContext, TokenGenerationServices tokenGenerationService)
        {
            _connectionContext = connectionContext;
            _tokenGenerationService = tokenGenerationService;
        }

        // Get the user email and password

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> GetUserLogin(LoginModelDto loginModelDto)
        {
            try
            {
                var user = await _connectionContext.Users.Find(u => u.Email == loginModelDto.Email).SingleOrDefaultAsync() ?? throw new Exception($"User not found");
                var isValidPassword = _tokenGenerationService.VerifyPassword(loginModelDto.Password, user.PasswordSalt, user.PasswordHash);

                if (!isValidPassword) return Unauthorized("Invalid password");

                var token = _tokenGenerationService.GenerateToken(user.Email);
                return Ok(new LoginResponse { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
