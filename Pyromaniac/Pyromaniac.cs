using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pyromaniac.DTOs.Outbound;
using Pyromaniac.Helpers;
using System.Text.Json;

namespace Pyromaniac;

public class Pyromaniac
{
    private readonly RequestDelegate _next;
    private readonly ILogger<Pyromaniac> _logger;

    // Catch Used To Stop Things Being Logged Multiple Times
    private bool _isLogCatched = false;

    public Pyromaniac(RequestDelegate next, ILoggerFactory logger)
    {
        _next = next;
        _logger = logger.CreateLogger<Pyromaniac>();
    }

    public async Task Invoke(HttpContext context)
    {
        IConfigurationSection pyroConfig = ConfigurationHelper.Configuration.GetSection("Pyromaniac");
        if (!pyroConfig.Exists())
        {
            LogOnce("Pyromaniac Is Not Configured In AppSettings | Skipping...");

            await _next(context);
            return;
        }

        if (!pyroConfig.GetValue<bool>("Enabled"))
        {
            LogOnce("Pyromaniac Is Disabled In AppSettings | Skipping...");

            await _next(context);
            return;
        }

        int invokeChance = pyroConfig.GetValue<int>("InvokeChance");
        int randomValue = new Random().Next(1, 100);

        ResponseOutputDto response = new()
        {
            Data = new InvokationChangeOutputDto
            {
                InvokeChange = invokeChance,
                RolledValue = randomValue
            }
        };

        if (randomValue <= invokeChance)
        {
            LogIfAllowed($"Pyromaniac Burned This Response - Rolled {randomValue}.");

            context.Response.StatusCode = StatusCodeHelper.Fetch();
            context.Response.ContentType = ContentTypeHelper.Fetch();
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            return;
        }

        await _next(context);
    }

    #region Private Methods & Functions

    private void LogOnce(string message)
    {
        if (_isLogCatched)
        {
            return;
        }

        _logger.LogWarning(message);
        _isLogCatched = true;
    }

    private void LogIfAllowed(string message)
    {
        if (!ConfigurationHelper.Configuration.GetValue<bool>("Pyromaniac:Verbose"))
            return;

        Enum.TryParse(ConfigurationHelper.Configuration.GetValue<string>("Pyromaniac:LogLevel", "Debug"), out LogLevel permittedLogLevel);

        _logger.Log(permittedLogLevel, message);
    }

    #endregion
}