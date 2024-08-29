using EisenhowerWebAPI.Dto;

namespace EisenhowerWebAPI.Services
{
    public class TasksServices
    {
        public List<TaskModelDto> GetListBasedOnType(ETaskType taskType, List<TaskModelDto> doList, List<TaskModelDto> decideList, List<TaskModelDto> delegateList, List<TaskModelDto> deleteList)
        {
            return taskType switch
            {
                ETaskType.Do => doList,
                ETaskType.Decide => decideList,
                ETaskType.Delegate => delegateList,
                ETaskType.Delete => deleteList,
                _ => throw new Exception("List not found")
            }; ;
        }

        public void SortTasks(List<TaskModelDto> list) => list.Sort((x, y) => DateTime.Compare(x.Hour, y.Hour)); 
    }
}
