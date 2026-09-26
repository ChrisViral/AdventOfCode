using System.Reflection;
using AdventOfCode.API;
using Challenge.CLI;
using Challenge.Utils.Extensions.Assemblies;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AdventOfCode.Resolver;

/// <summary>
/// Advent of Code program setup class
/// </summary>
public sealed class AdventOfCodeSetup()
    : SolverSetup<AdventOfCodeSettings, AdventOfCodeResolver>("Advent of Code")
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        // Setup user agent value
        Version fileVersion = Assembly.GetExecutingAssembly().GetFileVersion;
        string userAgent = $"ChrisViral.{typeof(AdventOfCodeResolver).FullName}/{fileVersion.ToString(2)} (https://github.com/ChrisViral/AdventOfCode)";

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
