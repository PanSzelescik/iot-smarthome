using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class ThermometersEndpoints
{
    public static void UseThermometersEndpoints(this IEndpointRouteBuilder api)
    {
        var thermometersGroup = api
            .MapGroup("thermometers")
            .WithTags("Thermometers")
            .RequireAuthorization();

        thermometersGroup.MapGet(string.Empty, GetUserThermometers)
            .WithSummary("Lista termometrów z możliwością paginacji.");
        
        thermometersGroup.MapPost(string.Empty, AddThermometer)
            .WithSummary("Sparowanie nowego termometru.");
        
        var thermometerGroup = thermometersGroup
            .MapGroup("{deviceId}");
        
        thermometerGroup.MapGet(string.Empty, GetUserThermometer)
            .WithSummary("Pobranie informacji o termometrze.")
            .WithName(nameof(GetUserThermometer));
        
        thermometerGroup.MapDelete(string.Empty, DeleteThermometer)
            .WithSummary("Usunięcie sparowanego termometru.");

        thermometerGroup.MapPut("{userId:int}", AddThermometerToUser)
            .WithSummary("Dodanie dostepu do termometru dla innego użytkownika.");
        
        thermometerGroup.MapDelete("{userId:int}", DeleteThermometerFromUser)
            .WithSummary("Usunięcie dostępu do termometru przez innego użytkownika.");
        
        thermometerGroup.UseTemperaturesEndpoints();
    }

    private static async Task<Ok<PaginatedResponse<UserThermometerEntity>>> GetUserThermometers(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();

        var userThermometers = await db.UserThermometers
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .ToPaginatedResponseAsync(skip, take, cancellationToken);

        return TypedResults.Ok(userThermometers);
    }
    
    private static async Task<Results<Ok<UserThermometerEntity>, NotFound>> GetUserThermometer(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();

        var userThermometer = await db.UserThermometers
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return userThermometer == null ? TypedResults.NotFound() : TypedResults.Ok(userThermometer);
    }

    private static async Task<Results<CreatedAtRoute<UserThermometerEntity>, BadRequest<string>>> AddThermometer(
        [FromBody] AddThermometerRequest request,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var entity = new UserThermometerEntity
        {
            UserId = userId,
            DeviceId = request.DeviceId,
            FriendlyName = request.FriendlyName,
            IsAdmin = true,
            CreatedDate = DateTimeOffset.UtcNow,
        };
        await db.UserThermometers.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return TypedResults.CreatedAtRoute(entity, nameof(GetUserThermometer), new { deviceId = entity.DeviceId });
    }

    private static async Task<Results<BadRequest<string>, NoContent>> DeleteThermometer(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        if (!isAdmin)
        {
            var userThermometer = await db.UserThermometers
                .Where(x => x.DeviceId == deviceId && x.UserId == userId && x.IsAdmin)
                .FirstOrDefaultAsync(cancellationToken);
            if (userThermometer == null)
            {
                return TypedResults.BadRequest("This device is not registered to your account or not exists.");
            }
        }
        
        var count = await db.UserThermometers
            .Where(x => x.DeviceId == deviceId)
            .ExecuteDeleteAsync(cancellationToken);

        return count > 0 ? TypedResults.NoContent() : TypedResults.BadRequest("This device is not registered to your account or not exists.");
    }

    private static async Task<Results<BadRequest<string>, CreatedAtRoute<UserThermometerEntity>>> AddThermometerToUser(
        [FromRoute] string deviceId,
        [FromRoute(Name = "userId")] int userId2,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        var userThermometer = await db.UserThermometers
            .Where(x => x.DeviceId == deviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        if (userThermometer == null)
        {
            return TypedResults.BadRequest("This device is not registered to your account or not exists.");
        }
        
        var entity = new UserThermometerEntity
        {
            UserId = userId2,
            DeviceId = deviceId,
            FriendlyName = userThermometer.FriendlyName,
            IsAdmin = false,
            CreatedDate = DateTimeOffset.UtcNow,
        };
        await db.UserThermometers.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return TypedResults.CreatedAtRoute(entity, nameof(GetUserThermometer), new { deviceId = entity.DeviceId });
    }

    private static async Task<Results<BadRequest<string>, NoContent>> DeleteThermometerFromUser(
        [FromRoute] string deviceId,
        [FromRoute(Name = "userId")] int userId2,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        var userThermometerExists = await db.UserThermometers
            .Where(x => x.DeviceId == deviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .AnyAsync(cancellationToken);
        if (!userThermometerExists)
        {
            return TypedResults.BadRequest("This device is not registered to your account or not exists.");
        }
        
        var count = await db.UserThermometers
            .Where(x => x.DeviceId == deviceId && x.UserId == userId2)
            .ExecuteDeleteAsync(cancellationToken);
        return count > 0 ? TypedResults.NoContent() : TypedResults.BadRequest("This device is not registered to your account or not exists.");
    }
}