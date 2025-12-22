using System.Text;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using static System.Convert;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 16
/// </summary>
public sealed class Day16 : Solver<Day16.Packet>
{
    /// <summary>
    /// Packet operation type
    /// </summary>
    public enum PacketType : byte
    {
        SUM = 0,
        MUL = 1,
        MIN = 2,
        MAX = 3,
        VAL = 4,
        GTG = 5,
        LST = 6,
        EQU = 7
    }

    /// <summary>
    /// Packet object
    /// </summary>
    /// <param name="Version">Packet version</param>
    /// <param name="Type">Packet type</param>
    /// <param name="Size">Sub-packets size</param>
    public sealed record Packet(byte Version, PacketType Type, long Size)
    {
        /// <summary>Parsing StringBuilder</summary>
        private static readonly StringBuilder Builder = new();

        /// <summary>
        /// Sub-packets of this packet
        /// </summary>
        private List<Packet>? SubPackets { get; init; }

        /// <summary>
        /// Sum of the version of this packet and it's sub-packets
        /// </summary>
        public int VersionSum => Version + (this.SubPackets?.Sum(p => p.VersionSum) ?? 0);

        /// <summary>
        /// Value of this packet
        /// </summary>
        public long Value => this.Type switch
        {
            PacketType.SUM => this.SubPackets!.Sum(p => p.Value),
            PacketType.MUL => this.SubPackets!.Aggregate(1L, (t, p) => t * p.Value),
            PacketType.MIN => this.SubPackets!.Min(p => p.Value),
            PacketType.MAX => this.SubPackets!.Max(p => p.Value),
            PacketType.VAL => Size,
            PacketType.GTG => this.SubPackets![0].Value >  this.SubPackets[1].Value ? 1L : 0L,
            PacketType.LST => this.SubPackets![0].Value <  this.SubPackets[1].Value ? 1L : 0L,
            PacketType.EQU => this.SubPackets![0].Value == this.SubPackets[1].Value ? 1L : 0L,
            _              => throw this.Type.Invalid()
        };

        /// <summary>
        /// Parses the packet from the given binary string value
        /// </summary>
        /// <param name="bits">Binary string to parse the packet from</param>
        /// <returns>The parsed packet</returns>
        public static Packet ParsePacket(string bits) => ParsePacket(bits, 0).packet;

        /// <summary>
        /// Parses the packet from the given binary string value and starting position in the string
        /// </summary>
        /// <param name="bits">Binary string to parse the packet from</param>
        /// <param name="i">Current position within the string</param>
        /// <returns>The parsed packet, and the count of bits used while parsing</returns>
        /// ReSharper disable once CognitiveComplexity
        private static (Packet packet, int used) ParsePacket(string bits, int i)
        {
            byte version    = ToByte(bits[i..(i += 3)], 2);
            PacketType type = (PacketType)ToByte(bits[i..(i += 3)], 2);
            int used = 6;

            if (type is PacketType.VAL)
            {
                do
                {
                    Builder.Append(bits[(i + 1)..(i + 5)]);
                    i    += 5;
                    used += 5;
                }
                while (bits[i - 5] is not '0');

                long value = ToInt64(Builder.ToString(), 2);
                Builder.Clear();
                return (new Packet(version, type, value), used);
            }

            bool isLength = bits[i++] is '0';
            short length  = ToInt16(isLength ? bits[i..(i += 15)] : bits[i..(i += 11)], 2);
            Packet packet = new(version, type, length) { SubPackets = [] };
            used += isLength ? 16 : 12;

            if (isLength)
            {
                used += length;
                int totalSub = 0;
                do
                {
                    (Packet subPacket, int subUsed) = ParsePacket(bits, i);
                    packet.SubPackets.Add(subPacket);
                    totalSub += subUsed;
                    i        += subUsed;
                }
                while (totalSub < length);
            }
            else
            {
                packet.SubPackets.EnsureCapacity(length);
                foreach (int _ in ..length)
                {
                    (Packet subPacket, int subUsed) = ParsePacket(bits, i);
                    packet.SubPackets.Add(subPacket);
                    i    += subUsed;
                    used += subUsed;
                }
            }

            return (packet, used);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver for 2021 - 16 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.VersionSum);
        AoCUtils.LogPart2(this.Data.Value);
    }

    /// <summary>
    /// Convert a byte value to a binary string
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns></returns>
    private static string ToBinaryString(byte value) => System.Convert.ToString(value, 2).PadLeft(8, '0');

    /// <inheritdoc />
    protected override Packet Convert(string[] rawInput)
    {
        byte[] bytes = FromHexString(rawInput[0]);
        StringBuilder builder = new(bytes.Length * 4);
        string bits = builder.AppendJoin(string.Empty, bytes.Select(ToBinaryString))
                             .ToString();
        return Packet.ParsePacket(bits);
    }
}
