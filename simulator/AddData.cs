using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IotSmartHome.Function;

public class AddData(ILogger<AddData> logger, HttpClient client)
{
    [Function("AddData")]
    public async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        var token = GetToken();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        
        using var message = new HttpRequestMessage(HttpMethod.Get, "https://ha.panszelescik.pl/api/states/sensor.pokoj_szymona_termometr_temperature");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        using var response = await client.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<StateResponse>(cancellationToken);
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        
        logger.LogInformation("Current temperature is: {State}", result.State);
    }
    
    private static string? GetToken() => Environment.GetEnvironmentVariable("HOME_ASSISTANT_TOKEN");
}

public sealed record StateResponse
{
    public required string State { get; init; }
}