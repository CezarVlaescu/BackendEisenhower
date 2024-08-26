using EisenhowerWebAPI.Models.Shared;
using MongoDB.Bson.Serialization.Attributes;

namespace EisenhowerWebAPI.Models
{
    public enum ETaskType { Do, Decide, Delegate, Delete }
    public class TasksPoolModel
    {
        [BsonElement("doTasks")]
        public List<TaskModel> DoTasks { get; set; } = new();

        [BsonElement("decideTasks")]
        public List<TaskModel> DecideTasks { get; set; } = new();

        [BsonElement("delegateTasks")]
        public List<TaskModel> DelegateTasks { get; set; } = new();

        [BsonElement("deleteTasks")]
        public List<TaskModel> DeleteTasks { get; set; } = new();
    }

    public class TaskModel : SharedModel
    {
        public string Name { get; set; }
        public DateTime Hour { get; set; }
        public ETaskType Type { get; set; }
        public bool? IsCommented { get; set; }
        public string? Comments { get; set; }

    }
}
