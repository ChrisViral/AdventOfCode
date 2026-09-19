using Refit;

namespace AdventOfCode.Resolver;

/// <summary>
/// Advent of Code API
/// </summary>
internal interface IAdventOfCodeAPI
{
    /// <summary>
    /// Gets the challenge input for the given day
    /// </summary>
    /// <param name="year">Challenge year</param>
    /// <param name="day">Challenge day</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The puzzle input for that challenge</returns>
    [Get("/{year}/day/{day}/input")]
    Task<string> GetInput(uint year, uint day, CancellationToken token = default);
}
