using EisenhowerWebAPI.Models;
using EisenhowerWebAPI.MongoContext;
using Microsoft.AspNetCore.Connections;
using MongoDB.Driver;

namespace EisenhowerWebAPI.Services
{
    public interface ICleanUpService
    {
        Task CleanUp(TasksPoolModel taskPool); 
    }
    public class CleanUpService : ICleanUpService
    {
        public Task CleanUp(TasksPoolModel taskPool)
        {
            taskPool.DoTasks.Clear();
            taskPool.DecideTasks.Clear();
            taskPool.DelegateTasks.Clear();
            taskPool.DeleteTasks.Clear();

            return Task.CompletedTask;
        }
    }

    public class ScheduleCleanUpService : IDisposable, IHostedService
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public ScheduleCleanUpService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var midnight = DateTime.UtcNow.Date.AddDays(1).AddHours(0);
            var timeUntilMidnight = midnight - DateTime.UtcNow;
            _timer = new Timer(DoWork, null, timeUntilMidnight, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _scopeFactory.CreateScope();
            var cleanUpService = scope.ServiceProvider.GetRequiredService<ICleanUpService>();
            var connectionContext = scope.ServiceProvider.GetRequiredService<MongoConnectionContext>();

            var users = await connectionContext.Users.Find(_ => true).ToListAsync();

            foreach (var user in users)
            {
                await cleanUpService.CleanUp(user.Tasks);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
