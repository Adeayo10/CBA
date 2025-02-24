using System.Collections.Concurrent;
using CBA.Models;

namespace CBA.Services;
public class BackgroundEmailService : BackgroundService, IBackgroundEmailService
{
    private readonly ILogger<BackgroundEmailService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentQueue<Message> _queue;
    private readonly SemaphoreSlim _signal;

    public BackgroundEmailService(ILogger<BackgroundEmailService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _queue = new ConcurrentQueue<Message>();
        _signal = new SemaphoreSlim(0);
        _logger.LogInformation("BackgroundEmailService constructor called");
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BackgroundEmailService is starting");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExecuteAsync method started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Waiting for signal. Current queue size: {QueueSize}", _queue.Count);
                await _signal.WaitAsync(stoppingToken);

                _logger.LogInformation("Signal received. Processing queue. Size: {QueueSize}", _queue.Count);

                while (_queue.TryDequeue(out var message))
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            _logger.LogInformation("Processing email: {MessageContent}", message.Content);
                            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            await emailService.SendEmail(message);
                            _logger.LogInformation("Email sent successfully: {MessageContent}", message.Content);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process email: {MessageContent}", message.Content);
                        _queue.Enqueue(message);
                        _signal.Release();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background service operation cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExecuteAsync");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("ExecuteAsync method ending");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BackgroundEmailService is stopping");
        await base.StopAsync(cancellationToken);
    }

    public void QueueEmail(Message message)
    {
        if (message == null)
        {
            _logger.LogError("Attempted to queue null message");
            throw new ArgumentNullException(nameof(message));
        }

        _queue.Enqueue(message);
        _logger.LogInformation("Email queued. Queue size: {QueueSize}, Content: {MessageContent}", _queue.Count, message.Content);
        _signal.Release();
    }
}

public interface IBackgroundEmailService : IHostedService
{
    void QueueEmail(Message message);
}
