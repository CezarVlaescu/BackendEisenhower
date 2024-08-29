namespace EisenhowerWebAPI.Dto
{
    public enum ETaskType { Do, Decide, Delegate, Delete }
    public class TaskModelDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Hour { get; set; }
        public ETaskType Type { get; set; }
        public bool? IsCommented { get; set; }
        public string? Comments { get; set; }
    }
}
