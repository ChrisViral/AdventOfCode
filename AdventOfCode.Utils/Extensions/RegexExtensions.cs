using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Regexes;

/// <summary>
/// Regex extension methods
/// </summary>
[PublicAPI]
public static class RegexExtensions
{
    extension(Match match)
    {
        /// <summary>
        /// Gets all the captured groups of the match
        /// </summary>
        /// <value>Enumerable of the captured groups</value>
        public IEnumerable<Group> CapturedGroups
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => match.Groups
                        .Cast<Group>()
                        .Skip(1)
                        .Where(g => !string.IsNullOrEmpty(g.Value));
        }
    }
}
