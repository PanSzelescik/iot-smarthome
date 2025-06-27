namespace IotSmartHome.Data.Dto;

public sealed record StateWithDateResponse
{
    public required Guid Id { get; init; }
    
    public required bool State { get; set; }
    
    public required DateTimeOffset CreatedDate { get; init; }
}