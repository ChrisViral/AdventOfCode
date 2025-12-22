using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Collections.Search;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 15
/// </summary>
public sealed class Day15 : Solver<Grid<Day15.Entity>>
{
    /// <summary>
    /// Unit alliance
    /// </summary>
    public enum Alliance
    {
        ELF,
        GOBLIN
    }

    /// <summary>
    /// Entity base class
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// If this is a traversable entity
        /// </summary>
        public bool IsPassable => this is Passive { IsEmpty: true };

        /// <summary>
        /// Creaters a new entity from the specified character and position
        /// </summary>
        /// <param name="value">Entity value</param>
        /// <param name="position">Entity position</param>
        /// <returns>The created entity</returns>
        /// <exception cref="InvalidOperationException">When an invalid entity type is detected</exception>
        public static Entity Parse(char value, Vector2<int> position) => value switch
        {
            '.' => Passive.Empty,
            '#' => Passive.Wall,
            'E' => new Unit(position, Alliance.ELF),
            'G' => new Unit(position, Alliance.GOBLIN),
            _   => throw new InvalidOperationException("Unknown entity type")
        };
    }

    /// <summary>
    /// Passive entity
    /// </summary>
    public sealed class Passive : Entity
    {
        /// <summary>
        /// Empty entity instance
        /// </summary>
        public static Passive Empty { get; } = new(true);

        /// <summary>
        /// Wall entity instance
        /// </summary>
        public static Passive Wall { get; } = new(false);

        /// <summary>
        /// Creates an entity, prevents extensnal instantiation
        /// </summary>
        /// <param name="isEmpty">If the passive entity is empty or not</param>
        private Passive(bool isEmpty) => this.IsEmpty = isEmpty;

        /// <summary>
        /// If this entity is empty or not
        /// </summary>
        public bool IsEmpty { get; }

        /// <inheritdoc />
        public override string ToString() => this.IsEmpty ? "." : "#";
    }

    /// <summary>
    /// Unit entity
    /// </summary>
    /// <param name="position">Unit's position</param>
    /// <param name="alliance">Unit's alliance</param>
    public sealed class Unit(Vector2<int> position, Alliance alliance) : Entity, IComparable<Unit>
    {
        /// <summary>
        /// Simulation check directions, in proper order
        /// </summary>
        private static readonly ImmutableArray<Vector2<int>> CheckDirections = [Vector2<int>.Up, Vector2<int>.Left, Vector2<int>.Right, Vector2<int>.Down];

        /// <summary>
        /// Unit's position
        /// </summary>
        private Vector2<int> position = position;
        /// <summary>
        /// Unit's alliance
        /// </summary>
        private readonly Alliance alliance = alliance;

        /// <summary>
        /// Current hitpoints for this unit
        /// </summary>
        public int HitPoints { get; private set; } = 200;

        /// <summary>
        /// Current attack power of this unit
        /// </summary>
        public int AttackPower { get; set; } = 3;

        /// <summary>
        /// If this unit is dead
        /// </summary>
        public bool IsDead => this.HitPoints <= 0;

        /// <summary>
        /// If this unit is an elf
        /// </summary>
        public bool IsElf => this.alliance == Alliance.ELF;

        /// <summary>
        /// Unit clonining constructor
        /// </summary>
        /// <param name="other">Other Unit to clone</param>
        public Unit(Unit other) : this(other.position, other.alliance) { }

        /// <summary>
        /// Checks if this is an enemy unit
        /// </summary>
        /// <param name="unit">Other unit to check</param>
        /// <returns><see langword="true"/> when <paramref name="unit"/> is an enemy, otherwise <see langword="false"/></returns>
        private bool IsEnemy(Unit unit) => this.alliance != unit.alliance;

        /// <summary>
        /// Updates this unit
        /// </summary>
        /// <param name="map">Map to update within</param>
        /// <param name="units">List of other units</param>
        /// <param name="causedElfDeath">Output parameter indicating if an elf death was caused by this update</param>
        /// <returns><see langword="true"/> if the update succeeded and targets could be found, otherwise <see langword="false"/></returns>
        public bool Update(Grid<Entity> map, IReadOnlyList<Unit> units, ref bool causedElfDeath)
        {
            // Get all targets
            using Pooled<List<Unit>> targets = GetTargets(units);
            if (targets.Ref.IsEmpty) return false;

            // Get positions in range of a target
            using Pooled<HashSet<Vector2<int>>> inRange = GetInRange(map, targets.Ref);

            // Check if we're already in range of a target
            if (!inRange.Ref.Contains(this.position))
            {
                // Choose best move towards targets
                if (MoveTowardsTarget(map, inRange.Ref, out Vector2<int>? move))
                {
                    // Update map data
                    map[move.Value] = this;
                    map[this.position] = Passive.Empty;
                    this.position = move.Value;
                }
            }

            // If we're still not in range, abort
            if (!inRange.Ref.Contains(this.position)) return true;

            // Select target to attack
            Unit? target = SelectTarget(map);
            if (target is null) return true;

            // Attack target
            target.HitPoints -= this.AttackPower;
            if (target.IsDead)
            {
                map[target.position] = Passive.Empty;
                causedElfDeath = target.IsElf;
            }

            return true;
        }

        /// <summary>
        /// Obtains the enemy targets from the specified units
        /// </summary>
        /// <param name="units">Current units on the map</param>
        /// <returns>A list of units that can be targetted</returns>
        private Pooled<List<Unit>> GetTargets(IReadOnlyList<Unit> units)
        {
            Pooled<List<Unit>> targets = ListObjectPool<Unit>.Shared.Get();
            targets.Ref.AddRange(units.Where(u => !u.IsDead && IsEnemy(u)));
            return targets;
        }

        /// <summary>
        /// Gets the set of positions which are within range of targets
        /// </summary>
        /// <param name="map">Map to check within</param>
        /// <param name="targets">Target units list</param>
        /// <returns>A set of positions adjacent to the selected targets</returns>
        private Pooled<HashSet<Vector2<int>>> GetInRange(Grid<Entity> map, IReadOnlyList<Unit> targets)
        {
            Pooled<HashSet<Vector2<int>>> inRange = HashSetObjectPool<Vector2<int>>.Shared.Get();
            foreach (Unit target in targets)
            {
                // Get adjacent and reachable
                foreach (Vector2<int> adjacent in target.position.Adjacent())
                {
                    if (map[adjacent].IsPassable || adjacent == this.position)
                    {
                        inRange.Ref.Add(adjacent);
                    }
                }
            }
            return inRange;
        }

        /// <summary>
        /// Tries to get the best move towards one of the selected targets
        /// </summary>
        /// <param name="map">Map to move within</param>
        /// <param name="inRange">Positions which are in range of enemy targets</param>
        /// <param name="move">Selected move output parameter</param>
        /// <returns><see langword="true"/> if a move was found, othewise <see langword="false"/></returns>
        private bool MoveTowardsTarget(Grid<Entity> map, IReadOnlySet<Vector2<int>> inRange,  [NotNullWhen(true)] out Vector2<int>? move)
        {
            int bestPathLength = int.MaxValue;
            move = null;
            // Select reachable moves
            foreach (Vector2<int> target in inRange.OrderBy(p => p.Y).ThenBy(p => p.X))
            {
                // Check if target can be reached
                if (CanReachTarget(map, target, out int distance, out Vector2<int>? firstMove)
                    && bestPathLength > distance)
                {
                    bestPathLength = distance;
                    move = firstMove.Value;
                }
            }

            return move is not null;
        }

        /// <summary>
        /// Checks if a selected target can currently be reached
        /// </summary>
        /// <param name="map">Map to check within</param>
        /// <param name="target">Target to try to reach</param>
        /// <param name="distance">Distance to the target output parameter</param>
        /// <param name="move">Move towards the target output parameter</param>
        /// <returns><see langword="true"/> when the target can be reached, otherwise <see langword="false"/></returns>
        private bool CanReachTarget(Grid<Entity> map, Vector2<int> target, out int distance, [NotNullWhen(true)] out Vector2<int>? move)
        {
            distance = int.MaxValue;
            move = null;
            foreach (Vector2<int> direction in CheckDirections)
            {
                // Check we can start from there
                Vector2<int> start = this.position + direction;
                if (!map[start].IsPassable) continue;

                // Get the path length
                int? pathLength = SearchUtils.GetPathLengthBFS(start, target,
                                                               p => p.AsAdjacentEnumerable()
                                                                     .Where(a => map[a].IsPassable));

                // Only keep if better
                if (distance > pathLength)
                {
                    distance = pathLength.Value;
                    move = start;
                }
            }

            return move is not null;
        }

        /// <summary>
        /// Selects a given target to attack within the map
        /// </summary>
        /// <param name="map">Map to select within</param>
        /// <returns>The selected target to attack if one was found, otherwise <see langword="null"/></returns>
        private Unit? SelectTarget(Grid<Entity> map)
        {
            Unit? selectedTarget = null;
            foreach (Vector2<int> direction in CheckDirections)
            {
                // Check if the target is an enemy with the least hit points
                Vector2<int> targetPosition = this.position + direction;
                if (map[targetPosition] is Unit currentTarget
                 && IsEnemy(currentTarget)
                 && (selectedTarget is null || selectedTarget.HitPoints > currentTarget.HitPoints))
                {
                    selectedTarget = currentTarget;
                }
            }
            return selectedTarget;
        }

        /// <inheritdoc />
        public override string ToString() => this.alliance switch
        {
            Alliance.ELF    => "E",
            Alliance.GOBLIN => "G",
            _               => throw new InvalidEnumArgumentException(nameof(this.alliance), (int)this.alliance, typeof(Alliance))
        };

        /// <inheritdoc />
        public int CompareTo(Unit? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return ComponentComparer<int>.Instance.Compare(this.position, other.position);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Simulates one round of combat
        int rounds = SimulateCombat(out List<Unit> units);
        int outcome = units.Sum(u => u.HitPoints) * rounds;
        AoCUtils.LogPart1(outcome);

        // Count the elves on the map
        int elfCount = this.Data.Count(e => e is Unit { IsElf: true });
        int power = 4;
        do
        {
            // Simulate combat with increased elf power
            units.Clear();
            rounds = SimulateCombat(out units, power++, true);
        }
        while (units.Count(u => u.IsElf) != elfCount);

        // Print final outcome
        outcome = units.Sum(u => u.HitPoints) * rounds;
        AoCUtils.LogPart2(outcome);
    }

    /// <summary>
    /// Creates a deep copy of the map
    /// </summary>
    /// <returns>A deep copy of the map</returns>
    private Grid<Entity> CloneData()
    {
        Grid<Entity> map = new(this.Data.Width, this.Data.Height);
        foreach (Vector2<int> position in map.Dimensions.Enumerate())
        {
            Entity entity = this.Data[position];
            map[position] = entity is Unit unit ? new Unit(unit) : entity;
        }
        return map;
    }

    // ReSharper disable once CognitiveComplexity
    private int SimulateCombat(out List<Unit> units, int attackPower = 3, bool returnOnElfDeath = false)
    {
        int rounds = -1;
        bool hasTargets = true;
        bool causedElfDeath = false;
        Grid<Entity> map = CloneData();
        units = map.Where(e => e is Unit)
                   .Cast<Unit>()
                   .ToList();
        units.Where(u => u.IsElf).ForEach(u => u.AttackPower = attackPower);
        do
        {
            rounds++;
            units.Sort();
            foreach (Unit unit in units)
            {
                if (unit.IsDead) continue;

                if (!unit.Update(map, units, ref causedElfDeath))
                {
                    hasTargets = false;
                    break;
                }

                if (causedElfDeath && returnOnElfDeath)
                {
                    units.RemoveAll(u => u.IsDead);
                    return rounds;
                }
            }
            units.RemoveAll(u => u.IsDead);
        }
        while (hasTargets);
        return rounds;
    }

    /// <inheritdoc />
    protected override Grid<Entity> Convert(string[] rawInput)
    {
        Grid<Entity> map = new(rawInput[0].Length, rawInput.Length);
        foreach (Vector2<int> position in map.Dimensions.Enumerate())
        {
            map[position] = Entity.Parse(rawInput[position.Y][position.X], position);
        }
        return map;
    }
}
