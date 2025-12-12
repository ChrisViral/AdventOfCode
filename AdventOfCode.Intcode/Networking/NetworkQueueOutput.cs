using System.Runtime.CompilerServices;
using AdventOfCode.Intcode.Output;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Networking;

/// <summary>
/// Networked queue output broadcasting packets
/// </summary>
[PublicAPI]
public sealed class NetworkQueueOutput : QueueOutput
{
    /// <summary>
    /// Network handler
    /// </summary>
    private readonly NAT network;

    /// <summary>
    /// Creates a new network queue output from a given nework
    /// </summary>
    /// <param name="network">Network handler</param>
    public NetworkQueueOutput(NAT network) => this.network = network;

    /// <summary>
    /// Creates a new newtwork queue output via copy
    /// </summary>
    /// <param name="other">Other network output to copy</param>
    public NetworkQueueOutput(NetworkQueueOutput other) : base(new Queue<long>(other.outputQueue))
    {
        this.network = other.network;
    }

    /// <inheritdoc />
    public override void AddOutput(long value)
    {
        if (this.Count < 2)
        {
            base.AddOutput(value);
            return;
        }

        int address = (int)GetValue();
        Packet packet = new(GetValue(), value);
        this.network.TransmitPacket(address, packet);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IOutputProvider Clone() => new NetworkQueueOutput(this);
}
