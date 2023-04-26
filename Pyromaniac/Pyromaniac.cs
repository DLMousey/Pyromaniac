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
    public static IConfiguration Configuratuion { get; private set; }

    // Catch Used To Stop Things Being Logged Multiple Times
    private bool _logCatch = false;

    public Pyromaniac(RequestDelegate next, IConfiguration configuration, ILoggerFactory logger)
    {
        _next = next;
        Configuratuion = configuration;
        _logger = logger.CreateLogger<Pyromaniac>();
    }

    public async Task Invoke(HttpContext context)
    {
        IConfigurationSection pyroConfig = Configuratuion.GetSection("Pyromaniac");
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
        if (_logCatch)
        {
            return;
        }

        _logger.LogWarning(message);
        _logCatch = true;
    }

    private void LogIfAllowed(string message)
    {
        if (!Configuratuion.GetValue<bool>("Pyromaniac:Verbose"))
            return;

        Enum.TryParse(Configuratuion.GetValue<string>("Pyromaniac:LogLevel", "Debug"), out LogLevel permittedLogLevel);

        _logger.Log(permittedLogLevel, message);
    }

    #endregion
}