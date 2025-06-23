using System.Globalization;
using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder api)
    {
        var usersGroup = api
            .MapGroup("users")
            .WithTags("Users")
            .RequireAdminAuthorization();
        
        usersGroup.MapGet(string.Empty, GetUsers)
            .WithSummary("Lista użytkowników z możliwością paginacji.");
        
        usersGroup.MapGet("{userId:int}", GetUser)
            .WithSummary("Pobiera użytkownika.");
        
        var administratorsGroup = usersGroup
            .MapGroup("administrators")
            .WithTags("Administrators");
        
        administratorsGroup.MapGet(string.Empty, GetAdministrators)
            .WithSummary("Lista administratorów z możliwością paginacji.");

        administratorsGroup.MapPut("{userId:int}", AddAdministrator)
            .WithSummary("Dodaje użytkownika do administratorów.");
        
        administratorsGroup.MapDelete("{userId:int}", RemoveAdministrator)
            .WithSummary("Usuwa użytkownika z administratorów.");
    }
    
    private static async Task<Ok<PaginatedResponse<UserResponse>>> GetUsers(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var users = await db.Users
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Email = x.Email!,
                EmailConfirmed = x.EmailConfirmed,
            })
            .ToPaginatedResponseAsync(skip, take, cancellationToken);
        
        return TypedResults.Ok(users);
    }
    
    private static async Task<Results<Ok<UserResponse>, NotFound>> GetUser(
        [FromRoute] int userId,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Where(x => x.Id == userId)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Email = x.Email!,
                EmailConfirmed = x.EmailConfirmed,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }
    
    private static async Task<Ok<PaginatedResponse<UserResponse>>> GetAdministrators(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var users = await db.Users
            .Where(x => db.UserRoles.Any(y => y.UserId == x.Id && y.RoleId == AppConsts.RoleAdminId))
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Email = x.Email!,
                EmailConfirmed = x.EmailConfirmed,
            })
            .ToPaginatedResponseAsync(skip, take, cancellationToken);
        
        return TypedResults.Ok(users);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> AddAdministrator(
        [FromRoute] int userId,
        [FromServices] UserManager<UserEntity> userManager)
    {
        var user = await userManager.FindByIdAsync(userId.ToString(CultureInfo.InvariantCulture));
        if (user == null)
        {
            return TypedResults.NotFound();
        }

        var result = await userManager.AddToRoleAsync(user, AppConsts.RoleAdmin);
        if (!result.Succeeded)
        {
            return result.CreateValidationProblem();
        }
        
        return TypedResults.NoContent();
    }
    
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> RemoveAdministrator(
        [FromRoute] int userId,
        [FromServices] UserManager<UserEntity> userManager)
    {
        var user = await userManager.FindByIdAsync(userId.ToString(CultureInfo.InvariantCulture));
        if (user == null)
        {
            return TypedResults.NotFound();
        }

        var result = await userManager.RemoveFromRoleAsync(user, AppConsts.RoleAdmin);
        if (!result.Succeeded)
        {
            return result.CreateValidationProblem();
        }
        
        return TypedResults.NoContent();
    }
}