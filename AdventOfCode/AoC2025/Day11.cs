using System.Collections.Frozen;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 11
/// </summary>
public sealed partial class Day11 : Solver<Dictionary<string, Day11.Device>>
{
    public sealed class Device(string id) : IEquatable<Device>
    {
        private readonly string id = id;

        public List<Device> Attached { get; } = [];

        public HashSet<Device> AttachedParents { get; } = [];

        /// <inheritdoc />
        public bool Equals(Device? other) => other?.id == this.id;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Device device && Equals(device);

        /// <inheritdoc />
        public override int GetHashCode() => this.id.GetHashCode();

        public override string ToString() => this.id;
    }

    private const string OUT = "out";

    private static readonly Dictionary<Device, int> Cache = new(1000);
    private static readonly Queue<Device> SearchQueue = new(1000);

    [GeneratedRegex(@"(\w{3}): ([\w ]+)")]
    private static partial Regex DeviceMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Device you = this.Data["you"];
        Device end = this.Data[OUT];

        int pathes = GetPathesCount(you, end);
        AoCUtils.LogPart1(pathes);

        Device svr = this.Data["svr"];
        Device dac = this.Data["dac"];
        Device fft = this.Data["fft"];

        FrozenSet<Device> dacParents = GetAllParents(dac);
        FrozenSet<Device> fftParents = GetAllParents(fft);

        long totalPathes;
        if (dacParents.Contains(fft))
        {
            totalPathes  = GetPathesCount(svr, fft, fftParents);
            totalPathes *= GetPathesCount(fft, dac, dacParents);
            totalPathes *= GetPathesCount(dac, end);
        }
        else
        {
            totalPathes  = GetPathesCount(svr, dac, dacParents);
            totalPathes *= GetPathesCount(dac, fft, fftParents);
            totalPathes *= GetPathesCount(fft, end);
        }

        AoCUtils.LogPart2(totalPathes);
    }

    private static FrozenSet<Device> GetAllParents(Device device)
    {
        HashSet<Device> parents = new(100) { device };
        foreach (Device parent in device.AttachedParents)
        {
            SearchQueue.Enqueue(parent);
            parents.Add(parent);
        }

        while (SearchQueue.TryDequeue(out Device? parent))
        {
            foreach (Device grandParent in parent.AttachedParents)
            {
                if (parents.Add(grandParent))
                {
                    SearchQueue.Enqueue(grandParent);
                }
            }
        }
        return parents.ToFrozenSet();
    }

    private static int GetPathesCount(Device start, Device end, FrozenSet<Device> parents)
    {
        int GetPathesFrom(Device current)
        {
            if (Cache.TryGetValue(current, out int pathes)) return pathes;

            pathes = 0;
            foreach (Device attached in current.Attached)
            {
                if (!parents.Contains(attached)) continue;

                pathes += !attached.Equals(end) ? GetPathesFrom(attached) : 1;
            }

            Cache[current] = pathes;
            return pathes;
        }

        int count = GetPathesFrom(start);
        Cache.Clear();
        return count;
    }

    private static int GetPathesCount(Device start, Device end)
    {
        int GetPathesFrom(Device current)
        {
            if (Cache.TryGetValue(current, out int pathes)) return pathes;

            pathes = 0;
            foreach (Device attached in current.Attached)
            {
                pathes += !attached.Equals(end) ? GetPathesFrom(attached) : 1;
            }

            Cache[current] = pathes;
            return pathes;
        }

        int count = GetPathesFrom(start);
        Cache.Clear();
        return count;
    }


    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Device> Convert(string[] rawInput)
    {
        Dictionary<string, string[]> attached = new(rawInput.Length + 1);
        Dictionary<string, Device> deviceMap  = new(rawInput.Length + 1);

        // Parse devices
        foreach (string line in rawInput)
        {
            Match match = DeviceMatcher.Match(line);
            string id = match.Groups[1].Value;
            deviceMap.Add(id, new Device(id));
            attached.Add(id, match.Groups[2].Value.Split(' '));
        }

        // Add out device
        deviceMap.Add(OUT, new Device(OUT));
        attached.Add(OUT, []);

        // Add attachments
        foreach ((string id, Device device) in deviceMap)
        {
            foreach (string attachedID in attached[id])
            {
                Device attachedDevice = deviceMap[attachedID];
                device.Attached.Add(attachedDevice);
                attachedDevice.AttachedParents.Add(device);
            }
        }

        return deviceMap;
    }
}
