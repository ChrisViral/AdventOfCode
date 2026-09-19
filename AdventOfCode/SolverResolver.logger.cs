using Microsoft.Extensions.Logging;

namespace AdventOfCode
{
    public partial class SolverResolver
    {
        [LoggerMessage(LogLevel.Error, "Could not find the input fetcher settings file, please add your cookie to the generated file.\n{FileName}")]
        static partial void LogSettingsFileNotFound(ILogger logger, string fileName);

        [LoggerMessage(LogLevel.Error, "Only {Seconds:F0} seconds elapsed since last request, please wait at least 900 seconds")]
        static partial void LogRateLimited(ILogger logger, double seconds);
    }
}