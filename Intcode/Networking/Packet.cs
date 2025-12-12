namespace AdventOfCode.Intcode.Networking;

/// <summary>
/// Intcode network packet
/// </summary>
/// <param name="X">Packet X value</param>
/// <param name="Y">Packet Y value</param>
public readonly record struct Packet(long X, long Y);
