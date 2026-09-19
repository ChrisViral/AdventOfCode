using Microsoft.Extensions.Logging;

namespace AdventOfCode.Resolver;

internal partial class SolverResolver
{
    [LoggerMessage(LogLevel.Error, "Only {Seconds:F0} seconds elapsed since last request, please wait at least 900 seconds")]
    static partial void LogRateLimited(ILogger logger, double seconds);

    [LoggerMessage(LogLevel.Information, "Cached input fetched from {FileName}")]
    static partial void LogCachedInputLoaded(ILogger logger, string fileName);
}
