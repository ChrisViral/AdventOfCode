using System.Collections.Immutable;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using JetBrains.Annotations;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 21
/// </summary>
public sealed class Day21 : Solver<Day21.Stats>
{
    public readonly record struct Stats(int HitPoints, int Damage, int Armour);

    private readonly record struct Item([UsedImplicitly] string Name, int Cost, int Damage, int Armour);

    private const int HIT_POINTS = 100;

    private static readonly ImmutableArray<Item> Weapons =
    [
        new("Dagger",     Cost: 8,  Damage: 4, Armour: 0),
        new("Shortsword", Cost: 10, Damage: 5, Armour: 0),
        new("Warhammer",  Cost: 25, Damage: 6, Armour: 0),
        new("Longsword",  Cost: 40, Damage: 7, Armour: 0),
        new("Greataxe",   Cost: 74, Damage: 8, Armour: 0)
    ];

    private static readonly ImmutableArray<Item> Armours =
    [
        new("Leather",    Cost: 13,  Damage: 0, Armour: 1),
        new("Chainmail",  Cost: 31,  Damage: 0, Armour: 2),
        new("Splintmail", Cost: 53,  Damage: 0, Armour: 3),
        new("Bandedmail", Cost: 75,  Damage: 0, Armour: 4),
        new("Platemail",  Cost: 102, Damage: 0, Armour: 5)
    ];

    private static readonly ImmutableArray<Item> Rings =
    [
        new("Damage +1",  Cost: 25,  Damage: 1, Armour: 0),
        new("Damage +2",  Cost: 50,  Damage: 2, Armour: 0),
        new("Damage +3",  Cost: 100, Damage: 3, Armour: 0),
        new("Defense +1", Cost: 20,  Damage: 0, Armour: 1),
        new("Defense +2", Cost: 40,  Damage: 0, Armour: 2),
        new("Defense +3", Cost: 80,  Damage: 0, Armour: 3)
    ];

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Stats player = new(HIT_POINTS, 0, 0);
        int cost = ChooseWeaponMinCost(player);
        AoCUtils.LogPart1(cost);

        cost = ChooseWeaponMaxCost(player);
        AoCUtils.LogPart2(cost);
    }

    private int ChooseWeaponMinCost(Stats player)
    {
        return Weapons.AsValueEnumerable()
                      .Min(weapon => ChooseArmourMinCost(player with { Damage = weapon.Damage }, weapon.Cost));
    }

    private int ChooseArmourMinCost(Stats player, int costSoFar)
    {
        if (WinsCombat(player)) return costSoFar;

        int noArmourCost = ChooseRingsMinCost(player, costSoFar);
        int withArmourMinCost = Armours.AsValueEnumerable()
                                       .Min(armour => ChooseRingsMinCost(player with { Armour = armour.Armour }, costSoFar + armour.Cost));
        return Math.Min(noArmourCost, withArmourMinCost);
    }

    private int ChooseRingsMinCost(Stats player, int costSoFar)
    {
        if (WinsCombat(player)) return costSoFar;

        int minCostOneRing = Rings.AsValueEnumerable()
                                  .Where(r => WinsCombat(player with { Damage = player.Damage + r.Damage, Armour = player.Armour + r.Armour }))
                                  .Select(r => r.Cost + costSoFar)
                                  .Append(int.MaxValue)
                                  .Min();
        int minCostTwoRings = Rings.EnumeratePairs()
                                   .AsValueEnumerable()
                                   .Where(p => WinsCombat(player with { Damage = player.Damage + p.first.Damage + p.second.Damage, Armour = player.Armour + p.first.Armour + p.second.Armour }))
                                   .Select(p => p.first.Cost + p.second.Cost + costSoFar)
                                   .Append(int.MaxValue)
                                   .Min();
        return Math.Min(minCostOneRing, minCostTwoRings);
    }

    private int ChooseWeaponMaxCost(Stats player)
    {
        return Weapons.AsValueEnumerable()
                      .Max(weapon => ChooseArmourMaxCost(player with { Damage = weapon.Damage }, weapon.Cost));
    }

    private int ChooseArmourMaxCost(Stats player, int costSoFar)
    {
        if (WinsCombat(player)) return int.MinValue;

        int noArmourCost = ChooseRingsMaxCost(player, costSoFar);
        int withArmourMaxCost = Armours.AsValueEnumerable()
                                       .Max(armour => ChooseRingsMaxCost(player with { Armour = armour.Armour }, costSoFar + armour.Cost));
        return Math.Max(noArmourCost, withArmourMaxCost);
    }

    private int ChooseRingsMaxCost(Stats player, int costSoFar)
    {
        if (WinsCombat(player)) return int.MinValue;

        int maxCostOneRing = Rings.AsValueEnumerable()
                                  .Where(r => !WinsCombat(player with { Damage = player.Damage + r.Damage, Armour = player.Armour + r.Armour }))
                                  .Select(r => r.Cost + costSoFar)
                                  .Append(int.MinValue)
                                  .Max();
        int maxCostTwoRings = Rings.EnumeratePairs()
                                   .AsValueEnumerable()
                                   .Where(p => !WinsCombat(player with { Damage = player.Damage + p.first.Damage + p.second.Damage, Armour = player.Armour + p.first.Armour + p.second.Armour }))
                                   .Select(p => p.first.Cost + p.second.Cost + costSoFar)
                                   .Append(int.MinValue)
                                   .Max();
        return Math.Max(maxCostOneRing, maxCostTwoRings);
    }

    private bool WinsCombat(Stats player)
    {
        int hitsToKillBoss    = CalculateHitsToKill(player, this.Data);
        int hitsForBossToKill = CalculateHitsToKill(this.Data, player);
        return hitsToKillBoss <= hitsForBossToKill;
    }

    private static int CalculateHitsToKill(Stats attacker, Stats attacked)
    {
        int damage = Math.Max(attacker.Damage - attacked.Armour, 1);
        return (attacked.HitPoints + damage - 1) / damage;
    }

    /// <inheritdoc />
    protected override Stats Convert(string[] rawInput)
    {
        Span<Range> splits = stackalloc Range[2];
        ReadOnlySpan<char> line = rawInput[0];
        line.Split(splits, ':', StringSplitOptions.TrimEntries);
        int hitPoints = int.Parse(line[splits[1]]);

        line = rawInput[1];
        line.Split(splits, ':', StringSplitOptions.TrimEntries);
        int damage = int.Parse(line[splits[1]]);

        line = rawInput[2];
        line.Split(splits, ':', StringSplitOptions.TrimEntries);
        int armour = int.Parse(line[splits[1]]);
        return new Stats(hitPoints, damage, armour);
    }
}
