using System.Globalization;
using IotSmartHome.Function;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var polishCultureInfo = new CultureInfo("pl-PL");
CultureInfo.CurrentCulture = polishCultureInfo;
CultureInfo.CurrentUICulture = polishCultureInfo;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IoTHubServiceClient>();
builder.Services.AddHttpClient();

builder.Build().Run();
