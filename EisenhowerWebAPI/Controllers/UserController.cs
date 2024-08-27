using EisenhowerWebAPI.Dto;
using EisenhowerWebAPI.Models;
using EisenhowerWebAPI.MongoContext;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EisenhowerWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MongoConnectionContext _connectionContext;

        public UserController(MongoConnectionContext connectionContext) => _connectionContext = connectionContext;

        // HttpPost Mehtod -> Add new user

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserRegisterDTO userDTO)
        {
            try
            {
                if (userDTO == null) throw new ArgumentNullException(nameof(userDTO), "UserModel cannot be null");

                string salt = userDTO.GenerateSalt();
                string hash = userDTO.HashPassword(userDTO.Password, salt);

                var newUser = new UserModel
                {
                    Name = userDTO.Username,
                    Email = userDTO.Email,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    Image = userDTO.Image
                };

                // Log the newUser object
                Console.WriteLine($"Inserting user: {newUser}");

                await _connectionContext.Users.InsertOneAsync(newUser);

                // Log after insertion
                Console.WriteLine($"User inserted with ID: {newUser.Id}");

                return Ok(newUser);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // HttpGet Method -> Return all users

        [HttpGet]
        public async Task<ActionResult<List<UserModel>>> GetUsersAsync() => Ok(await _connectionContext.Users.Find(_ => true).ToListAsync()); 

        // HttpGet Method -> Return one user

        [HttpGet("{id}")]

        public async Task<ActionResult<UserModel>> GetOneUserAsync(string id)
        {
            try
            {
                var userId = new ObjectId(id);
                var user = await _connectionContext.Users.Find(u => u.Id == userId).SingleOrDefaultAsync() ?? throw new Exception($"User not found with id: {id}");
                return Ok(user);
            }
            catch
            {
                throw new Exception($"User not found with id: {id}");
            }
        }

        // HttpPut Method -> Update one user

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUserAsync([FromBody] UserModel updatedUser, [FromRoute] ObjectId id)
        {
            try
            {
                var updateUser = await _connectionContext.Users.ReplaceOneAsync(u => u.Id == id, updatedUser);
                if (updateUser.ModifiedCount == 0) return NotFound();
                return Ok(updatedUser);

            }
            catch
            {
                throw new Exception($"User was not updated");
            }
        }

        // HttpDelete Method -> Detele one user

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUserAsync(ObjectId id)
        {
            try
            {
                var deleteUser = await _connectionContext.Users.DeleteOneAsync(u => u.Id == id);
                if (deleteUser.DeletedCount == 0) return NotFound();
                return Ok(deleteUser);
            }
            catch
            {
                throw new Exception("User was not deleted");
            }
        }
    }
}
