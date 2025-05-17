using IotSmartHome.Data;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
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
        group.MapGet("{deviceId}/average", AverageDeviceTemperature);
        group.MapGet("{deviceId}/min", MinDeviceTemperature);
        group.MapGet("{deviceId}/max", MaxDeviceTemperature);
    }
    
    private static async Task<Ok<List<TemperatureEntity>>> DeviceTemperatures(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var temperatures = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(temperatures);
    }

    private static async Task<Results<Ok<TemperatureEntity>, NotFound>> CurrentDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var temperatureEntity = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        return temperatureEntity == null ? TypedResults.NotFound() : TypedResults.Ok(temperatureEntity);
    }
    
    private static async Task<Results<Ok<double>, NotFound>> AverageDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var date = DateTimeOffset.UtcNow.AddDays(-7);

        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId && x.CreatedDate >= date)
            .Select(x => (double?)x.State) // <- rzutowanie na nullable
            .DefaultIfEmpty()
            .AverageAsync(cancellationToken);
        
        return temperature == null ? TypedResults.NotFound() : TypedResults.Ok(temperature.Value);
    }
    
    private static async Task<Results<Ok<double>, NotFound>> MinDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var date = DateTimeOffset.UtcNow.AddDays(-7);

        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId && x.CreatedDate >= date)
            .Select(x => (double?)x.State) // <- rzutowanie na nullable
            .DefaultIfEmpty()
            .MinAsync(cancellationToken);
        
        return temperature == null ? TypedResults.NotFound() : TypedResults.Ok(temperature.Value);
    }
    
    private static async Task<Results<Ok<double>, NotFound>> MaxDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var date = DateTimeOffset.UtcNow.AddDays(-7);

        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId && x.CreatedDate >= date)
            .Select(x => (double?)x.State) // <- rzutowanie na nullable
            .DefaultIfEmpty()
            .MaxAsync(cancellationToken);
        
        return temperature == null ? TypedResults.NotFound() : TypedResults.Ok(temperature.Value);
    }

}