using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommunityToolkit.HighPerformance;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Linq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Regexes;

/// <summary>
/// Regex extension methods
/// </summary>
[PublicAPI]
public static class RegexExtensions
{
    /// <summary>
    /// Regex captures enumerator
    /// </summary>
    /// <param name="groups">Regex group collection</param>
    public ref struct CapturesEnumerator(GroupCollection groups) : IValueEnumerator<Group>
    {
        private readonly GroupCollection groups = groups;
        private int index = 1;

        /// <inheritdoc />
        public bool TryGetNext(out Group current)
        {
            if (this.index == this.groups.Count)
            {
                current = null!;
                return false;
            }

            current = this.groups[this.index++];
            return true;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.groups.Count - 1;
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSpan(out ReadOnlySpan<Group> span)
        {
            span = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryCopyTo(scoped Span<Group> destination, Index offset) => false;

        /// <inheritdoc />
        public void Dispose() { }
    }

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
}
