using AdventOfCode;
using Challenge.CLI;
using DotMake.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Console.Title = "Advent of Code";

// DI Configuration
Cli.Ext.ConfigureServices(services =>
{
    Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("results.txt")
                .Enrich.FromLogContext()
                .CreateLogger();

    services.AddSingleton<ISolverResolver, SolverResolver>()
            .AddLogging(builder => builder.AddSerilog(Log.Logger, true));
});

if (args is [])
{
    args = ["-h"];
}

#if DEBUG
// Don't wrap on debug to allow breakpoints
return await Cli.RunAsync<ChallengeCommand>(args).ConfigureAwait(false);
#else
try
{
    // Try running the command
    return await Cli.RunAsync<ChallengeCommand>(args).ConfigureAwait(false);
}
catch (Exception e)
{
    // Log exceptions
    Log.Error(e, "An error occured while executing the command");
    return 1;
}
#endif
