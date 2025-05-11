namespace IotSmartHome.Data.Dto;

public sealed record TemperatureResponse
{
    public required double State { get; init; }
}