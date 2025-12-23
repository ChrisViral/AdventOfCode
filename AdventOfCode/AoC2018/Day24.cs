using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Ranges;
using CommunityToolkit.HighPerformance;
using SpanLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 24
/// </summary>
public sealed partial class Day24 : Solver<(Day24.Army immune, Day24.Army infection)>
{
    /// <summary>
    /// Type of damage modifier
    /// </summary>
    public enum ModifierType
    {
        WEAK,
        IMMUNE,
    }

    /// <summary>
    /// Group allegiance
    /// </summary>
    public enum Allegiance
    {
        IMMUNE_SYSTEM,
        INFECTION
    }

    /// <summary>
    /// Army group list
    /// </summary>
    /// <param name="capacity">Army capacity</param>
    public sealed class Army(int capacity) : List<Group>(capacity)
    {
        public Army() : this(10) { }
    }

    /// <summary>
    /// Army group
    /// </summary>
    /// <param name="units">Total units</param>
    /// <param name="hitPoints">Hit points per unit</param>
    /// <param name="damage">Damage dealt per unit</param>
    /// <param name="damageType">Damage type</param>
    /// <param name="initiative">Initiative value</param>
    [DebuggerDisplay("{Allegiance} {ID} Units: {Units}, Power: {EffectivePower}")]
    public sealed class Group(int units, int hitPoints, int damage, string damageType, int initiative)
    {
        /// <summary>
        /// Group ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Units total
        /// </summary>
        public int Units
        {
            get;
            private set
            {
                field = value;
                this.EffectivePower = value * this.Damage;
            }
        } = units;

        /// <summary>
        /// Hit points per unit
        /// </summary>
        public int HitPoints { get; } = hitPoints;

        /// <summary>
        /// Weak damage matchups
        /// </summary>
        public ImmutableArray<string> Weaknesses { get; } = [];

        /// <summary>
        /// Immune damage matchups
        /// </summary>
        public ImmutableArray<string> Immunities { get; } = [];

        /// <summary>
        /// Damage per unit
        /// </summary>
        public int Damage { get; } = damage;

        /// <summary>
        /// Damage type
        /// </summary>
        public string DamageType { get; } = damageType;

        /// <summary>
        /// Initiative value
        /// </summary>
        public int Initiative { get; } = initiative;

        /// <summary>
        /// Group allegiance
        /// </summary>
        public Allegiance Allegiance { get; set; }

        /// <summary>
        /// Current target
        /// </summary>
        public Group? Target { get; private set; }

        /// <summary>
        /// Group currently targetting
        /// </summary>
        public Group? TargetOf { get; private set; }

        /// <summary>
        /// Effective group power
        /// </summary>
        public int EffectivePower { get; private set; } = units * damage;

        /// <summary>
        /// If this group has at least one unit remaining
        /// </summary>
        public bool IsAlive => this.Units is not 0;

        /// <summary>
        /// If this group is currently being targetted
        /// </summary>
        public bool IsTargetted => this.TargetOf is not null;

        /// <summary>
        /// If this group has a set target
        /// </summary>
        public bool HasTarget => this.Target is not null;

        // ReSharper disable InvalidXmlDocComment
        /// <inheritdoc cref="AdventOfCode.AoC2018.Day24.Group(int, int, int, string, int)" />
        /// <param name="modifierType">Damage modifier type</param>
        /// <param name="modifierList">Damage modifiers</param>
        public Group(int units, int hitPoints,
                     ModifierType modifierType, string modifierList,
                     int damage, string damageType, int initiative)
            : this(units, hitPoints, damage, damageType, initiative)
        {
            switch (modifierType)
            {
                case ModifierType.WEAK:
                    this.Weaknesses = [..modifierList.Split(',', DEFAULT_OPTIONS)];
                    break;

                case ModifierType.IMMUNE:
                    this.Immunities = [..modifierList.Split(',', DEFAULT_OPTIONS)];
                    break;

                default:
                    modifierType.ThrowInvalid();
                    return;
            }
        }

        /// <inheritdoc cref="AdventOfCode.AoC2018.Day24.Group(int, int, int, string, int)" />
        /// <param name="firstModifierType">First damage modifier type</param>
        /// <param name="firstModifierList">First damage modifiers</param>
        /// <param name="secondModifierType">Second damage modifier type</param>
        /// <param name="secondModifierList">Second damage modifiers</param>
        public Group(int units, int hitPoints,
                     ModifierType firstModifierType, string firstModifierList,
                     ModifierType secondModifierType, string secondModifierList,
                     int damage, string damageType, int initiative)
            : this(units, hitPoints, firstModifierType, firstModifierList, damage, damageType, initiative)
        {
            switch (secondModifierType)
            {
                case ModifierType.WEAK:
                    this.Weaknesses = [..secondModifierList.Split(',', DEFAULT_OPTIONS)];
                    break;

                case ModifierType.IMMUNE:
                    this.Immunities = [..secondModifierList.Split(',', DEFAULT_OPTIONS)];
                    break;

                default:
                    secondModifierType.ThrowInvalid();
                    return;
            }
        }
        // ReSharper restore InvalidXmlDocComment

        /// <summary>
        /// Group copy constructor
        /// </summary>
        /// <param name="other">Other group to copy from</param>
        /// <param name="boost">Attack boost to apply</param>
        public Group(Group other, int boost = 0)
            : this(other.Units, other.HitPoints, other.Damage + boost, other.DamageType, other.Initiative)
        {
            this.ID = other.ID;
            this.Weaknesses = other.Weaknesses;
            this.Immunities = other.Immunities;
            this.Allegiance = other.Allegiance;
        }

        /// <summary>
        /// Resets this group's targets
        /// </summary>
        public void ResetTargets()
        {
            this.Target   = null;
            this.TargetOf = null;
        }

        /// <summary>
        /// Sets this group's target and correctly sets the target to be targetted by this group
        /// </summary>
        /// <param name="target">Target to set</param>
        public void SetTarget(Group target)
        {
            this.Target = target;
            target.TargetOf = this;
        }

        /// <summary>
        /// Calculates how much damage an attacking group would cause
        /// </summary>
        /// <param name="attacker">Attacking group</param>
        /// <returns>The damage caused by the attacker</returns>
        public int CalculateDamage(Group attacker)
        {
            if (this.Immunities.Contains(attacker.DamageType)) return 0;
            if (this.Weaknesses.Contains(attacker.DamageType)) return attacker.EffectivePower * 2;
            return attacker.EffectivePower;
        }

        /// <summary>
        /// Takes damage from an attacking group
        /// </summary>
        /// <param name="attacker">Attacking group</param>
        /// <returns><see langword="true"/> if the attack caused kills, otherwise <see langword="false"/></returns>
        public bool TakeDamage(Group attacker)
        {
            int damage = CalculateDamage(attacker);
            int kills = Math.Min(this.Units, damage / this.HitPoints);
            this.Units -= kills;
            return kills > 0;
        }
    }

    /// <summary>
    /// Group data matcher
    /// </summary>
    [GeneratedRegex(@"^(\d+) units each with (\d+) hit points(?: \((weak|immune) to ([\w, ]+)(?:; (weak|immune) to ([\w, ]+))?\))? with an attack that does (\d+) (\w+) damage at initiative (\d{1,2})$")]
    private static partial Regex GroupMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Copy both armies
        Army immune = new(this.Data.immune.Count);
        Army infection = new(this.Data.infection.Count);

        // Simulate the entire battle
        SimulateBattle(immune, infection);

        // Get the total unit count for the winning army
        Army winning = immune.IsEmpty ? infection : immune;
        int remainingUnits = winning.Sum(g => g.Units);
        AoCUtils.LogPart1(remainingUnits);

        int boost = 0;
        do
        {
            // Simulate battle until only the immune system is left
            SimulateBattle(immune, infection, boost++);
        }
        while (immune.IsEmpty || !infection.IsEmpty);

        // Print out immune system armies
        remainingUnits = immune.Sum(g => g.Units);
        AoCUtils.LogPart2(remainingUnits);
    }

    /// <summary>
    /// Simulates a battle until one army wins or a deadlock is reached
    /// </summary>
    /// <param name="immune">Immune army</param>
    /// <param name="infection">Infection army</param>
    /// <param name="boost">Immune attack boost</param>
    private void SimulateBattle(Army immune, Army infection, int boost = 0)
    {
        // Recreate immune army with the boost
        immune.Clear();
        immune.AddRange(this.Data.immune.AsEnumerable().Select(g => new Group(g, boost)));

        // Recreate infection army
        infection.Clear();
        infection.AddRange(this.Data.infection.AsEnumerable().Select(g => new Group(g)));

        // Simulate fights until one of the teams is empty
        do
        {
            if (!Fight(immune, infection))
            {
                // If the fight cause no kills, a deadlock is reached
                return;
            }
        }
        while (!immune.IsEmpty && !infection.IsEmpty);
    }

    /// <summary>
    /// Simulates a fight between both armies
    /// </summary>
    /// <param name="immune">Immune army</param>
    /// <param name="infection">Infection army</param>
    /// <returns><see langword="true"/> if the fight caused kills, otherwise <see langword="false"/></returns>
    private static bool Fight(Army immune, Army infection)
    {
        // Target selection phase
        foreach (Group group in immune.Concat(infection)
                                      .OrderByDescending(g => g.EffectivePower)
                                      .ThenByDescending(g => g.Initiative))
        {
            // Select best enemy
            Army enemies = GetEnemies(group, immune, infection);
            Group? target = enemies.AsSpan()
                                   .Where(e => e is { IsAlive: true, IsTargetted: false })
                                   .Select(e => (grp: e, dmg: e.CalculateDamage(group)))
                                   .Where(t => t.dmg > 0)
                                   .OrderByDescending(t => t.dmg)
                                   .ThenByDescending(t => t.grp.EffectivePower)
                                   .ThenByDescending(t => t.grp.Initiative)
                                   .FirstOrDefault().grp;

            if (target is not null)
            {
                group.SetTarget(target);
            }
        }

        // Attack phase
        bool hasKills = false;
        foreach (Group group in immune.Concat(infection)
                                      .Where(g => g.HasTarget)
                                      .OrderByDescending(g => g.Initiative))
        {
            // Ignore groups that have just been wiped out
            if (!group.IsAlive) continue;

            // Track kills
            hasKills |= group.Target!.TakeDamage(group);
        }

        // Clear out immune army
        immune.RemoveAll(g => !g.IsAlive);
        immune.ForEach(g => g.ResetTargets());

        // Clear out infection army
        infection.RemoveAll(g => !g.IsAlive);
        infection.ForEach(g => g.ResetTargets());

        // Return kill result
        return hasKills;
    }

    /// <summary>
    /// Gets the enemy army for a specific group
    /// </summary>
    /// <param name="group">Group to get the enemies for</param>
    /// <param name="immune">Immune army</param>
    /// <param name="infection">Infection army</param>
    /// <returns>The enemy army for the given group</returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">When the group's allegiance is invalid</exception>
    private static Army GetEnemies(Group group, Army immune, Army infection) => group.Allegiance switch
    {
        Allegiance.IMMUNE_SYSTEM => infection,
        Allegiance.INFECTION     => immune,
        _                        => throw group.Allegiance.Invalid()
    };

    /// <inheritdoc />
    protected override (Army, Army) Convert(string[] rawInput)
    {
        // Get separator index
        int infectionStart = rawInput.IndexOf("Infection:");

        // Parse lines with regex
        RegexFactory<Group> groupFactory = new(GroupMatcher);
        Army immune = [..groupFactory.ConstructObjects(rawInput[1..infectionStart])];
        Army infection = [..groupFactory.ConstructObjects(rawInput[(infectionStart + 1)..])];

        // Set IDs for immune army
        foreach (int i in ..immune.Count)
        {
            Group group = immune[i];
            group.ID = i;
            group.Allegiance = Allegiance.IMMUNE_SYSTEM;
        }

        // Set IDs for infection army
        foreach (int i in ..infection.Count)
        {
            Group group = infection[i];
            group.ID = i;
            group.Allegiance = Allegiance.INFECTION;
        }

        // Return armies
        return (immune, infection);
    }
}
