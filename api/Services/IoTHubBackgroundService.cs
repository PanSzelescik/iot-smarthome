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
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IoTHubSenderService ioTHubSenderService)
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

                    var temperature = data.EventBody.ToObjectFromJson<TemperatureResponse>();
                    if (temperature != null)
                    {
                        var temperatureEntity = new TemperatureEntity
                        {
                            Id = messageIdGuid,
                            DeviceId = deviceId,
                            State = temperature.State,
                            CreatedDate = enqueuedTime,
                        };
                        logger.LogInformation("Temperature: {Message}", temperatureEntity);
                        await ProcessTemperatureMessage(temperatureEntity, stoppingToken);
                        
                        continue;
                    }
                    
                    var switchResponse = data.EventBody.ToObjectFromJson<SwitchResponse>();
                    if (switchResponse == null)
                    {
                        logger.LogWarning("Received message without temperature or enabled state, ignoring...");
                        continue;
                    }

                    var enabledEntity = new SwitchEntity
                    {
                        Id = messageIdGuid,
                        DeviceId = deviceId,
                        State = switchResponse.Enabled,
                        CreatedDate = enqueuedTime,
                    };
                    logger.LogInformation("Switch: {Message}", enabledEntity);
                    await ProcessSwitchMessage(enabledEntity, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message, ignoring...");
                }
            }
        }
    }
    
    private async Task ProcessSwitchMessage(SwitchEntity switchEntity, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!db.Temperatures.Any(x => x.Id == switchEntity.Id))
        {
            await db.Switches.AddAsync(switchEntity, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task ProcessTemperatureMessage(TemperatureEntity temperatureEntity, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!db.Temperatures.Any(x => x.Id == temperatureEntity.Id))
        {
            await db.Temperatures.AddAsync(temperatureEntity, cancellationToken);
            
            await ProcessAutomations(db, temperatureEntity, cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task ProcessAutomations(ApplicationDbContext db, TemperatureEntity temperatureEntity, CancellationToken cancellationToken)
    {
        var automations = await db.Automations
            .Where(x => x.UserThermometer.DeviceId == temperatureEntity.DeviceId)
            .Include(x => x.UserSwitch)
            .ToListAsync(cancellationToken);

        foreach (var automation in automations.Where(x => x.WhenCondition.IsTrue(temperatureEntity.State, x.WhenState)))
        {
            logger.LogInformation("Automation triggered: {Automation}", automation);
            try
            {
                await ioTHubSenderService.SendJsonAsync(automation.UserSwitch.DeviceId, new SwitchResponse { Enabled = automation.ThenState });
                await db.Switches.AddAsync(new SwitchEntity
                {
                    Id = Guid.CreateVersion7(),
                    DeviceId = automation.UserSwitch.DeviceId,
                    State = automation.ThenState,
                    CreatedDate = DateTimeOffset.UtcNow
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending switch response for automation {AutomationId}", automation.Id);
            }
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