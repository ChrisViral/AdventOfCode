using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils.ValueEnumerators;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Regexes;

/// <summary>
/// Regex extension methods
/// </summary>
[PublicAPI]
public static class RegexExtensions
{
    /// <param name="match">Match instance</param>
    extension(Match match)
    {
        /// <summary>
        /// Gets all the captured groups of the match
        /// </summary>
        /// <value>Enumerable of the captured groups</value>
        public ValueEnumerable<CapturesEnumerator, Group> CapturedGroups
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(new CapturesEnumerator(match.Groups));
        }
    }

    /// <param name="enumerator">ValueMatch enumerator</param>
    extension(Regex.ValueMatchEnumerator enumerator)
    {
        /// <summary>
        /// Gets a ValueEnumerable over this ValueMatchEnumerator
        /// </summary>
        /// <value>Enumerable of the ValueMatches</value>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueEnumerable<FromValueMatchEnumerator, MatchData> AsValueEnumerable() => new(new FromValueMatchEnumerator(enumerator));
    }
}
