# Pyromaniac :fire:

A tiny piece of ASP.NET Core middleware that'll pseudo-randomly burn an API response to keep developers on their toes.

## Configuration

Pyromaniac uses your application's `IConfiguration` to get it's config values, so you'll have to create a `pyromaniac` section somewhere
in your config for it to work. 

### Via appsettings

Add a new `pyromaniac` object to your appsettings.json/appsettings.Development.json files.
```json
  {
      "pyromaniac": {
        "enabled": true,
        "invokeChance": 90
      }
  }
```

### Via separate config file

If you want to keep pyromaniac's config out of your appsettings (e.g. ensuring it doesn't make it to production by not including the file in the build), that's possible as well.

Create a new config file (e.g. `pyromaniac.json`) and add the config object;
```json
  {
  "pyromaniac": {
    "enabled": true,
    "invokeChance": 90
  }
}
```

Include the new config file in `Program.cs`
```csharp
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => 
                    config.AddJsonFile("pyromaniac.json", optional: true, reloadOnChange: true)) // Add this
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
```

As long as your configuration is accessible via `IConfiguration`, it doesn't matter what file format it's in.

## Using as middleware

Pyromaniac is intended to be run as a piece of ASP.NET Core middleware, to enable it add this to the `Configure()` method in `startup.cs`;
```csharp
app.UseMiddleware<Pyromaniac.Pyromaniac>();
```

If enabled in config, Pyromaniac will now start rolling the dice to see if it'll burn a HTTP response.