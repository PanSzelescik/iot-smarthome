﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Microsoft.Azure.Devices.Client;

namespace IotSmartHome.Function;

public class IoTHubDeviceClient : IDisposable, IAsyncDisposable
{
    private readonly DeviceClient _client;

    public IoTHubDeviceClient(string device)
    {
        var connectionString = Environment.GetEnvironmentVariable(Consts.IoTHubDeviceConnectionString(device));
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        _client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
    }

    public async Task SendJsonAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
        await SendBytesAsync(bytes, cancellationToken);
    }

    public async Task SendBytesAsync(byte[] message, CancellationToken cancellationToken = default)
    {
        using var msg = new Message(message);
        msg.MessageId = Guid.CreateVersion7().ToString();
        await _client.SendEventAsync(msg, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}

public sealed record TemperatureResponse
{
    public required double State { get; init; }

    [SetsRequiredMembers]
    public TemperatureResponse(HomeAssistantStateResponse response) : this(response.State)
    {
    }
    
    [SetsRequiredMembers]
    public TemperatureResponse(string state)
    {
        if (!double.TryParse(state, CultureInfo.InvariantCulture, out var parsedState))
        {
            throw new ArgumentException($"Invalid state: {state}", nameof(state));
        }
        
        State = parsedState;
    }
}

public sealed record SwitchResponse
{
    public required bool Enabled { get; init; }

    [SetsRequiredMembers]
    public SwitchResponse(bool enabled)
    {
        Enabled = enabled;
    }
}