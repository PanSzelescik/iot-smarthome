namespace IotSmartHome.Data.Dto;

public sealed record TemperatureWithDateResponse
{
    public required Guid Id { get; init; }
    
    public required double State { get; set; }
    
    public required DateTimeOffset CreatedDate { get; init; }
}