using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdventOfCode.Extensions.Assemblies;
using JetBrains.Annotations;

namespace AdventOfCode;

/// <summary>
/// Input fetching helper class
/// </summary>
[PublicAPI]
public static partial class InputFetcher
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
    private static readonly string SettingsPath = Path.Join(INPUT_FOLDER, "settings.json");

    /// <summary>
    /// Gets the associated input file, or fetches it from the AoC website if needed
    /// </summary>
    /// <param name="year">Event year</param>
    /// <param name="day">Problem day</param>
    /// <returns>The Input file for the problem</returns>
    public static async Task<string> EnsureInput(int year, int day)
    {
        //Check for the input file
        FileInfo inputFile = new(Path.Combine(INPUT_FOLDER, year.ToString(), $"day{day:D2}.txt"));
        string input;
        if (inputFile.Exists)
        {
            using StreamReader reader = inputFile.OpenText();
            input = await reader.ReadToEndAsync();
        }
        else
        {
            //Make sure the directory exists
            if (!inputFile.Directory?.Exists ?? false)
            {
                inputFile.Directory!.Create();
            }

            //Get input and write to file
            input = await GetInputFromWebsite(year, day);
            await using StreamWriter writer = inputFile.CreateText();
            await writer.WriteAsync(input);
        }

#if DEBUG
        //Additionally write to project if in debug
        await CopyFileToProject(inputFile, Path.Combine(INPUT_FOLDER, year.ToString()), overwrite: false);
#endif

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
    private static async Task<string> GetInputFromWebsite(int year, int day)
    {
        // Check if settings exist
        FileInfo settingsFile = new(SettingsPath);
        if (!settingsFile.Exists)
        {
            // Create empty settings file
            await using (FileStream emptyFileWriteStream = settingsFile.Create())
            {
                await JsonSerializer.SerializeAsync(emptyFileWriteStream, default, SettingsJsonContext.Default.Settings);
            }

#if DEBUG
            // Copy to project folder
            await CopyFileToProject(settingsFile, INPUT_FOLDER);
#endif

            // Prompt user to add cookie to file
            await Console.Error.WriteLineAsync("Could not find the input fetcher settings file, please add your cookie to the generated file.\n" + settingsFile.FullName);
            throw new FileNotFoundException("Could not find input fetcher settings file", settingsFile.FullName);
        }

        // Get settings
        Settings settings;
        await using (FileStream settingsReadFileStream = settingsFile.OpenRead())
        {
            settings = await JsonSerializer.DeserializeAsync(settingsReadFileStream, SettingsJsonContext.Default.Settings);
        }

        // Validate rate limit
        TimeSpan timeSinceLastRequest = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(settings.LastRequestTimestamp);
        if (timeSinceLastRequest.TotalSeconds < 900d)
        {
            await Console.Error.WriteLineAsync($"Only {timeSinceLastRequest.TotalSeconds:F0} seconds elapsed since last request, please wait at least 900 seconds.");
            throw new InvalidOperationException("Request rate limited");
        }

        // Create client
        using HttpClient client = new();
        client.BaseAddress = new Uri(BASE_URL);

        // Add cookie header
        client.DefaultRequestHeaders.Add("cookie", "session=" + settings.Cookie);

        // Add User-Agent header
        Version fileVersion = Assembly.GetExecutingAssembly().GetFileVersion;
        string userAgentValue = $"ChrisViral.{typeof(InputFetcher).FullName}Bot/{fileVersion.ToString(2)} (github.com/ChrisViral/AdventOfCode by christophe_savard@hotmail.ca)";
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgentValue);

        // Fetch input
        using HttpResponseMessage response = await client.GetAsync($"{year}/day/{day}/input");
        await using Stream responseStream  = await response.Content.ReadAsStreamAsync();
        using StreamReader responseReader  = new(responseStream, Encoding.UTF8);

        // Write back settings with new timestamp
        await using (FileStream settingsWriteFileStream = settingsFile.OpenWrite())
        {
            Settings updatedSettings = settings with { LastRequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
            await JsonSerializer.SerializeAsync(settingsWriteFileStream, updatedSettings, SettingsJsonContext.Default.Settings);
        }

#if DEBUG
        // Copy to project folder
        await CopyFileToProject(settingsFile, INPUT_FOLDER);
#endif

        // Return fetched input
        return await responseReader.ReadToEndAsync();
    }

    /// <summary>
    /// Copies the given file to the project folder
    /// </summary>
    /// <param name="sourceFile">Source file to copy</param>
    /// <param name="subfolder">Subfolder to copy the file to</param>
    /// <param name="overwrite">If existing files should be overwritten</param>
    private static async Task CopyFileToProject(FileInfo sourceFile, string subfolder, bool overwrite = true)
    {
        // Get target path in project
        string targetPath = Path.GetFullPath(Path.Combine("..", "..", "..", subfolder, sourceFile.Name));
        FileInfo targetFile = new(targetPath);

        // Don't clobber unless requested to
        if (!overwrite && targetFile.Exists) return;

        // Create directory if needed
        if (!targetFile.Directory!.Exists)
        {
            targetFile.Directory.Create();
        }

        // Copy file over
        byte[] fileData = await File.ReadAllBytesAsync(sourceFile.FullName);
        await File.WriteAllBytesAsync(targetPath, fileData);
    }
}
