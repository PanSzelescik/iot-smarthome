namespace IotSmartHome.Function;

public static class Consts
{
    public const string HomeAssistantToken = "HOME_ASSISTANT_TOKEN";
    
    public static string IoTHubDeviceConnectionString(string device) => $"IOTHUB_DEVICE_CONNECTION_STRING_{device.ToUpperInvariant()}";
}