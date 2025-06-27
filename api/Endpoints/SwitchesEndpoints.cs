using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class SwitchesEndpoints
{
    public static void UseSwitchesEndpoints(this IEndpointRouteBuilder api)
    {
        var switchesGroup = api
            .MapGroup("switches")
            .WithTags("Switches")
            .RequireAuthorization();

        switchesGroup.MapGet(string.Empty, GetUserSwitches)
            .WithSummary("Lista przełączników z możliwością paginacji.");
        
        switchesGroup.MapPost(string.Empty, AddSwitch)
            .WithSummary("Sparowanie nowego przełącznika.");
        
        var switchGroup = switchesGroup
            .MapGroup("{deviceId}");
        
        switchGroup.MapGet(string.Empty, GetUserSwitch)
            .WithSummary("Pobranie informacji o przełączniku.")
            .WithName(nameof(GetUserSwitch));
        
        switchGroup.MapDelete(string.Empty, DeleteSwitch)
            .WithSummary("Usunięcie sparowanego przełącznika.");

        switchGroup.MapPut("{userId:int}", AddSwitchToUser)
            .WithSummary("Dodanie dostępu do przełącznika dla innego użytkownika.");
        
        switchGroup.MapDelete("{userId:int}", DeleteSwitchFromUser)
            .WithSummary("Usunięcie dostępu do przełącznika przez innego użytkownika.");
        
        switchGroup.UseStatesEndpoints();
    }

    private static async Task<Ok<PaginatedResponse<UserSwitchEntity>>> GetUserSwitches(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();

        var userSwitches = await db.UserSwitches
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .ToPaginatedResponseAsync(skip, take, cancellationToken);

        return TypedResults.Ok(userSwitches);
    }
    
    private static async Task<Results<Ok<UserSwitchEntity>, NotFound>> GetUserSwitch(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();

        var userSwitch = await db.UserSwitches
            .Where(x => x.DeviceId == deviceId)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return userSwitch == null ? TypedResults.NotFound() : TypedResults.Ok(userSwitch);
    }

    private static async Task<Results<CreatedAtRoute<UserSwitchEntity>, BadRequest<string>>> AddSwitch(
        [FromBody] AddSwitchRequest request,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();

        var entity = new UserSwitchEntity
        {
            UserId = userId,
            DeviceId = request.DeviceId,
            FriendlyName = request.FriendlyName,
            IsAdmin = true,
            CreatedDate = DateTimeOffset.UtcNow,
        };
        await db.UserSwitches.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return TypedResults.CreatedAtRoute(entity, nameof(GetUserSwitch), new { deviceId = entity.DeviceId });
    }

    private static async Task<Results<BadRequest<string>, NoContent>> DeleteSwitch(
        [FromRoute] string deviceId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        if (!isAdmin)
        {
            var userSwitch = await db.UserSwitches
                .Where(x => x.DeviceId == deviceId && x.UserId == userId && x.IsAdmin)
                .FirstOrDefaultAsync(cancellationToken);
            if (userSwitch == null)
            {
                return TypedResults.BadRequest("This device is not registered to your account or not exists.");
            }
        }
        
        var count = await db.UserSwitches
            .Where(x => x.DeviceId == deviceId)
            .ExecuteDeleteAsync(cancellationToken);

        return count > 0 ? TypedResults.NoContent() : TypedResults.BadRequest("This device is not registered to your account or not exists.");
    }

    private static async Task<Results<BadRequest<string>, CreatedAtRoute<UserSwitchEntity>>> AddSwitchToUser(
        [FromRoute] string deviceId,
        [FromQuery] string email,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        UserManager<UserEntity> userManager,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        var userSwitch = await db.UserSwitches
            .Where(x => x.DeviceId == deviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        if (userSwitch == null)
        {
            return TypedResults.BadRequest("This device is not registered to your account or not exists.");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return TypedResults.BadRequest("User with this email does not exist.");
        }
        
        var entity = new UserSwitchEntity
        {
            UserId = user.Id,
            DeviceId = deviceId,
            FriendlyName = userSwitch.FriendlyName,
            IsAdmin = false,
            CreatedDate = DateTimeOffset.UtcNow,
        };
        await db.UserSwitches.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return TypedResults.CreatedAtRoute(entity, nameof(GetUserSwitch), new { deviceId = entity.DeviceId });
    }

    private static async Task<Results<BadRequest<string>, NoContent>> DeleteSwitchFromUser(
        [FromRoute] string deviceId,
        [FromQuery] string email,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        var userSwitchExists = await db.UserSwitches
            .Where(x => x.DeviceId == deviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .AnyAsync(cancellationToken);
        if (!userSwitchExists)
        {
            return TypedResults.BadRequest("This device is not registered to your account or not exists.");
        }
        
        var count = await db.UserSwitches
            .Where(x => x.DeviceId == deviceId && x.User.Email == email)
            .ExecuteDeleteAsync(cancellationToken);
        return count > 0 ? TypedResults.NoContent() : TypedResults.BadRequest("This device is not registered to your account or not exists.");
    }
}