using System.Globalization;
using IotSmartHome.Data;
using IotSmartHome.Data.Entities;
using IotSmartHome.Endpoints;
using IotSmartHome.Extensions;
using IotSmartHome.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var polishCultureInfo = new CultureInfo("pl-PL");
CultureInfo.CurrentCulture = polishCultureInfo;
CultureInfo.CurrentUICulture = polishCultureInfo;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetRequiredConnectionString("Postgres");
_ = builder.Configuration.GetRequiredConnectionString("EventHubCompatibleConnectionString");
_ = builder.Configuration.GetRequiredConnectionString("EventHubName");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(postgresConnectionString));

builder.Services
    .AddIdentity<UserEntity, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHostedService<IoTHubBackgroundService>();
builder.Services.AddSingleton<IEmailSender<UserEntity>, EmailSender>();

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
app.MapIdentityApi<UserEntity>();
app.UseTemperatureEndpoints();

await using var scope = app.Services.CreateAsyncScope();
await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync(app.Lifetime.ApplicationStopping);

app.Run();