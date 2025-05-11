using IotSmartHome.Data;
using IotSmartHome.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class TemperatureEndpoints
{
    public static void UseTemperatureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("temperature")
            .WithTags("Temperature")
            .RequireAuthorization();

        group.MapGet("{deviceId}", DeviceTemperatures);
        group.MapGet("{deviceId}/current", CurrentDeviceTemperature);
    }
    
    private static async Task<Ok<List<TemperatureEntity>>> DeviceTemperatures(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var temperatures = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(temperatures);
    }

    private static async Task<Results<Ok<TemperatureEntity>, NotFound>> CurrentDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        return temperature == null ? TypedResults.NotFound() : TypedResults.Ok(temperature);
    }
}