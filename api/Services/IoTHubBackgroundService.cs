using System.Text;
using IotSmartHome.Data;
using Microsoft.Azure.Devices.Client;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Services;

public class IoTHubBackgroundService(
    ILogger<IoTHubBackgroundService> logger,
    IConfiguration configuration,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : BackgroundService
{
    private DeviceClient? _deviceClient;
    
    private string ConnectionString
    {
        get
        {
            var connectionString = configuration.GetConnectionString("IoTHubDevice");
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            return connectionString;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await ReconnectAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Disconnecting from IoT Hub...");
        if (_deviceClient != null)
        {
            await _deviceClient.CloseAsync(cancellationToken);
            await _deviceClient.DisposeAsync();
        }

        logger.LogInformation("Disconnected from IoT Hub!");
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_deviceClient == null)
                {
                    if (!await ReconnectAsync(cancellationToken))
                    {
                        continue;
                    }
                }

                using var receivedMessage = await _deviceClient!.ReceiveAsync(cancellationToken);
                if (receivedMessage != null)
                {
                    if (!receivedMessage.Properties.TryGetValue("iothub-creation-time-utc", out var creationTimeUtcString)
                        || !DateTime.TryParse(creationTimeUtcString, out var creationTimeUtc))
                    {
                        creationTimeUtc = DateTime.UtcNow;
                    }

                    var messageBody = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    logger.LogInformation("[{MessageId}]: {MessageBody} at: {CreationTimeUtc}", receivedMessage.MessageId, messageBody, creationTimeUtc);

                    await _deviceClient.CompleteAsync(receivedMessage, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading message from IoT Hub");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task<bool> ReconnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_deviceClient != null)
            {
                logger.LogInformation("Disconnecting from IoT Hub...");
                await _deviceClient.CloseAsync(cancellationToken);
                await _deviceClient.DisposeAsync();
                _deviceClient = null;
                logger.LogInformation("Disconnected from IoT Hub!");
            }

            logger.LogInformation("Connecting to IoT Hub...");
            _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
            await _deviceClient.OpenAsync(cancellationToken);
            logger.LogInformation("Connected to IoT Hub!");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while connecting to IoT Hub");
        }

        return false;
    }
}