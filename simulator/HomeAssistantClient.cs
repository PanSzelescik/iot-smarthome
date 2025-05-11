using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IotSmartHome.Function;

public class HomeAssistantClient
{
    private readonly ILogger<HomeAssistantClient> _logger;
    private readonly HttpClient _client;
    private readonly string _homeAssistantToken;

    public HomeAssistantClient(ILogger<HomeAssistantClient> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
        
        var homeAssistantToken = Environment.GetEnvironmentVariable(Consts.HomeAssistantToken);
        ArgumentException.ThrowIfNullOrEmpty(homeAssistantToken);
        _homeAssistantToken = homeAssistantToken;
    }

    public async Task<HomeAssistantStateResponse> FetchData(string sensor, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"https://ha.panszelescik.pl/api/states/{sensor}");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _homeAssistantToken);
        
        using var response = await _client.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<HomeAssistantStateResponse>(cancellationToken);
        ArgumentNullException.ThrowIfNull(result);
        
        _logger.LogInformation("{Sensor}: {State}", sensor, result.State);
        return result;
    }
}

public sealed record HomeAssistantStateResponse
{
    public required string State { get; init; }
}