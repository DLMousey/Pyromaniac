# Pyromaniac :fire:

A tiny piece of ASP.NET Core middleware that'll pseudo-randomly burn an API response to keep developers on their toes.
When Pyromaniac decides to burn a response it'll return the following instead;
```json
{
  "Status":500,
  "Message":"Pyromaniac burned this response",
  "Data": {
    "invokeChance":90,
    "rolledValue":22
  }
}
```

Support for custom response shapes is on the nice-to-have list for the future so you can integrate non-standard response envelopes.

## Feature wishlist (PRs more than welcome)

- [ ] Optional support for long/unhelpful error messages, useful for detection of raw error responses being displayed
- [ ] Pseudo-random content type changes, expecting json? too bad!
- [ ] Psuedo-random HTTP status code selection, make sure your error handling logic isn't only checking the status code

## Installation

### Mac and Linux 

Pyromaniac isn't available on NuGet just yet, but it can be installed locally. To install it;

- `git clone https://github.com/DLMousey/Pyromaniac`
- cd `Pyromaniac`
- `dotnet pack` + make a note of where the `nupkg` file is created
- Edit `~/.nuget/NuGet/NuGet.Config`
- Inside `<packageSources>` add the following;
```
<add key="pyromaniac-local" value="<PATH/TO/NUPKG/FILE>" />
```
- You should now be able to install Pyromaniac by `cd`ing to your project directory and running;
```
dotnet package add pyromaniac
```

### Windows install

I don't currently have a Windows machine to test installation on, PRs more than welcome with instructions

## Configuration

Pyromaniac uses your application's `IConfiguration` to get it's config values, so you'll have to create a `pyromaniac` section somewhere
in your config for it to work. 

### Via appsettings

Add a new `Pyromaniac` object to your appsettings.json/appsettings.Development.json files.
```json
  {
      "Pyromaniac": {
        "Enabled": true,
        "InvokeChance": 90,
        "LogLevel": "Information",
        "Verbose": true
      }
  }
```

_(nb: `LogLevel` and `Verbose` are optional - if verbose mode is not enabled no logging will happen)_

### Via separate config file

If you want to keep pyromaniac's config out of your appsettings (e.g. ensuring it doesn't make it to production by not including the file in the build), that's possible as well.

Create a new config file (e.g. `pyromaniac.json`) and add the config object;
```json
  {
  "Pyromaniac": {
    "Enabled": true,
    "InvokeChance": 90,
    "LogLevel": "Information",
    "Verbose": true
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
