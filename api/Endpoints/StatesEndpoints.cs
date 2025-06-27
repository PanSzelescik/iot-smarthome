using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class StatesEndpoints
{
    public static void UseStatesEndpoints(this IEndpointRouteBuilder switchesGroup)
    {
        var statesGroup = switchesGroup
            .MapGroup("states")
            .WithTags("States");

        statesGroup.MapGet("list", DeviceStates)
            .WithSummary("Lista statusów dla urządzenia od najnowszych do najstarszych z możliwością filtrowania po czasie i paginacji.");
        
        statesGroup.MapGet("count", CountDeviceStates)
            .WithSummary("Ilość statusów w bazie danych dla urządzenia z możliwością filtrowania po czasie.");
    }
    
    private static async Task<Results<Ok<PaginatedResponse<StateWithDateResponse>>, NotFound>> DeviceStates(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? before,
        [FromQuery] DateTimeOffset? after,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        if (!isAdmin && !await db.UserSwitches.AnyAsync(x => x.DeviceId == deviceId && x.UserId == userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var states = await db.Switches
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new StateWithDateResponse
            {
                Id = x.Id,
                State = x.State,
                CreatedDate = x.CreatedDate,
            })
            .ToPaginatedResponseAsync(skip, take, cancellationToken);

        return TypedResults.Ok(states);
    }
    
    private static async Task<Results<Ok<int>, NotFound>> CountDeviceStates(
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

        var states = await db.Temperatures
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(before.HasValue, x => x.CreatedDate < before)
            .WhereIf(after.HasValue, x => x.CreatedDate > after)
            .CountAsync(cancellationToken);

        return TypedResults.Ok(states);
    }
}