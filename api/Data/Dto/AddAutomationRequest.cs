using IotSmartHome.Data.Enums;

namespace IotSmartHome.Data.Dto;

public sealed record AddAutomationRequest
{
    public required string DeviceId { get; init; }
    
    public required double WhenState { get; init; }
    
    public required ConditionEnum WhenCondition { get; init; }
    
    public required string ThenDeviceId { get; init; }
    
    public required bool ThenState { get; init; }
}