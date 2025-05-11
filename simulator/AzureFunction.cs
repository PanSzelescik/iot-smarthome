using Microsoft.Azure.Functions.Worker;

namespace IotSmartHome.Function;

public class AzureFunction(HomeAssistantClient homeAssistantClient, IoTHubDeviceClientFactory ioTHubDeviceClientFactory)
{
    [Function("PokojSzymona")]
    public async Task PokojSzymona([TimerTrigger("0 */15 * * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        await FetchAndSendData("sensor.pokoj_szymona_termometr_temperature", "pokoj_szymona", cancellationToken);
    }
    
    [Function("Salon")]
    public async Task Salon([TimerTrigger("0 */15 * * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        await FetchAndSendData("sensor.salon_termometr_temperature", "salon", cancellationToken);
    }

    private async Task FetchAndSendData(string sensor, string device, CancellationToken cancellationToken)
    {
        var homeAssistantStateResponse = await homeAssistantClient.FetchData(sensor, cancellationToken);
        var temperatureResponse = new TemperatureResponse(homeAssistantStateResponse);
        
        var deviceClient = ioTHubDeviceClientFactory.GetClient(device);
        await deviceClient.SendJsonAsync(temperatureResponse, cancellationToken);
    }
}