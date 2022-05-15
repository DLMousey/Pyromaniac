using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pyromaniac;

public class Pyromaniac
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public Pyromaniac(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        IConfigurationSection pyroConfig = _config.GetSection("pyromaniac");

        if (!pyroConfig.Exists())
        {
            await _next(context);
        }

        int invokeChance = pyroConfig.GetValue<int>("invokeChance");
        Random random = new Random();

        int randomValue = random.Next(1, 100);

        var result = new ResponseEnvelope
        {
            Data = new
            {
                invokeChance,
                rolledValue = randomValue
            }
        };

        if (randomValue <= invokeChance)
        {
            context.Response.StatusCode = result.Status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            return;
        }

        await _next(context);
    }

    private class ResponseEnvelope
    {
        public int Status { get; init; } = 500;

        public string Message { get; init; } = "Pyromaniac burned this response";

        public dynamic? Data { get; set; }
    }
}