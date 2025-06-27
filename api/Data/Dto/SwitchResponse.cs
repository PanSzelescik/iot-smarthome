namespace IotSmartHome.Data.Dto;

public sealed record SwitchResponse
{
    public required bool Enabled { get; init; }
}