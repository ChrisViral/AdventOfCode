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
    : SolverSetup<AdventOfCodeSettings, AdventOfCodeResolver>(AdventOfCodeResolver.CHALLENGE_NAME)
{
    /// <inheritdoc />
    public override void ConfigureAPIClients(IServiceCollection services)
    {
        // Setup user agent value
        Version fileVersion = Assembly.GetExecutingAssembly().GetFileVersion;
        string userAgent = $"ChrisViral.{nameof(AdventOfCodeResolver)}/{fileVersion.ToString(2)} (https://github.com/ChrisViral/AdventOfCode)";
        string cookie = $"session={this.settings.Cookie}";

        // Add HTTP Clients
        services.AddRefitClient<IAdventOfCodeAPI>()
                .ConfigureHttpClient(client =>
                 {
                     // Set address and headers
                     client.BaseAddress = new Uri("https://adventofcode.com");
                     client.DefaultRequestHeaders.Add("cookie", cookie);
                     client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                 });
    }
}
