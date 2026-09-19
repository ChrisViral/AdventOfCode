using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Challenge.CLI;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.Resolver;

/// <summary>
/// Solver resolver and input fetcher
/// </summary>
/// <param name="logger">Logger instance</param>
internal sealed partial class SolverResolver(ILogger<SolverResolver> logger, IAdventOfCodeApi api, ResolverSettings settings) : ISolverResolver
{
    /// <summary>
    /// Input folder name
    /// </summary>
    private const string INPUT_FOLDER = "Input";

    /// <summary>
    /// Session cookie file
    /// </summary>
    public static string SettingsPath { get; } = Path.Combine(INPUT_FOLDER, "settings.json");

    /// <inheritdoc />
    public string ChallengeName => "Advent of Code";

    /// <summary>
    /// Logger instance
    /// </summary>
    private ILogger Logger { get; } = logger;

    /// <summary>
    /// Advent of Code API
    /// </summary>
    private IAdventOfCodeApi API { get; } = api;

    /// <summary>
    /// Resolver settings
    /// </summary>
    private ResolverSettings Settings { get; } = settings;

    /// <inheritdoc />
    public async Task<Result<string, Exception>> FetchInput(uint year, uint day, string module, CancellationToken token = default)
    {
        // Check for the input file
        FileInfo inputFile = new(Path.Combine(INPUT_FOLDER, year.ToString(), $"day{day:D2}.txt"));
        if (inputFile.Exists)
        {
            // Read input from file
            using StreamReader reader = inputFile.OpenText();
            string input = await reader.ReadToEndAsync(token).ConfigureAwait(false);
            LogCachedInputLoaded(this.Logger, inputFile.FullName);
            return input;
        }

        // Make sure the directory exists
        if (inputFile.Directory is { Exists: false })
        {
            inputFile.Directory.Create();
        }

        try
        {
            // Fetch input and write to file
            string input = await GetInputFromWebsite(year, day, token).ConfigureAwait(false);
            await using StreamWriter writer = inputFile.CreateText();
            await writer.WriteAsync(input).ConfigureAwait(false);
            return input;
        }
        catch (Exception e)
        {
            // Return exception description in case of failure
            return Result.Failure<string, Exception>(e);
        }
    }

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Always thrown by this method</exception>
    [DoesNotReturn]
    public Task<Result> SubmitAnswer(string answer, CancellationToken token = default) => throw new NotSupportedException("Advent of Code API does not support submitting answers automatically");

    /// <summary>
    /// Fetches the input from the AoC website
    /// </summary>
    /// <param name="year">Event year</param>
    /// <param name="day">Problem day</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The input for the problem</returns>
    /// <exception cref="FileNotFoundException">If the settings file is not found</exception>
    /// <exception cref="InvalidOperationException">If the fetch is being rate limited</exception>
    private async Task<string> GetInputFromWebsite(uint year,uint day, CancellationToken token)
    {
        // Validate rate limit
        TimeSpan timeSinceLastRequest = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(this.Settings.LastRequestTimestamp);
        if (timeSinceLastRequest.TotalSeconds < 900d)
        {
            LogRateLimited(this.Logger, timeSinceLastRequest.TotalSeconds);
            throw new InvalidOperationException("Request rate limited");
        }

        // Fetch input
        string input = await this.API.GetInput(year, day, token).ConfigureAwait(false);

        // Write back settings with new timestamp
        FileInfo settingsFile = new(SettingsPath);
        await using FileStream settingsWriteFileStream = settingsFile.OpenWrite();
        this.Settings.LastRequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await JsonSerializer.SerializeAsync(settingsWriteFileStream, this.Settings, ResolverSettingsJsonContext.Default.ResolverSettings, token).ConfigureAwait(false);

        // Return fetched input
        return input;
    }
}
