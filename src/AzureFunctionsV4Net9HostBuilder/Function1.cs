using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsV4Net9HostBuilder;

public class Function1
{
    private readonly ILogger _logger;
    private readonly IHostEnvironment _env;

    public Function1(ILoggerFactory loggerFactory, IHostEnvironment env)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
        _env = env;
    }

    [Function("Function1")]
    public void Run([TimerTrigger("*/2 * * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("[{EnvironmentName}] C# Timer trigger function executed at: {DateTimeNow}", _env.EnvironmentName, DateTime.Now);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
