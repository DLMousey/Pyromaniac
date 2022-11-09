using Pyromaniac.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Pyromaniac;

public class Pyromaniac
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly ILogger<Pyromaniac> _logger;

    // Catch used to stop things being logged multiple times
    private bool _logCatch = false;

    public Pyromaniac(RequestDelegate next, IConfiguration config, ILoggerFactory logger)
    {
        _next = next;
        _config = config;
        _logger = logger.CreateLogger<Pyromaniac>();
    }

    public async Task Invoke(HttpContext context)
    {
        IConfigurationSection pyroConfig = _config.GetSection("Pyromaniac");
        if (!pyroConfig.Exists())
        {
            LogOnce("Pyromaniac Is Not Configured - Skipping...");

            await _next(context);
            return;
        }

        if (!pyroConfig.GetValue<bool>("Enabled"))
        {
            LogOnce("Pyromaniac is disabled - skipping");

            await _next(context);
            return;
        }

        int invokeChance = pyroConfig.GetValue<int>("InvokeChance");
        int randomValue = new Random().Next(1, 100);

        ResponseEnvelope result = new()
        {
            Data = new
            {
                invokeChance,
                rolledValue = randomValue
            }
        };

        if (randomValue <= invokeChance)
        {
            LogIfAllowed($"Response Burned A Response - Rolled {randomValue}");

            context.Response.StatusCode = result.Status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));

            return;
        }

        await _next(context);
    }

    private void LogOnce(string message)
    {
        if (_logCatch)
        {
            return;
        }

        _logger.LogWarning(message);
        _logCatch = true;
    }

    private void LogIfAllowed(string message)
    {
        if (!_config.GetValue<bool>("Pyromaniac:verbose"))
        {
            return;
        }

        LogLevel permittedLogLevel;
        Enum.TryParse(_config.GetValue<string>("Pyromaniac:LogLevel", "Debug"), out permittedLogLevel);

        _logger.Log(permittedLogLevel, message);
    }
}