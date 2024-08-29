using System.Text;
using System.Security.Cryptography;

namespace EisenhowerWebAPI.Dto
{
    public class UserRegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Image {  get; set; }

        public UserRegisterDTO(string username, string email, string password, string image)
        {
            Username = username;
            Email = email;
            Image = image;
            Password = password;
        }
    }
}
