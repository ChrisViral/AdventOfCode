using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils;
using Challenge.Utils.Extensions.Enumerables;
using Challenge.Utils.Extensions.Strings;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 4
/// </summary>
[Solver(2016, 4)]
public sealed partial class Day04 : RegexSolver<Day04.Room>
{
    public class Room
    {
        public string ID { get; }

        public int Sector { get; }

        public ImmutableArray<char> Checksum { get; }

        public string DecryptedID { get; }

        public Room(string id, int sector, string checksum)
        {
            this.ID = id;
            this.Sector = sector;
            this.Checksum = checksum.Order().ToImmutableArray();
            using PooledArray<char> decryptedID = id.Select(c => c is not '-' ? ((c.AsIndex + sector) % StringUtils.LETTER_COUNT).AsAsciiLower : ' ').ToArrayPool();
            this.DecryptedID = decryptedID.Span.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => $"ID: {this.ID}, Sector: {this.Sector}, Checksum: {string.Join(string.Empty, this.Checksum)}";
    }

    /// <inheritdoc />
    [GeneratedRegex(@"([a-z\-]+)-(\d+)\[([a-z]{5})\]")]
    protected override partial Regex Matcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int result = 0;
        Counter<char> frequencies = new(26);
        foreach (Room room in this.Data)
        {
            room.ID.Where(c => c is not '-').AddTo(frequencies);
            bool isReal = frequencies.AsValueEnumerable()
                                     .OrderByDescending(p => p.Value)
                                     .ThenBy(p => p.Key)
                                     .Take(5)
                                     .OrderBy(p => p.Key)
                                     .Select(p => p.Key)
                                     .SequenceEqual(room.Checksum);
            if (isReal)
            {
                result += room.Sector;
            }
            frequencies.Clear();
        }
        LogAnswer(result);

        Room storage = this.Data.Single(r => r.DecryptedID.Contains("northpole"));
        LogAnswer(storage.Sector);
    }
}
