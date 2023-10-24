using Microsoft.Extensions.Configuration;

namespace Pyromaniac.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot Configuration { get; private set; }
        static ConfigurationHelper()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();
        }
    }
}
