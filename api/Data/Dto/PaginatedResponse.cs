namespace IotSmartHome.Data.Dto;

public sealed record PaginatedResponse<TResult>
{
    public required int TotalCount { get; set; }
    public required List<TResult> Results { get; set; } = [];
}