using System.Collections.Immutable;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Tasks;

namespace AdventOfCode.Intcode.IO.Networking;

/// <summary>
/// Intcode NAT (Not Always Transmitting)
/// </summary>
public sealed class NAT : IDisposable
{
    /// <summary>
    /// Time in ms before the network is considered idle
    /// </summary>
    private const int IDLE_TIMER  = 30;
    /// <summary>
    /// NAT storage address
    /// </summary>
    private const int NAT_ADDRESS = 255;

    private readonly Lock storedPacketLock = new();
    private readonly CancellationTokenSource shutdownSource = new();
    private readonly Thread[] networkThreads;
    private readonly ImmutableArray<IntcodeVM> network;
    private readonly ManualResetEventSlim firstPacketEvent = new(false);
    private readonly Barrier startBarrier;
    private CancellationTokenSource idleSource = new();
    private Packet lastRelayed;

    /// <summary>
    /// Currently stored packet by the NAT
    /// </summary>
    public Packet StoredPacket
    {
        get
        {
            lock (this.storedPacketLock)
            {
                return field;
            }
        }
        private set
        {
            lock (this.storedPacketLock)
            {
                field = value;
            }
        }
    }

    /// <summary>
    /// Setup the network with the given VM template
    /// </summary>
    /// <param name="template">VM template to replicate</param>
    /// <param name="count">Amount of VMs to use</param>
    public NAT(IntcodeVM template, int count)
    {
        // Setup input
        template.Input = new NetworkQueueInput();
        template.Output = new NetworkQueueOutput(this);

        // Create network array
        ImmutableArray<IntcodeVM>.Builder computers = ImmutableArray.CreateBuilder<IntcodeVM>(count);
        computers.Add(template);

        // Fill network
        foreach (int _ in 1..count)
        {
            computers.Add(new IntcodeVM(template));
        }

        // Pass ID to computers
        this.network = computers.ToImmutable();
        foreach (int i in ..count)
        {
            this.network[i].Input.AddValue(i);
        }

        // Create thread array
        this.networkThreads = new Thread[count];
        this.startBarrier = new Barrier(count + 1);

        foreach (int i in ..count)
        {
            // Setup threads
            IntcodeVM computer = this.network[i];
            Thread networkThread = new(() =>
            {
                this.startBarrier.SignalAndWait(this.shutdownSource.Token);
                computer.Run(this.shutdownSource.Token);
            });
            this.networkThreads[i] = networkThread;
        }
    }

    /// <summary>
    /// Starts the network worker VMs
    /// </summary>
    public void Start()
    {
        // Start network
        this.networkThreads.ForEach(t => t.Start());
        this.startBarrier.SignalAndWait(this.shutdownSource.Token);

        // Monitor traffic
        MonitorTraffic(this.shutdownSource.Token).Forget();
    }

    /// <summary>
    /// Monitors network traffic and transmits packets to restore communications as needed
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task MonitorTraffic(CancellationToken cancellationToken)
    {
        do
        {
            // Wait until the network is idle
            await WaitForIdle(IDLE_TIMER).ConfigureAwait(false);

            if (this.lastRelayed.Y == this.StoredPacket.Y)
            {
                // Duplicate Y value on packets, exit
                await this.shutdownSource.CancelAsync().ConfigureAwait(false);
            }
            else
            {
                // Transmit last stored packet to computer 0
                this.lastRelayed = this.StoredPacket;
                TransmitPacket(0, this.StoredPacket);
            }
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// Waits for the network to be idle for a set amount of time
    /// </summary>
    /// <param name="idleTime">Time for the network to be considered idle, in ms</param>
    private async Task WaitForIdle(int idleTime)
    {
        while (true)
        {
            try
            {
                // Wait until network is idle
                await Task.Delay(idleTime, this.idleSource.Token).ConfigureAwait(false);
                return;
            }
            catch (OperationCanceledException)
            {
                // Replace idle source and dispose old one
                CancellationTokenSource oldSource = Interlocked.Exchange(ref this.idleSource, new CancellationTokenSource());
                oldSource.Dispose();
            }
        }
    }

    /// <summary>
    /// Waits for the first NAT packet to come through
    /// </summary>
    public void WaitForFirstPacket() => this.firstPacketEvent.Wait(this.shutdownSource.Token);

    /// <summary>
    /// Waits for the network to complete
    /// </summary>
    public void WaitForCompletion()
    {
        // Join all network threads
        foreach (Thread networkThread in this.networkThreads)
        {
            networkThread.Join();
        }
    }

    /// <summary>
    /// Transmits a packet through the network
    /// </summary>
    /// <param name="address">Destination address</param>
    /// <param name="packet">Packet data</param>
    public void TransmitPacket(int address, Packet packet)
    {
        // Reset the idle timer
        this.idleSource.Cancel();
        if (address is NAT_ADDRESS)
        {
            // Keep packet for NAT
            this.StoredPacket = packet;
            this.firstPacketEvent.Set();
            return;
        }

        // Send the packet to the target computer
        IntcodeVM computer = this.network[address];
        NetworkQueueInput networkInput = (NetworkQueueInput)computer.Input;
        networkInput.ReceivePacket(packet);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.shutdownSource.Dispose();
        this.idleSource.Dispose();
        this.startBarrier.Dispose();
        this.firstPacketEvent.Dispose();
    }
}
