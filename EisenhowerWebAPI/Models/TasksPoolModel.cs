using EisenhowerWebAPI.Dto;
using MongoDB.Bson.Serialization.Attributes;

namespace EisenhowerWebAPI.Models
{
    public class TasksPoolModel
    {
        [BsonElement("doTasks")]
        public List<TaskModelDto> DoTasks { get; set; } = new();

        [BsonElement("decideTasks")]
        public List<TaskModelDto> DecideTasks { get; set; } = new();

        [BsonElement("delegateTasks")]
        public List<TaskModelDto> DelegateTasks { get; set; } = new();

        [BsonElement("deleteTasks")]
        public List<TaskModelDto> DeleteTasks { get; set; } = new();
    }
}
