using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Challenge.CLI;
using JetBrains.Annotations;

namespace AdventOfCode.Resolver;

/// <summary>
/// <see cref="AdventOfCodeSettings"/> JSON source generation context
/// </summary>
[PublicAPI, JsonSerializable(typeof(AdventOfCodeSettings)), JsonSourceGenerationOptions(WriteIndented = true)]
internal sealed partial class AdventOfCodeSettingsJsonContext : JsonSerializerContext;

/// <summary>
/// Advent of Code settings
/// </summary>
/// <param name="Cookie">Request cookie</param>
/// <param name="LastRequestTimestamp">Last request timestamp</param>
public sealed record AdventOfCodeSettings(string Cookie, long LastRequestTimestamp)
    : ResolverSettings(Cookie, LastRequestTimestamp),
      IResolverSettings<AdventOfCodeSettings>
{
    /// <inheritdoc />
    public static AdventOfCodeSettings Default { get; } = new(string.Empty, 0L);

    /// <inheritdoc />
    public static JsonTypeInfo<AdventOfCodeSettings> SettingsTypeInfo => AdventOfCodeSettingsJsonContext.Default.AdventOfCodeSettings;
}
