using EisenhowerWebAPI.Models.Shared;
using MongoDB.Bson.Serialization.Attributes;

namespace EisenhowerWebAPI.Models
{
    public class UserModel : SharedModel
    {
        [BsonElement("name"), BsonRequired]
        public string Name { get; set; }

        [BsonElement("email"), BsonRequired]
        public string Email { get; set; }

        [BsonElement("passwordSalt")]
        public string PasswordSalt { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("image")]
        public string? Image {  get; set; }

        [BsonElement("tasks")]
        public TasksPoolModel Tasks { get; set; } = new();
    }
}
