using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IotSmartHome.Endpoints;

public static class DevicesEndpoints
{
    public static void UseDevicesEndpoints(this IEndpointRouteBuilder api)
    {
        var devicesGroup = api
            .MapGroup("device")
            .WithTags("Devices")
            .RequireAuthorization();

        devicesGroup.MapGet(string.Empty, GetDevices)
            .WithSummary("Lista urządzeń z możliwością paginacji.");
        
        var deviceGroup = devicesGroup
            .MapGroup("{deviceId}");
        
        deviceGroup.UseTemperaturesEndpoints();
    }

    private static async Task<Ok<PaginatedResponse<string>>> GetDevices(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var devices = await db.Temperatures
            .Select(x => x.DeviceId)
            .Distinct()
            .Order()
            .ToPaginatedResponseAsync(skip, take, cancellationToken);

        return TypedResults.Ok(devices);
    }
}