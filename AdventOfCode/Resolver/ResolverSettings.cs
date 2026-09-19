using System.Text.Json.Serialization;

namespace AdventOfCode.Resolver;

/// <summary>
/// <see cref="ResolverSettings"/> JSON source generation context
/// </summary>
[JsonSerializable(typeof(ResolverSettings)), JsonSourceGenerationOptions(WriteIndented = true)]
internal sealed partial class ResolverSettingsJsonContext : JsonSerializerContext;

/// <summary>
/// Input fetcher settings struct
/// </summary>
/// <param name="Cookie">Request cookie</param>
/// <param name="LastRequestTimestamp">Last request timestamp</param>
[method: JsonConstructor]
internal record ResolverSettings(string Cookie, long LastRequestTimestamp)
{
    /// <summary>
    /// Last request timestamp
    /// </summary>
    public long LastRequestTimestamp { get; set; } = LastRequestTimestamp;
}
