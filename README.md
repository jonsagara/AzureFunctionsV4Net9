# Azure Functions v4 with `FunctionsApplicationBuilder` not reading environment name from `launchSettings.json`

This is a small reproduction of an issue where using the new `FunctionsApplicationBuilder` API causes the Azure Functions runtime to NOT load the environment specified in `launchSettings.json`. I noticed this after changing from using `HostBuilder` to using `FunctionsApplicationBuilder`. 

There are two projects:

- `AzureFunctionsV4Net9HostApplicationBuilder`: Built with `FunctionsApplicationBuilder`. Demonstrates the incorrect behavior of displaying the `EnvironmentName` as `Production`
- `AzureFunctionsV4Net9HostBuilder`: Built with `HostBuilder`. Demonstrates the correct behavior of displaying the `EnvironmentName` as `Development`.

Both projects have a single `TimerTrigger` function that prints out a message every two seconds. The message includes the `IHostEnvironment.EnvironmentName`. Here is the function code:

```csharp
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
```

Both projects also have a `launchSettings.json` file, specifying the environment name in the `AZURE_FUNCTIONS_ENVIRONMENT` environment variable. The files look like this:

```json
{
  "profiles": {
    "AzureFunctionsV4Net9HostBuilder": {
      "commandName": "Project",
      "commandLineArgs": "--port 7267",
      "environmentVariables": {
        "AZURE_FUNCTIONS_ENVIRONMENT": "Development"
      }
    }
  }
}
```


## Results

### AzureFunctionsV4Net9HostApplicationBuilder

When I run this function, I expect the environment to be `Development`. Instead, it's `Production`.

```
Azure Functions Core Tools
Core Tools Version:       4.0.6610 Commit hash: N/A +0d55b5d7efe83d85d2b5c6e0b0a9c1b213e96256 (64-bit)
Function Runtime Version: 4.1036.1.23224

[2024-11-19T05:22:18.459Z] Found C:\Dev\SANDBOX\AzureFunctionsV4Net9\src\AzureFunctionsV4Net9HostApplicationBuilder\AzureFunctionsV4Net9HostApplicationBuilder.csproj. Using for user secrets file configuration.
Skipping 'AZURE_FUNCTIONS_ENVIRONMENT' from local settings as it's already defined in current environment variables.
[2024-11-19T05:22:19.675Z] AzureFunctionsV4Net9HostApplicationBuilder Environment: Production
[2024-11-19T05:22:19.719Z] Worker process started and initialized.

Functions:

        Function1: timerTrigger

For detailed output, run func with --verbose flag.
[2024-11-19T05:22:20.054Z] Executing 'Functions.Function1' (Reason='Timer fired at 2024-11-18T21:22:20.0249607-08:00', Id=34079b07-a558-453b-85a2-bf7d56db1836)
[2024-11-19T05:22:20.140Z] [Production] C# Timer trigger function executed at: 11/18/2024 21:22:20
[2024-11-19T05:22:20.157Z] Executed 'Functions.Function1' (Succeeded, Id=34079b07-a558-453b-85a2-bf7d56db1836, Duration=128ms)
[2024-11-19T05:22:22.005Z] Executing 'Functions.Function1' (Reason='Timer fired at 2024-11-18T21:22:22.0017859-08:00', Id=f4cdb979-73a8-4501-a3b1-ada0ea5ac6d4)
[2024-11-19T05:22:22.019Z] [Production] C# Timer trigger function executed at: 11/18/2024 21:22:22
[2024-11-19T05:22:22.026Z] Executed 'Functions.Function1' (Succeeded, Id=f4cdb979-73a8-4501-a3b1-ada0ea5ac6d4, Duration=24ms)
```


### AzureFunctionsV4Net9HostBuilder

When I run this function, I expect the environment to be `Development`, and it is actually `Development`.

```
Azure Functions Core Tools
Core Tools Version:       4.0.6610 Commit hash: N/A +0d55b5d7efe83d85d2b5c6e0b0a9c1b213e96256 (64-bit)
Function Runtime Version: 4.1036.1.23224

[2024-11-19T05:24:15.087Z] Found C:\Dev\SANDBOX\AzureFunctionsV4Net9\src\AzureFunctionsV4Net9HostBuilder\AzureFunctionsV4Net9HostBuilder.csproj. Using for user secrets file configuration.
Skipping 'AZURE_FUNCTIONS_ENVIRONMENT' from local settings as it's already defined in current environment variables.
[2024-11-19T05:24:16.283Z] AzureFunctionsV4Net9HostBuilder Environment: Development
[2024-11-19T05:24:16.340Z] Worker process started and initialized.

Functions:

        Function1: timerTrigger

For detailed output, run func with --verbose flag.
[2024-11-19T05:24:18.045Z] Executing 'Functions.Function1' (Reason='Timer fired at 2024-11-18T21:24:18.0128755-08:00', Id=910e1740-ae93-4d67-9238-e693ceae4459)
[2024-11-19T05:24:18.153Z] [Development] C# Timer trigger function executed at: 11/18/2024 21:24:18
[2024-11-19T05:24:18.179Z] Executed 'Functions.Function1' (Succeeded, Id=910e1740-ae93-4d67-9238-e693ceae4459, Duration=160ms)
[2024-11-19T05:24:20.008Z] Executing 'Functions.Function1' (Reason='Timer fired at 2024-11-18T21:24:20.0048842-08:00', Id=f638196b-6b38-49e3-b77a-34025abd885c)
[2024-11-19T05:24:20.018Z] [Development] C# Timer trigger function executed at: 11/18/2024 21:24:20
[2024-11-19T05:24:20.022Z] Executed 'Functions.Function1' (Succeeded, Id=f638196b-6b38-49e3-b77a-34025abd885c, Duration=17ms)
```


## Development Information

- Windows 11 Version 23H2 (OS Build 22631.4460)
- Visual Studio 17.13.0 Preview 1.0
- .NET SDK 9.0.100
- Azure Functions v4
- Azure Functions Worker SDKs 2.0
- Azure Functions Worker Runtime: dotnet-isolated
