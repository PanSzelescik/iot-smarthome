using IotSmartHome.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Endpoints;

public static class StatesEndpoints
{
    public static void UseStatesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/state", async ([FromServices] ApplicationDbContext db) =>
        {
            var states = await db.States.ToListAsync();
            return Results.Ok(states);
        });
        
        app.MapGet("/state/{id:int}", async ([FromRoute] int id, [FromServices] ApplicationDbContext db) =>
        {
            var states = await db.States.FirstOrDefaultAsync(x => x.Id == id);
            return states == null ? Results.NotFound() : Results.Ok(states);
        });
    }
}