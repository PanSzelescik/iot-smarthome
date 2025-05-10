using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IotSmartHome.Function;

public class AddData
{
    private readonly ILogger<AddData> _logger;
    private readonly HttpClient _client;
    private readonly IoTHubServiceClient _ioTHubServiceClient;
    private readonly string _homeAssistantToken;

    public AddData(ILogger<AddData> logger, HttpClient client, IoTHubServiceClient ioTHubServiceClient)
    {
        _logger = logger;
        _client = client;
        _ioTHubServiceClient = ioTHubServiceClient;
        
        var homeAssistantToken = Environment.GetEnvironmentVariable("HOME_ASSISTANT_TOKEN");
        ArgumentException.ThrowIfNullOrEmpty(homeAssistantToken);
        _homeAssistantToken = homeAssistantToken;
    }

    [Function("AddData")]
    public async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "https://ha.panszelescik.pl/api/states/sensor.pokoj_szymona_termometr_temperature");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _homeAssistantToken);
        
        using var response = await _client.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<StateResponse>(cancellationToken);
        ArgumentNullException.ThrowIfNull(result);
        
        _logger.LogInformation("Current temperature is: {State}", result.State);
        
        var bytes = JsonSerializer.SerializeToUtf8Bytes(result);
        await _ioTHubServiceClient.SendMessageToDeviceAsync("thermometer", bytes);
    }
}

public sealed record StateResponse
{
    public required string State { get; init; }
}