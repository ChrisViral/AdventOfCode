using System.Diagnostics.CodeAnalysis;
using AdventOfCode.API;
using Challenge.CLI;
using Challenge.Solvers;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AdventOfCode;

/// <summary>
/// Solver resolver and input fetcher
/// </summary>
/// <param name="logger">Logger instance</param>
/// <param name="settings">Resolver settings</param>
/// <param name="api">Advent of Code web API</param>
[PublicAPI, SolverTable]
public sealed partial class AdventOfCodeResolver(ILogger<AdventOfCodeResolver> logger, ResolverSettings settings, IAdventOfCodeAPI api)
    : DefaultSolverResolverBase(logger, settings)
{
    /// <inheritdoc />
    public override string ChallengeName => "Advent of Code";

    /// <inheritdoc />
    protected override TimeSpan RateLimit { get; } = TimeSpan.FromSeconds(900);

    /// <summary>
    /// Advent of Code API
    /// </summary>
    private IAdventOfCodeAPI API { get; } = api;

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Always thrown by this method</exception>
    [DoesNotReturn]
    public override Task<Result> SubmitAnswer(string answer, SolverData data, CancellationToken token = default) => throw new NotSupportedException("Advent of Code API does not support submitting answers automatically");

    /// <inheritdoc />
    protected override string GetInputFileName(in SolverData data) => Path.Combine(INPUT_FOLDER, data.Year.ToString(), $"day{data.Day:D2}.txt");

    /// <inheritdoc />
    protected override async Task<Result<string>> GetInputFromAPI(SolverData data, CancellationToken token) => await this.API.GetInput(data.Year, data.Day, token);
}
