using EisenhowerWebAPI.Dto;
using EisenhowerWebAPI.Models;
using EisenhowerWebAPI.MongoContext;
using MongoDB.Driver;

public class CleanupHostedService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly ILogger<CleanupHostedService> _logger;
    private readonly MongoConnectionContext _context;

    public CleanupHostedService(ILogger<CleanupHostedService> logger, MongoConnectionContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleanup Hosted Service running.");

        // Setează timer-ul să verifice la fiecare oră
        _timer = new Timer(DoCleanup, null, TimeSpan.Zero, TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private async void DoCleanup(object state)
    {
        var now = DateTime.UtcNow;

        // Verifică dacă este 12 noaptea
        if (now.Hour == 0 && now.Minute == 0)
        {
            _logger.LogInformation("Cleaning up tasks...");
            await _context.Users.UpdateManyAsync(
                FilterDefinition<UserModel>.Empty,
                Builders<UserModel>.Update
                    .Set(u => u.Tasks.DoTasks, new List<TaskModelDto>())
                    .Set(u => u.Tasks.DecideTasks, new List<TaskModelDto>())
                    .Set(u => u.Tasks.DelegateTasks, new List<TaskModelDto>())
                    .Set(u => u.Tasks.DeleteTasks, new List<TaskModelDto>())
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleanup Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}