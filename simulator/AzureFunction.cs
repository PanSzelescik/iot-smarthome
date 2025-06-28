using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace IotSmartHome.Function;

public class AzureFunction(HomeAssistantClient homeAssistantClient, IoTHubDeviceClientFactory ioTHubDeviceClientFactory)
{
    [Function("PokojSzymona")]
    public async Task PokojSzymona(
        [TimerTrigger("0 */15 * * * *")] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        await FetchAndSendData("sensor.pokoj_szymona_termometr_temperature", "pokoj_szymona", cancellationToken);
    }
    
    [Function("Salon")]
    public async Task Salon(
        [TimerTrigger("0 */15 * * * *")] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        await FetchAndSendData("sensor.salon_termometr_temperature", "salon", cancellationToken);
    }
    
    [Function("FakePokojSzymona")]
    public async Task<IActionResult> FakePokojSzymona(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = "pokoj_szymona")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        return await FakeAndSendData(request, "pokoj_szymona", cancellationToken);
    }
    
    [Function("FakeSalon")]
    public async Task<IActionResult> FakeSalon(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = "salon")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        return await FakeAndSendData(request, "salon", cancellationToken);
    }

    [Function("FakeKlima")]
    public async Task<IActionResult> FakeKlima(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = "klima")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        return await SendData(request, "klima", cancellationToken);
    }
    
    [Function("FakeGrzejnik")]
    public async Task<IActionResult> FakeGrzejnik(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = "grzejnik")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        return await SendData(request, "grzejnik", cancellationToken);
    }
    
    private async Task FetchAndSendData(string sensor, string device, CancellationToken cancellationToken)
    {
        var homeAssistantStateResponse = await homeAssistantClient.FetchData(sensor, cancellationToken);
        var temperatureResponse = new TemperatureResponse(homeAssistantStateResponse);

        await SendData(temperatureResponse, device, cancellationToken);
    }
    
    private async Task<IActionResult> FakeAndSendData(HttpRequest request, string device, CancellationToken cancellationToken)
    {
        if (!request.Query.TryGetValue("temperature", out var temperature))
        {
            return new BadRequestObjectResult("Missing 'temperature' query parameter.");
        }
        
        var temperatureString = temperature.ToString();
        if (string.IsNullOrWhiteSpace(temperatureString))
        {
            return new BadRequestObjectResult("Temperature cannot be empty.");
        }
        
        var temperatureResponse = new TemperatureResponse(temperatureString);
        
        await SendData(temperatureResponse, device, cancellationToken);
        
        return new OkObjectResult(temperatureResponse);
    }

    private async Task SendData(TemperatureResponse temperatureResponse, string device, CancellationToken cancellationToken)
    {
        var deviceClient = ioTHubDeviceClientFactory.GetClient(device);
        await deviceClient.SendJsonAsync(temperatureResponse, cancellationToken);
    }
    
    private async Task<IActionResult> SendData(HttpRequest request, string device, CancellationToken cancellationToken)
    {
        if (!request.Query.TryGetValue("enabled", out var enabled))
        {
            return new BadRequestObjectResult("Missing 'enabled' query parameter.");
        }
        
        var enabledString = enabled.ToString();
        if (string.IsNullOrWhiteSpace(enabledString))
        {
            return new BadRequestObjectResult("Enabled cannot be empty.");
        }
        
        if (!bool.TryParse(enabledString, out var isEnabled))
        {
            return new BadRequestObjectResult("Invalid 'enabled' value. It must be 'true' or 'false'.");
        }

        var switchResponse = new SwitchResponse(isEnabled);
        
        var deviceClient = ioTHubDeviceClientFactory.GetClient(device);
        await deviceClient.SendJsonAsync(switchResponse, cancellationToken);

        return new OkObjectResult(switchResponse);
    }
}