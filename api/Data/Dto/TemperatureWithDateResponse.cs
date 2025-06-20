namespace IotSmartHome.Data.Dto;

public sealed record TemperatureWithDateResponse
{
    public required Guid Id { get; init; }
    
    public required double State { get; init; }
    
    public required DateTimeOffset CreatedDate { get; init; }
}