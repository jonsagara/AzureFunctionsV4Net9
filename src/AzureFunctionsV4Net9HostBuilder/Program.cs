using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
        //services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();
    });

using var host = builder.Build();

var env = host.Services.GetRequiredService<IHostEnvironment>();
Console.WriteLine($"{nameof(AzureFunctionsV4Net9HostBuilder)} Environment: {env.EnvironmentName}");

host.Run();
