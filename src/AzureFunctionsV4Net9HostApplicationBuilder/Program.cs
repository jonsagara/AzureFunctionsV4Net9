using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

using var host = builder.Build();

var env = host.Services.GetRequiredService<IHostEnvironment>();
Console.WriteLine($"{nameof(AzureFunctionsV4Net9HostApplicationBuilder)} Environment: {env.EnvironmentName}");

host.Run();
