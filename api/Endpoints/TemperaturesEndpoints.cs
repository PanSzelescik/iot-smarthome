using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class TemperaturesEndpoints
{
    public static void UseTemperaturesEndpoints(this IEndpointRouteBuilder deviceGroup)
    {
        var temperaturesGroup = deviceGroup
            .MapGroup("temperature")
            .WithTags("Temperatures");

        temperaturesGroup.MapGet("list", DeviceTemperatures)
            .WithSummary("Lista temperatur dla urządzenia od najnowszych do najstarszych z możliwością filtrowania po czasie i paginacji.");
        
        temperaturesGroup.MapGet("count", CountDeviceTemperatures)
            .WithSummary("Ilość temperatur w bazie danych dla urządzenia z możliwością filtrowania po czasie.");
        
        temperaturesGroup.MapGet("current", CurrentDeviceTemperature)
            .WithSummary("Ostatnia temperatura urządzenia.");
        
        temperaturesGroup.MapGet("average", AverageDeviceTemperature)
            .WithSummary("Średnia temperatura urządzenia z możliwością filtrowania po czasie.");
        
        temperaturesGroup.MapGet("min", MinDeviceTemperature)
            .WithSummary("Minimalna temperatura urządzenia z możliwością filtrowania po czasie.");
        
        temperaturesGroup.MapGet("max", MaxDeviceTemperature)
            .WithSummary("Maksymalna temperatura urządzenia z możliwością filtrowania po czasie.");
    }
    
    private static async Task<Ok<PaginatedResponse<TemperatureWithDateResponse>>> DeviceTemperatures(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var temperatures = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new TemperatureWithDateResponse
            {
                Id = x.Id,
                State = x.State,
                CreatedDate = x.CreatedDate,
            })
            .ToPaginatedResponseAsync(skip, take, cancellationToken);

        return TypedResults.Ok(temperatures);
    }
    
    private static async Task<Ok<int>> CountDeviceTemperatures(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var temperatures = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .CountAsync(cancellationToken);

        return TypedResults.Ok(temperatures);
    }

    private static async Task<Results<Ok<TemperatureWithDateResponse>, NotFound>> CurrentDeviceTemperature(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var temperatureEntity = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new TemperatureWithDateResponse
            {
                Id = x.Id,
                State = x.State,
                CreatedDate = x.CreatedDate,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return temperatureEntity == null ? TypedResults.NotFound() : TypedResults.Ok(temperatureEntity);
    }
    
    private static async Task<Results<Ok<double>, NotFound>> AverageDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .Select(x => (double?)x.State) // <- rzutowanie na nullable
            .DefaultIfEmpty()
            .AverageAsync(cancellationToken);
        
        return temperature == null ? TypedResults.NotFound() : TypedResults.Ok(temperature.Value);
    }
    
    private static async Task<Results<Ok<TemperatureWithDateResponse>, NotFound>> MinDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var temperatureEntity = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .OrderBy(x => x.State)
            .Select(x => new TemperatureWithDateResponse
            {
                Id = x.Id,
                State = x.State,
                CreatedDate = x.CreatedDate,
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return temperatureEntity == null ? TypedResults.NotFound() : TypedResults.Ok(temperatureEntity);
    }
    
    private static async Task<Results<Ok<TemperatureWithDateResponse>, NotFound>> MaxDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        
        var temperatureEntity = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .OrderByDescending(x => x.State)
            .Select(x => new TemperatureWithDateResponse
            {
                Id = x.Id,
                State = x.State,
                CreatedDate = x.CreatedDate,
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return temperatureEntity == null ? TypedResults.NotFound() : TypedResults.Ok(temperatureEntity);
    }

}