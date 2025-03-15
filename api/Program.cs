using IotSmartHome.Database;
using IotSmartHome.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")!;
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(postgresConnectionString));

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => TypedResults.Redirect("/swagger/index.html")).ExcludeFromDescription(); // Redirect / to Swagger UI
app.UseStatesEndpoints();

await using var scope = app.Services.CreateAsyncScope();
await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync(app.Lifetime.ApplicationStopping);

await app.RunAsync();