using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace EisenhowerWebAPI.Dto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ETaskType
    {
        [EnumMember(Value = "Do")]
        Do,
        [EnumMember(Value = "Decide")]
        Decide,
        [EnumMember(Value = "Delegate")]
        Delegate,
        [EnumMember(Value = "Delete")]
        Delete
    }

    [BsonIgnoreExtraElements]
    public class TaskModelDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Hour { get; set; }
        public ETaskType Type { get; set; }
        public bool? IsCommented { get; set; }
        public string? Comments { get; set; }
    }
}
