using EisenhowerWebAPI.Dto;
using EisenhowerWebAPI.Models;
using EisenhowerWebAPI.MongoContext;
using EisenhowerWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EisenhowerWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly MongoConnectionContext _connectionContext;
        private readonly TasksServices _taskServices;
        public TasksController(MongoConnectionContext connectionContext, TasksServices tasksServices)
        {
            _connectionContext = connectionContext;
            _taskServices = tasksServices;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModelDto taskModelDto, string userId) 
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _connectionContext.Users.Find(u => u.Id == objectId).SingleOrDefaultAsync() ?? throw new Exception($"User not found with id: {objectId}");

                var taskList = _taskServices.GetListBasedOnType(taskModelDto.Type, user.Tasks.DoTasks, user.Tasks.DecideTasks, user.Tasks.DelegateTasks, user.Tasks.DeleteTasks);

                var newTask = new TaskModelDto
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = taskModelDto.Name,
                    Hour = taskModelDto.Hour,
                    Type = taskModelDto.Type,
                    IsCommented = taskModelDto.IsCommented,
                    Comments = taskModelDto.Comments
                };

                taskList.Add(newTask);
                _taskServices.SortTasks(taskList);

                await _connectionContext.Users.ReplaceOneAsync(u => u.Id == objectId, user);

                return Ok(new { message = "Task created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task: {ex.Message}");
                return StatusCode(500, "Problem creating the task");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<TasksPoolModel>> GetAllTaskPool(string userId)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _connectionContext.Users.Find(u => u.Id == objectId).SingleOrDefaultAsync() ?? throw new Exception($"User not found with id: {objectId}");

                return Ok(user.Tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tasks: {ex.Message}");
                return StatusCode(500, "Problem getting tasks");
            }
        }

        [HttpPut("{userId}/{taskId}")]
        public async Task<ActionResult<string>> UpdateTask(string userId, string taskId, ETaskType type, [FromBody] TaskModelDto updatedTask)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _connectionContext.Users.Find(u => u.Id == objectId).SingleOrDefaultAsync()
                          ?? throw new Exception($"User not found with id: {objectId}");

                var taskList = _taskServices.GetListBasedOnType(type, user.Tasks.DoTasks, user.Tasks.DecideTasks, user.Tasks.DelegateTasks, user.Tasks.DeleteTasks);

                var taskToUpdate = taskList.Find(item => item.Id == taskId)
                                    ?? throw new Exception($"Task not found with id: {taskId}");

                taskList.Remove(taskToUpdate);

                taskToUpdate.Name = updatedTask.Name;
                taskToUpdate.Hour = updatedTask.Hour;
                taskToUpdate.IsCommented = updatedTask.IsCommented;
                taskToUpdate.Comments = updatedTask.Comments;

                if (updatedTask.Type != type)
                {
                    var rightList = _taskServices.GetListBasedOnType(updatedTask.Type, user.Tasks.DoTasks, user.Tasks.DecideTasks, user.Tasks.DelegateTasks, user.Tasks.DeleteTasks);
                    rightList.Add(taskToUpdate);
                    _taskServices.SortTasks(rightList);
                }
                else
                {
                    taskList.Add(taskToUpdate);
                    _taskServices.SortTasks(taskList);
                }

                await _connectionContext.Users.ReplaceOneAsync(u => u.Id == objectId, user);

                return Ok("Task updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
                return StatusCode(500, $"Problem updating the task: {ex.Message}");
            }
        }

        [HttpDelete("{userId}/{taskId}")]
        public async Task<IActionResult> DeleteTask(string userId, string taskId, ETaskType type)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _connectionContext.Users.Find(u => u.Id == objectId).SingleOrDefaultAsync() ?? throw new Exception($"Task not found");

                var taskList = _taskServices.GetListBasedOnType(type, user.Tasks.DoTasks, user.Tasks.DecideTasks, user.Tasks.DelegateTasks, user.Tasks.DeleteTasks);

                var taskToUpdate = taskList.Find(item => item.Id == taskId) ?? throw new Exception($"Task not found");
                taskList.Remove(taskToUpdate);

                _taskServices.SortTasks(taskList);

                await _connectionContext.Users.ReplaceOneAsync(u => u.Id == objectId, user);

                return Ok("Task deleted successfull");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
                return StatusCode(500, "Problem deleting the task");
            }
        }
    }
}
