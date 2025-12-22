using System.Text;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 22
/// </summary>
public sealed class Day22 : Solver<(int[] p1, int[] p2)>
{
    /// <summary>
    /// Player enum
    /// </summary>
    public enum Player
    {
        P1,
        P2
    }

    /// <summary>
    /// State creation StringBuilder
    /// </summary>
    private static readonly StringBuilder StateBuilder = new();

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
        //Play a single game
        Queue<int> winner = PlayCombat(this.Data.p1, this.Data.p2);
        int count = winner.Count;
        AoCUtils.LogPart1(winner.Sum(c => c * count--));

        //Play a recursive game
        winner = PlayRecursiveCombat(this.Data.p1, this.Data.p2).deck;
        count = winner.Count;
        AoCUtils.LogPart2(winner.Sum(c => c * count--));
    }

    /// <summary>
    /// Plays a game of Combat and determines the winning deck
    /// </summary>
    /// <param name="p1Deck">Starting deck of the first player</param>
    /// <param name="p2Deck">Starting deck of the second player</param>
    /// <returns>The final deck of the winning player</returns>
    public static Queue<int> PlayCombat(IEnumerable<int> p1Deck, IEnumerable<int> p2Deck)
    {
        //Create decks
        Queue<int> p1 = new(p1Deck);
        Queue<int> p2 = new(p2Deck);
        //Keep running until a player has lost
        while (p1.Count is not 0 && p2.Count is not 0)
        {
            //Draw a card
            int c1 = p1.Dequeue();
            int c2 = p2.Dequeue();

            //Check round winner
            if (c1 > c2)
            {
                p1.Enqueue(c1);
                p1.Enqueue(c2);
            }
            else
            {
                p2.Enqueue(c2);
                p2.Enqueue(c1);
            }
        }
        //Return winning deck
        return p1.Count is not 0 ? p1 : p2;
    }

    /// <summary>
    /// Plays a game of Combat recursively and determines the winning deck
    /// </summary>
    /// <param name="p1Deck">Starting deck of the first player</param>
    /// <param name="p2Deck">Starting deck of the second player</param>
    /// <returns>A tuple containing the wining player and the final deck of the winning player</returns>
    /// ReSharper disable once CognitiveComplexity
    public static (Player winner, Queue<int> deck) PlayRecursiveCombat(IEnumerable<int> p1Deck, IEnumerable<int> p2Deck)
    {
        //Create decks
        Queue<int> p1 = new(p1Deck);
        Queue<int> p2 = new(p2Deck);
        //Create states memory and loop until an old state is repeated
        HashSet<string> states = [];
        while (states.Add(GetState(p1, p2)))
        {
            //Draw cards
            int c1 = p1.Dequeue();
            int c2 = p2.Dequeue();
            //Get round winner
            Player winner = p1.Count >= c1 && p2.Count >= c2 ? PlayRecursiveCombat(p1.Take(c1), p2.Take(c2)).winner : (c1 > c2 ? Player.P1 : Player.P2);
            //Adjust deck of winner
            switch (winner)
            {
                case Player.P1:
                    p1.Enqueue(c1);
                    p1.Enqueue(c2);
                    break;

                case Player.P2:
                    p2.Enqueue(c2);
                    p2.Enqueue(c1);
                    break;
            }

            //Check for a winner
            if (p1.Count is 0) return (Player.P2, p2);
            if (p2.Count is 0) return (Player.P1, p1);
        }

        //If an old state was reached, player 1 wins
        return (Player.P1, p1);
    }

    /// <summary>
    /// Gets a string state for the current decks
    /// </summary>
    /// <param name="p1">Deck of the first player</param>
    /// <param name="p2">Deck of the second player</param>
    /// <returns>A string representation of the game state</returns>
    private static string GetState(IEnumerable<int> p1, IEnumerable<int> p2) => StateBuilder.Clear().AppendJoin(',', p1).Append('-').AppendJoin(',', p2).ToString();

    /// <inheritdoc />
    protected override (int[], int[]) Convert(string[] rawInput)
    {
        int end = Enumerable.Range(1, rawInput.Length).First(i => rawInput[i][0] is 'P');
        return (rawInput[1..end++].ConvertAll(int.Parse), rawInput[end..].ConvertAll(int.Parse));
    }
}
