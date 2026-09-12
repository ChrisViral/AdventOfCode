using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 22
/// </summary>
public sealed class Day22 : Solver<Day22.Stats>
{
    /// <summary>
    /// Player stats
    /// </summary>
    /// <param name="HitPoints">Remaining hitpoints</param>
    /// <param name="Damage">Damage value</param>
    /// <param name="Armour">Armour value</param>
    /// <param name="Mana">Remaining mana</param>
    public record struct Stats(int HitPoints, int Damage, int Armour, int Mana);

    /// <summary>
    /// Turns left per effect array
    /// </summary>
    [InlineArray(4)]
    private struct EffectTurns
    {
        private int element;
    }

    /// <summary>
    /// Global combat state
    /// </summary>
    /// <param name="player">Player stats</param>
    /// <param name="boss">Boss stats</param>
    /// <param name="effects">Turns left per effect</param>
    private struct CombatState(Stats player, Stats boss, EffectTurns effects)
    {
        /// <summary> Player stats </summary>
        public Stats player = player;
        /// <summary> Boss stats </summary>
        public Stats boss   = boss;
        /// <summary> Turns left per effect </summary>
        public EffectTurns effects = effects;

        /// <summary>
        /// If the player is dead
        /// </summary>
        public bool IsPlayerDead => this.player.HitPoints <= 0;

        /// <summary>
        /// If the boss is dead
        /// </summary>
        public bool IsBossDead => this.boss.HitPoints <= 0;

        /// <summary>
        /// Apply and resolve effects
        /// </summary>
        public void ApplyEffects()
        {
            foreach (Effect effect in Effects)
            {
                if (!IsEffectActive(effect.Id)) continue;

                effect.ApplyEffect(ref this);

                if (!IsEffectActive(effect.Id))
                {
                    effect.EndEffect(ref this);
                }
            }
        }

        /// <summary>
        /// If the given effect ID is currently active
        /// </summary>
        /// <param name="effectID">Effect ID to check</param>
        /// <returns><see langword="true"/> when the effect is active, otherwise <see langword="false"/></returns>
        public bool IsEffectActive(int effectID) => this.effects[effectID] > 0;
    }

    /// <summary>
    /// Spell data
    /// </summary>
    /// <param name="Name">Spell name</param>
    /// <param name="Cost">Spell mana cost</param>
    /// <param name="Damage">Spell damage</param>
    /// <param name="Heal">Spell self-heal</param>
    /// <param name="EffectID">Spell effect ID</param>
    private readonly record struct Spell([UsedImplicitly] string Name, int Cost, int Damage, int Heal, int EffectID);

    /// <summary>
    /// Effect base class
    /// </summary>
    /// <param name="Turns">Turns the effect is active for</param>
    private abstract record Effect(int Turns)
    {
        /// <summary>
        /// Effect unique ID
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// Initialises the effect
        /// </summary>
        /// <param name="state">Current combat state reference</param>
        public virtual void StartEffect(ref CombatState state) => state.effects[this.Id] = this.Turns;

        /// <summary>
        /// Applies the effect for the current turn
        /// </summary>
        /// <param name="state">Current combat state reference</param>
        public virtual void ApplyEffect(ref CombatState state) => state.effects[this.Id]--;

        /// <summary>
        /// Removes the effect
        /// </summary>
        /// <param name="state">Current combat state reference</param>
        public virtual void EndEffect(ref CombatState state) { }
    }

    /// <summary>
    /// Sheild effect
    /// </summary>
    /// <param name="Turns">Turns the effect is active for</param>
    /// <param name="Armour">Armour value granted by the effect</param>
    private sealed record ShieldEffect(int Turns, int Armour) : Effect(Turns)
    {
        /// <summary>
        /// Shield effect ID
        /// </summary>
        public const int ID = 0;

        /// <inheritdoc />
        public override int Id => ID;

        /// <inheritdoc />
        public override void StartEffect(ref CombatState state)
        {
            base.StartEffect(ref state);
            state.player.Armour += this.Armour;
        }

        /// <inheritdoc />
        public override void EndEffect(ref CombatState state)
        {
            state.player.Armour -= this.Armour;
            base.EndEffect(ref state);
        }
    }

    /// <summary>
    /// Poison effect
    /// </summary>
    /// <param name="Turns">Turns the effect is active for</param>
    /// <param name="Damage">Damage applied per turn</param>
    private sealed record PoisonEffect(int Turns, int Damage) : Effect(Turns)
    {
        /// <summary>
        /// Poison effect ID
        /// </summary>
        public const int ID = 1;

        /// <inheritdoc />
        public override int Id => ID;

        /// <inheritdoc />
        public override void ApplyEffect(ref CombatState state)
        {
            base.ApplyEffect(ref state);
            state.boss.HitPoints -= this.Damage;
        }
    }

    /// <summary>
    /// Recharge effect
    /// </summary>
    /// <param name="Turns">Turns the effect is active for</param>
    /// <param name="Recharge">Mana recharged per turn</param>
    private sealed record RechargeEffect(int Turns, int Recharge) : Effect(Turns)
    {
        /// <summary>
        /// Recharge effect ID
        /// </summary>
        public const int ID = 2;

        /// <inheritdoc />
        public override int Id => ID;

        /// <inheritdoc />
        public override void ApplyEffect(ref CombatState state)
        {
            base.ApplyEffect(ref state);
            state.player.Mana += this.Recharge;
        }
    }

    /// <summary>
    /// No effect
    /// </summary>
    private sealed record NoEffect() : Effect(0)
    {
        /// <summary>
        /// No effect ID
        /// </summary>
        public const int ID = 3;

        /// <inheritdoc />
        public override int Id => ID;

        /// <inheritdoc />
        public override void StartEffect(ref CombatState state) { }

        /// <inheritdoc />
        public override void ApplyEffect(ref CombatState state) { }
    }

    /// <summary>
    /// Player start hit points
    /// </summary>
    private const int HIT_POINTS = 50;
    /// <summary>
    /// Player start mana
    /// </summary>
    private const int MANA  = 500;
    /// <summary>
    /// Part 2 bleed value
    /// </summary>
    private const int BLEED = 1;

    /// <summary>
    /// Spells list
    /// </summary>
    private static readonly ImmutableArray<Spell> Spells =
    [
        new("Magic Missile", Cost: 53,  Damage: 4, Heal: 0, EffectID: NoEffect.ID),
        new("Drain",         Cost: 73,  Damage: 2, Heal: 2, EffectID: NoEffect.ID),
        new("Shield",        Cost: 113, Damage: 0, Heal: 0, EffectID: ShieldEffect.ID),
        new("Poison",        Cost: 173, Damage: 0, Heal: 0, EffectID: PoisonEffect.ID),
        new("Recharge",      Cost: 229, Damage: 0, Heal: 0, EffectID: RechargeEffect.ID),
    ];

    /// <summary>
    /// Effects list
    /// </summary>
    private static readonly ImmutableArray<Effect> Effects =
    [
        new ShieldEffect(Turns: 6,   Armour: 7),
        new PoisonEffect(Turns: 6,   Damage: 3),
        new RechargeEffect(Turns: 5, Recharge: 101),
        new NoEffect()
    ];


    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup initial state
        CombatState state = new()
        {
            player = new Stats(HIT_POINTS, 0, 0, MANA),
            boss = this.Data
        };

        // Part 1: no bleed
        int manaCost = ExecutePlayerTurn(state, 0);
        AoCUtils.LogPart1(manaCost);

        // Part 2: with bleed
        manaCost = ExecutePlayerTurn(state, 0, BLEED);
        AoCUtils.LogPart2(manaCost);
    }

    /// <summary>
    /// Executes the player's turn and tests every spell to find the lowest mana cost
    /// </summary>
    /// <param name="state">Current combat state</param>
    /// <param name="manaCostSoFar">Mana cost so far in the combat</param>
    /// <param name="bleed">Player turn bleed value</param>
    /// <returns>The minimal mana cost from this combat state onwards</returns>
    private int ExecutePlayerTurn(CombatState state, int manaCostSoFar, int bleed = 0)
    {
        // Apply bleed, if player dies, we lose
        state.player.HitPoints -= bleed;
        if (state.IsPlayerDead) return int.MaxValue;

        // Apply effects, if boss dies, we win
        state.ApplyEffects();
        if (state.IsBossDead) return manaCostSoFar;

        // Initialize to max value, if no spell executed it will be returned
        int minManaCost = int.MaxValue;
        foreach (Spell spell in Spells)
        {
            // Ignore spells we can't cast
            if (state.player.Mana < spell.Cost) continue;
            // Ignore spells that would activate an effect that's already active
            if (state.IsEffectActive(spell.EffectID)) continue;

            // Apply the spell and it's effect if any
            CombatState newState = state;
            newState.player.Mana -= spell.Cost;
            newState.boss.HitPoints -= spell.Damage;
            newState.player.HitPoints += spell.Heal;
            Effects[spell.EffectID].StartEffect(ref newState);

            if (newState.IsBossDead)
            {
                // If the boss dies, keep track of that mana cost
                minManaCost = Math.Min(minManaCost, manaCostSoFar + spell.Cost);
            }
            else
            {
                // If not, execute the boss' turn next and keep track of that mana cost
                int manaCost = ExecuteBossTurn(newState, manaCostSoFar + spell.Cost, bleed);
                minManaCost = Math.Min(minManaCost, manaCost);
            }
        }

        // Return minimal mana cost
        return minManaCost;
    }

    /// <summary>
    /// Executes the boss's turn
    /// </summary>
    /// <param name="state">Current combat state</param>
    /// <param name="manaCostSoFar">Mana cost so far in the combat</param>
    /// <param name="bleed">Player turn bleed value</param>
    /// <returns>The minimal mana cost from this combat state onwards</returns>
    private int ExecuteBossTurn(CombatState state, int manaCostSoFar, int bleed)
    {
        // Apply effects, if boss dies, we win
        state.ApplyEffects();
        if (state.IsBossDead) return manaCostSoFar;

        // Execute boss attack, if player dies, we lose
        state.player.HitPoints -= Math.Max(state.boss.Damage - state.player.Armour, 1);
        if (state.IsPlayerDead) return int.MaxValue;

        // Execute player turn next
        return ExecutePlayerTurn(state, manaCostSoFar, bleed);
    }

    /// <inheritdoc />
    protected override Stats Convert(string[] rawInput)
    {
        // Parse hit points
        Span<Range> splits = stackalloc Range[2];
        ReadOnlySpan<char> line = rawInput[0];
        line.Split(splits, ':', StringSplitOptions.TrimEntries);
        int hitPoints = int.Parse(line[splits[1]]);

        // Parse damage
        line = rawInput[1];
        line.Split(splits, ':', StringSplitOptions.TrimEntries);
        int damage = int.Parse(line[splits[1]]);
        return new Stats(hitPoints, damage, 0, 0);
    }
}
