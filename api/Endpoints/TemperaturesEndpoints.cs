using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using UnitsNet;
using UnitsNet.Units;

namespace IotSmartHome.Endpoints;

public static class TemperaturesEndpoints
{
    private const string TemperatureUnits =
        """
        Dostępne jednostki temperatury (parametr 'unit' w zapytaniu):
        1  - Stopnie Celsjusza (°C),
        2  - Stopnie Delisle’a (°De),
        3  - Stopnie Fahrenheita (°F),
        4  - Stopnie Newtona (°N),
        5  - Stopnie Rankine’a (°Ra),
        6  - Stopnie Réaumura (°Ré),
        7  - Stopnie Rømera (°Rø),
        8  - Kelwiny (K),
        9  - Milistopnie Celsjusza (m°C),
        10 - Temperatura słoneczna (Solar Temperature),
        """;
    
    public static void UseTemperaturesEndpoints(this IEndpointRouteBuilder thermometersGroup)
    {
        var temperaturesGroup = thermometersGroup
            .MapGroup("temperatures")
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
    
    private static async Task<Results<Ok<PaginatedResponse<TemperatureWithDateResponse>>, NotFound>> DeviceTemperatures(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        [FromQuery, SwaggerParameter(TemperatureUnits)] TemperatureUnit temperatureUnit = TemperatureUnit.DegreeCelsius)

    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

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
        
        foreach (var data in temperatures.Results)
        {
            data.State = Temperature
                .FromDegreesCelsius(data.State)
                .As(temperatureUnit);
        }

        return TypedResults.Ok(temperatures);
    }
    
    private static async Task<Results<Ok<int>, NotFound>> CountDeviceTemperatures(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

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
        CancellationToken cancellationToken,
        [FromQuery, SwaggerParameter(TemperatureUnits)] TemperatureUnit temperatureUnit = TemperatureUnit.DegreeCelsius)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }
        
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

        if (temperatureEntity == null)
        {
            return TypedResults.NotFound();
        }

        temperatureEntity.State = Temperature
            .FromDegreesCelsius(temperatureEntity.State)
            .As(temperatureUnit);
        
        return TypedResults.Ok(temperatureEntity);
    }
    
    private static async Task<Results<Ok<double>, NotFound>> AverageDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        [FromQuery, SwaggerParameter(TemperatureUnits)] TemperatureUnit temperatureUnit = TemperatureUnit.DegreeCelsius)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var temperature = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .Select(x => (double?)x.State) // <- rzutowanie na nullable
            .DefaultIfEmpty()
            .AverageAsync(cancellationToken);
        
        if (temperature == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(Temperature
            .FromDegreesCelsius(temperature.Value)
            .As(temperatureUnit));
    }
    
    private static async Task<Results<Ok<TemperatureWithDateResponse>, NotFound>> MinDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        [FromQuery, SwaggerParameter(TemperatureUnits)] TemperatureUnit temperatureUnit = TemperatureUnit.DegreeCelsius)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

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
        
        if (temperatureEntity == null)
        {
            return TypedResults.NotFound();
        }

        temperatureEntity.State = Temperature
            .FromDegreesCelsius(temperatureEntity.State)
            .As(temperatureUnit);
        
        return TypedResults.Ok(temperatureEntity);
    }
    
    private static async Task<Results<Ok<TemperatureWithDateResponse>, NotFound>> MaxDeviceTemperature(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        [FromQuery, SwaggerParameter(TemperatureUnits)] TemperatureUnit temperatureUnit = TemperatureUnit.DegreeCelsius)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserThermometers.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }
        
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
        
        if (temperatureEntity == null)
        {
            return TypedResults.NotFound();
        }

        temperatureEntity.State = Temperature
            .FromDegreesCelsius(temperatureEntity.State)
            .As(temperatureUnit);
        
        return TypedResults.Ok(temperatureEntity);
    }
}