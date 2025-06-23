namespace IotSmartHome.Data.Dto;

public sealed record AddThermometerRequest
{
    public required string DeviceId { get; init; }
    
    public required string FriendlyName { get; init; }
}