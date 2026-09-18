using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Challenge.CLI;
using Challenge.Utils.Extensions.Assemblies;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AdventOfCode;

/// <summary>
/// Solver resolver and input fetcher
/// </summary>
/// <param name="logger">Logger instance</param>
[PublicAPI]
public sealed partial class SolverResolver(ILogger<SolverResolver> logger) : ISolverResolver
{
    /// <summary>
    /// <see cref="Settings"/> JSON source generation context
    /// </summary>
    [JsonSerializable(typeof(Settings)), JsonSourceGenerationOptions(WriteIndented = true)]
    private sealed partial class SettingsJsonContext : JsonSerializerContext;

    /// <summary>
    /// Input fetcher settings struct
    /// </summary>
    /// <param name="Cookie">Request cookie</param>
    /// <param name="LastRequestTimestamp">Last request timestamp</param>
    [method: JsonConstructor]
    private readonly record struct Settings(string Cookie, long LastRequestTimestamp);

    /// <summary>
    /// Base Advent of Code URL
    /// </summary>
    private const string BASE_URL = "https://adventofcode.com/";
    /// <summary>
    /// Input folder name
    /// </summary>
    private const string INPUT_FOLDER = "Input";
    /// <summary>
    /// Session cookie file
    /// </summary>
    private static readonly string SettingsPath = Path.Combine(INPUT_FOLDER, "settings.json");

    /// <inheritdoc />
    public string ChallengeName => "Advent of Code";

    /// <summary>
    /// Logger instance
    /// </summary>
    private ILogger Logger { get; } = logger;

    /// <inheritdoc />
    public string GetSolverFullName(int year, int day, string module) => $"{nameof(AdventOfCode)}.AoC{year}.Day{day:D2}";

    /// <inheritdoc />
    public async Task<Result<string>> FetchInput(int year, int day, string module, CancellationToken token = default)
    {
        //Check for the input file
        FileInfo inputFile = new(Path.Combine(INPUT_FOLDER, year.ToString(), $"day{day:D2}.txt"));
        string input;
        if (inputFile.Exists)
        {
            using StreamReader reader = inputFile.OpenText();
            input = await reader.ReadToEndAsync(token).ConfigureAwait(false);
        }
        else
        {
            //Make sure the directory exists
            if (!inputFile.Directory?.Exists ?? false)
            {
                inputFile.Directory!.Create();
            }

            //Get input and write to file
            try
            {
                input = await GetInputFromWebsite(year, day).ConfigureAwait(false);
                await using StreamWriter writer = inputFile.CreateText();
                await writer.WriteAsync(input).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return Result.Failure<string>($"[{e.GetType().Name}]: {e.Message}\n{e.StackTrace}");
            }
        }

        //Return the fetched input
        return input;
    }

    /// <summary>
    /// Fetches the input from the AoC website
    /// </summary>
    /// <param name="year">Event year</param>
    /// <param name="day">Problem day</param>
    /// <returns>The input for the problem</returns>
    /// <exception cref="FileNotFoundException">If the settings file is not found</exception>
    /// <exception cref="InvalidOperationException">If the fetch is being rate limited</exception>
    private async Task<string> GetInputFromWebsite(int year, int day)
    {
        // Check if settings exist
        FileInfo settingsFile = new(SettingsPath);
        if (!settingsFile.Exists)
        {
            // Create empty settings file
            await using (FileStream emptyFileWriteStream = settingsFile.Create())
            {
                await JsonSerializer.SerializeAsync(emptyFileWriteStream, default, SettingsJsonContext.Default.Settings).ConfigureAwait(false);
            }

            // Prompt user to add cookie to file
            this.Logger.LogError("Could not find the input fetcher settings file, please add your cookie to the generated file.\n{FileName}", settingsFile.FullName);
            throw new FileNotFoundException("Could not find input fetcher settings file", settingsFile.FullName);
        }

        // Get settings
        Settings settings;
        await using (FileStream settingsReadFileStream = settingsFile.OpenRead())
        {
            settings = await JsonSerializer.DeserializeAsync(settingsReadFileStream, SettingsJsonContext.Default.Settings).ConfigureAwait(false);
        }

        // Validate rate limit
        TimeSpan timeSinceLastRequest = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(settings.LastRequestTimestamp);
        if (timeSinceLastRequest.TotalSeconds < 900d)
        {
            this.Logger.LogError("Only {Seconds:F0} seconds elapsed since last request, please wait at least 900 seconds", timeSinceLastRequest.TotalSeconds);
            throw new InvalidOperationException("Request rate limited");
        }

        // Create client
        using HttpClient client = new();
        client.BaseAddress = new Uri(BASE_URL);

        // Add cookie header
        client.DefaultRequestHeaders.Add("cookie", "session=" + settings.Cookie);

        // Add User-Agent header
        Version fileVersion = Assembly.GetExecutingAssembly().GetFileVersion;
        string userAgentValue = $"ChrisViral.{typeof(SolverResolver).FullName}Bot/{fileVersion.ToString(2)} (github.com/ChrisViral/AdventOfCode by christophe_savard@hotmail.ca)";
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgentValue);

        // Fetch input
        using HttpResponseMessage response = await client.GetAsync($"{year}/day/{day}/input").ConfigureAwait(false);
        await using Stream responseStream  = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using StreamReader responseReader  = new(responseStream, Encoding.UTF8);

        // Write back settings with new timestamp
        await using (FileStream settingsWriteFileStream = settingsFile.OpenWrite())
        {
            Settings updatedSettings = settings with { LastRequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
            await JsonSerializer.SerializeAsync(settingsWriteFileStream, updatedSettings, SettingsJsonContext.Default.Settings).ConfigureAwait(false);
        }

        // Return fetched input
        return await responseReader.ReadToEndAsync().ConfigureAwait(false);
    }
}
