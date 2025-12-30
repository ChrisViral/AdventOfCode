using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
            while (this.index < this.groups.Count)
            {
                current = this.groups[this.index++];
                if (!current.ValueSpan.IsEmpty) return true;
            }

            current = null!;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
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
