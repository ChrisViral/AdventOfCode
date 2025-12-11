using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.Intcode.Input;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Networking;

/// <summary>
/// Networked queue input accepting packets
/// </summary>
[PublicAPI]
public class NetworkQueueInput : IInputProvider
{
    /// <summary>
    /// Next value to provide
    /// </summary>
    private long? next;
    /// <summary>
    /// Network packet queue
    /// </summary>
    private readonly ConcurrentQueue<Packet> packetQueue = new();

    /// <inheritdoc />
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.packetQueue.Count * 2 + (this.next.HasValue ? 1 : 0);
    }

    /// <summary>
    /// Creates a new NetworkQueueInput
    /// </summary>
    public NetworkQueueInput() { }

    /// <summary>
    /// Copies a NetworkQueueInput from another instance
    /// </summary>
    /// <param name="other"></param>
    public NetworkQueueInput(NetworkQueueInput other)
    {
        this.next  = other.next;
        this.packetQueue = new ConcurrentQueue<Packet>(other.packetQueue);
    }

    /// <inheritdoc />
    public bool TryGetInput(out long input)
    {
        if (this.next.HasValue)
        {
            input = this.next.Value;
            this.next = null;
            return true;
        }

        if (this.packetQueue.TryDequeue(out Packet packet))
        {
            this.next = packet.Y;
            input = packet.X;
            return true;
        }

        input = -1L;
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddValue(long value) => this.next = value;

    /// <summary>
    /// Receives an input packet
    /// </summary>
    /// <param name="packet">Packet received</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceivePacket(Packet packet) => this.packetQueue.Enqueue(packet);

    /// <inheritdoc />
    public void FillInput(IEnumerable<long> values) => throw new NotSupportedException("NetworkInputQueue does not support receiving collections of values");

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.packetQueue.Clear();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IInputProvider Clone() => new NetworkQueueInput(this);
}
