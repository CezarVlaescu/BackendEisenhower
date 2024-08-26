using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EisenhowerWebAPI.Models.Shared
{
    public abstract class SharedModel
    {
        [BsonElement("_id"), BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("createdAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
