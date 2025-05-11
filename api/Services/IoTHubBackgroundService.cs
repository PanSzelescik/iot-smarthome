using Azure.Messaging.EventHubs.Consumer;
using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Services;

public class IoTHubBackgroundService(
    ILogger<IoTHubBackgroundService> logger,
    IConfiguration configuration,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : BackgroundService
{
    private readonly string _eventHubCompatibleConnectionString = configuration.GetRequiredConnectionString("EventHubCompatibleConnectionString");

    private readonly string _eventHubName = configuration.GetRequiredConnectionString("EventHubName");
    private EventHubConsumerClient? _client;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _client = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, _eventHubCompatibleConnectionString, _eventHubName);
        
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var @event in _client!.ReadEventsAsync(stoppingToken))
            {
                try
                {
                    var data = @event.Data;
                    if (data == null)
                    {
                        continue;
                    }

                    var messageId = data.MessageId;
                    if (string.IsNullOrEmpty(messageId))
                    {
                        logger.LogWarning("Received message without MessageId, ignoring...");
                        continue;
                    }

                    if (!Guid.TryParse(messageId, out var messageIdGuid))
                    {
                        logger.LogWarning("Received messageId is not Guid, ignoring...");
                        continue;
                    }

                    var deviceId = data.SystemProperties["iothub-connection-device-id"]?.ToString();
                    if (string.IsNullOrEmpty(deviceId))
                    {
                        logger.LogWarning("Received message without DeviceId, ignoring...");
                        continue;
                    }

                    var enqueuedTime = data.EnqueuedTime;
                    if (enqueuedTime == default)
                    {
                        logger.LogWarning("Received message without enqueued time, setting to now...");
                        enqueuedTime = DateTimeOffset.UtcNow;
                    }

                    var state = data.EventBody.ToObjectFromJson<TemperatureResponse>();
                    if (state == null)
                    {
                        logger.LogWarning("Received invalid message, ignoring...");
                        continue;
                    }

                    var temperatureEntity = new TemperatureEntity
                    {
                        Id = messageIdGuid,
                        DeviceId = deviceId,
                        State = state.State,
                        CreatedDate = enqueuedTime,
                    };
                    logger.LogInformation("{Message}", temperatureEntity);
                    await ProcessTemperatureMessage(temperatureEntity, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message, ignoring...");
                }
            }
        }
    }

    private async Task ProcessTemperatureMessage(TemperatureEntity temperatureEntity, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!db.Temperatures.Any(x => x.Id == temperatureEntity.Id))
        {
            await db.Temperatures.AddAsync(temperatureEntity, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (_client != null)
        {
            await _client.DisposeAsync();
        }
    }
}