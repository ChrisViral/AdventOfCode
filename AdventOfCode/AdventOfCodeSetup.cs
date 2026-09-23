using System.Reflection;
using System.Text.Json;
using AdventOfCode.Resolver;
using Challenge.CLI;
using Challenge.Utils.Extensions.Assemblies;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;

namespace AdventOfCode;

/// <summary>
/// Advent of Code program setup class
/// </summary>
public sealed class AdventOfCodeSetup() : Setup<ResolverSettings>("Advent of Code")
{
    /// <inheritdoc />
    public override Task CreateDefaultSettings(FileStream fileStream, CancellationToken token)
    {
        return JsonSerializer.SerializeAsync(fileStream,
                                             new ResolverSettings(string.Empty, 0L),
                                             ResolverSettingsJsonContext.Default.ResolverSettings,
                                             token);
    }

    /// <inheritdoc />
    public override ValueTask<ResolverSettings?> GetSettings(FileStream fileStream, CancellationToken token)
    {
        return JsonSerializer.DeserializeAsync(fileStream,
                                               ResolverSettingsJsonContext.Default.ResolverSettings,
                                               token);
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        // Add services
        services.AddSingleton<ISolverResolver, SolverResolver>()
                .AddSingleton(this.settings)
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
                     client.DefaultRequestHeaders.Add("cookie", "session=" + this.settings.Cookie);
                     client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                 });
    }
}
