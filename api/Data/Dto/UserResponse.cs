namespace IotSmartHome.Data.Dto;

public sealed record UserResponse
{
    public required int Id { get; init; }
    
    public required string Email { get; init; }
    
    public required bool EmailConfirmed { get; init; }
}