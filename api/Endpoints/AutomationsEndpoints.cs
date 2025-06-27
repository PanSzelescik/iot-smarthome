using IotSmartHome.Data;
using IotSmartHome.Data.Dto;
using IotSmartHome.Data.Entities;
using IotSmartHome.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class AutomationsEndpoints
{
    public static void UseAutomationsEndpoints(this IEndpointRouteBuilder api)
    {
        var automationsGroup = api
            .MapGroup("automations")
            .WithTags("Automations")
            .RequireAuthorization();
        
        automationsGroup.MapGet(string.Empty, GetAutomations)
            .WithSummary("Lista automatyzacji z możliwością paginacji.");
        
        automationsGroup.MapPost(string.Empty, AddAutomation)
            .WithSummary("Dodanie nowej automatyzacji. Condition: 0 - Równa się, 1 - Nie równa się, 2 - Większe niż, 3 - Większe lub równe, 4 - Mniejsze niż, 5 - Mniejsze lub równe.");
        
        var automationGroup = automationsGroup
            .MapGroup("{automationId:int}");
        
        automationGroup.MapGet(string.Empty, GetAutomation)
            .WithSummary("Pobranie informacji o automatyzacji.")
            .WithName(nameof(GetAutomation));
        
        automationGroup.MapDelete(string.Empty, DeleteAutomation)
            .WithSummary("Usunięcie automatyzacji.");
    }
    
    private static async Task<Results<Ok<AutomationEntity>, NotFound>> GetAutomation(
        [FromRoute] int automationId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        var automation = await db.Automations
            .WhereIf(!isAdmin, x => x.UserThermometer.UserId == userId)
            .Include(x => x.UserThermometer)
            .Include(x => x.UserSwitch)
            .FirstOrDefaultAsync(x => x.Id == automationId, cancellationToken);

        return automation == null ? TypedResults.NotFound() : TypedResults.Ok(automation);
    }
    
    private static async Task<Ok<PaginatedResponse<AutomationEntity>>> GetAutomations(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        var automations = await db.Automations
            .WhereIf(!isAdmin, x => x.UserThermometer.UserId == userId)
            .Include(x => x.UserThermometer)
            .Include(x => x.UserSwitch)
            .ToPaginatedResponseAsync(skip, take, cancellationToken);
        
        return TypedResults.Ok(automations);
    }
    
    private static async Task<Results<CreatedAtRoute<AutomationEntity>, BadRequest<string>>> AddAutomation(
        [FromBody] AddAutomationRequest request,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetUserId();
        var isAdmin = httpContext.IsAdmin();

        var userThermometer = await db.UserThermometers
            .Where(x => x.DeviceId == request.DeviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userThermometer == null)
        {
            return TypedResults.BadRequest("This thermometer is not registered to your account or not exists.");
        }
        
        var userSwitch = await db.UserSwitches
            .Where(x => x.DeviceId == request.ThenDeviceId && x.IsAdmin)
            .WhereIf(!isAdmin, x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userSwitch == null)
        {
            return TypedResults.BadRequest("This switch is not registered to your account or not exists.");
        }
        
        var entity = new AutomationEntity
        {
            UserThermometer = userThermometer,
            WhenState = request.WhenState,
            WhenCondition = request.WhenCondition,
            UserSwitch = userSwitch,
            ThenState = request.ThenState,
        };
        
        await db.Automations.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return TypedResults.CreatedAtRoute(entity, nameof(GetAutomation), new { automationId = entity.Id });
    }
    
    private static async Task<Results<BadRequest<string>, NoContent>> DeleteAutomation(
        [FromRoute] int automationId,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var isAdmin = httpContext.IsAdmin();
        var userId = httpContext.GetUserId();
        
        var automationExists = await db.Automations
            .WhereIf(!isAdmin, x => x.UserThermometer.UserId == userId)
            .AnyAsync(x => x.Id == automationId, cancellationToken);

        if (!automationExists)
        {
            return TypedResults.BadRequest("This device is not registered to your account or not exists.");
        }
        
        var count = await db.Automations.Where(x => x.Id == automationId).ExecuteDeleteAsync(cancellationToken);
        return count > 0 ? TypedResults.NoContent() : TypedResults.BadRequest("This device is not registered to your account or not exists.");
    }
}