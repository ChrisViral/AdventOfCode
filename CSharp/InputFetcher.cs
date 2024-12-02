using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode;

/// <summary>
/// Input fetching helper class
/// </summary>
public class InputFetcher : IDisposable
{
    #region Constants
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
    private static readonly string CookiePath = Path.Join(INPUT_FOLDER, "cookie.txt");
    #endregion

    #region Properties
    /// <summary>
    /// Input HTTP client
    /// </summary>
    private HttpClient Client { get; }
    #endregion

    #region Constructors
    public InputFetcher()
    {
        Client = new HttpClient { BaseAddress = new Uri(BASE_URL) };
        Client.DefaultRequestHeaders.Add("cookie", $"session={File.ReadAllText(CookiePath)}");
    }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the associated input file, or fetches it from the AoC website if needed
    /// </summary>
    /// <param name="year">Event year</param>
    /// <param name="day">Problem day</param>
    /// <returns>The Input file for the problem</returns>
    public async Task<string> EnsureInput(int year, int day)
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
            input = await GetInput(year, day);
            await using StreamWriter writer = inputFile.CreateText();
            await writer.WriteAsync(input);
        }

        #if DEBUG
        //Additionally write to project if in debug
        string debugFilePath = Path.GetFullPath(Path.Combine("..", "..", "..", INPUT_FOLDER, year.ToString(), inputFile.Name));
        FileInfo debugFile = new(debugFilePath);

        // ReSharper disable once InvertIf
        if (!debugFile.Exists)
        {
            if (!debugFile.Directory!.Exists)
            {
                debugFile.Directory.Create();
            }

            await using StreamWriter debugWriter = debugFile.CreateText();
            await debugWriter.WriteAsync(input);
        }
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
    private async Task<string> GetInput(int year, int day)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{year}/day/{day}/input");
        await using Stream responseStream  = await response.Content.ReadAsStreamAsync();
        using StreamReader responseReader  = new(responseStream, Encoding.UTF8);
        return await responseReader.ReadToEndAsync();
    }
    #endregion

    #region IDisposable
    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        this.Client.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}