using System.Reflection;
using System.Text.Json;
using AdventOfCode.Resolver;
using Challenge.CLI;
using Challenge.Utils.Extensions.Assemblies;
using DotMake.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;
using Serilog.Events;

Console.Title = "Advent of Code";

// Setup cancellation token source
using CancellationTokenSource cancellationSource = new();
Console.CancelKeyPress += (_, _) =>
{
    // ReSharper disable once AccessToDisposedClosure
    cancellationSource.Cancel();
};

// Flush existing log file
string results = Path.Combine("Output", "results.txt");
if (File.Exists(results))
{
    File.Delete(results);
}

// Create logger
LoggerConfiguration configuration = new();
Log.Logger = configuration.WriteTo.Console()
                          .WriteTo.File(results)
                          .Enrich.FromLogContext()
#if DEBUG
                          .MinimumLevel.Debug()
                          .MinimumLevel.Override(typeof(HttpClient).FullName!, LogEventLevel.Information)
#else
                          .MinimumLevel.Information()
                          .MinimumLevel.Override(typeof(HttpClient).FullName!, LogEventLevel.Warning)
#endif
                          .CreateLogger();

// Ensure input directory exists
if (!Directory.Exists(SolverResolverBase.INPUT_FOLDER))
{
    Directory.CreateDirectory(SolverResolverBase.INPUT_FOLDER);
}

// Check if settings exist
FileInfo settingsFile = new(SolverResolverBase.SettingsPath);
if (!settingsFile.Exists)
{
    // Create empty settings file
    await using (FileStream emptyFileWriteStream = settingsFile.Create())
    {
        await JsonSerializer.SerializeAsync(emptyFileWriteStream,
                                            new ResolverSettings(string.Empty, 0L),
                                            ResolverSettingsJsonContext.Default.ResolverSettings,
                                            cancellationSource.Token)
                            .ConfigureAwait(false);
    }

    // Prompt user to add cookie to file
    Log.Error("Could not find the settings file, please add your cookie and to the generated file\n{FileName}", settingsFile.FullName);
    return 1;
}

// Get settings
ResolverSettings? settings;
await using (FileStream settingsReadFileStream = settingsFile.OpenRead())
{
    settings = await JsonSerializer.DeserializeAsync(settingsReadFileStream,
                                                     ResolverSettingsJsonContext.Default.ResolverSettings,
                                                     cancellationSource.Token)
                                   .ConfigureAwait(false);
}

if (settings is null)
{
    Log.Error("Could not deserialize settings file {FileName}", settingsFile.FullName);
    return 1;
}

// DI Configuration
Cli.Ext.ConfigureServices(services =>
{
    // Add services
    services.AddSingleton<ISolverResolver, SolverResolver>()
            .AddSingleton(settings)
            .AddLogging(builder => builder.AddSerilog(Log.Logger, true));

    // Setup user agent value
    Version fileVersion = Assembly.GetExecutingAssembly().GetFileVersion;
    string userAgent = $"ChrisViral.{typeof(SolverResolver).FullName}/{fileVersion.ToString(2)} (https://github.com/ChrisViral/EverybodyCodes)";

    // Add HTTP Clients
    services.AddRefitClient<IAdventOfCodeAPI>()
            .ConfigureHttpClient(client =>
             {
                 // Set address and headers
                 client.BaseAddress = new Uri("https://adventofcode.com");
                 client.DefaultRequestHeaders.Add("cookie", "session=" + settings.Cookie);
                 client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
             });
});

// Default args
if (args is [])
{
    args = ["-h"];
}

#if DEBUG
// Don't wrap on debug to allow breakpoints
return await Cli.RunAsync<ChallengeCommand>(args, cancellationToken: cancellationSource.Token).ConfigureAwait(false);
#else
try
{
    // Try running the command
    return await Cli.RunAsync<ChallengeCommand>(args, cancellationToken: cancellationSource.token).ConfigureAwait(false);
}
catch (Exception e)
{
    // Log exceptions
    Log.Error(e, "An error occured while executing the command");
    return 1;
}
#endif
