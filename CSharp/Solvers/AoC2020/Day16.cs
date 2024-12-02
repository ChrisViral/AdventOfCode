using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 16
/// </summary>
public class Day16 : Solver<(HashSet<Day16.Field> fields, Day16.Ticket ticket, Day16.Ticket[] examples)>
{
    /// <summary>
    /// Ticket field
    /// </summary>
    public record Field(string Name, int FirstLower, int FirstUpper, int SecondLower, int SecondUpper)
    {
        #region Constants
        /// <summary>
        /// Regex pattern
        /// </summary>
        public const string PATTERN = @"^([a-z ]+): (\d+)-(\d+) or (\d+)-(\d+)$";
        #endregion

        #region Methods
        /// <summary>
        /// Checks if a given value is in the range of the field
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value is in the range, false otherwise</returns>
        public bool InRange(int value) => value >= this.FirstLower && value <= this.FirstUpper || value >= this.SecondLower && value <= this.SecondUpper;
        #endregion
    }

    /// <summary>
    /// Ticket
    /// </summary>
    public class Ticket
    {
        #region Fields
        public readonly int[] values;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ticket from a given input line
        /// </summary>
        /// <param name="line">Input to create the ticket values from</param>
        public Ticket(string line) => this.values = line.Split(',').ConvertAll(int.Parse);
        #endregion

        #region Methods
        /// <summary>
        /// Gets the error value for this ticket from a list of possible fields
        /// </summary>
        /// <param name="fields">Fields to check</param>
        /// <returns>The value of each invalid ticket field</returns>
        public (bool, int) GetError(IEnumerable<Field> fields)
        {
            bool valid = true;
            int error = 0;
            //Have to do this because the error might be zero ffs
            foreach (int value in this.values.Where(v => fields.All(f => !f.InRange(v))))
            {
                error += value;
                valid = false;
            }

            return (valid, error);
        }

        /// <summary>
        /// Checks if a value at a given position is valid for the given field
        /// </summary>
        /// <param name="field">Field to check</param>
        /// <param name="position">Position to check</param>
        /// <returns>True if the field is valid at the given position, false otherwise</returns>
        public bool IsValid(Field field, int position) => field.InRange(this.values[position]);
        #endregion
    }

    #region Constants
    private const string TARGET_START = "departure";
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1,T2,T3}"/> fails</exception>
    public Day16(string input) : base(input, options: StringSplitOptions.TrimEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        List<Ticket> valid = new(this.Data.examples.Length);
        int totalError = 0;
        foreach (Ticket ticket in this.Data.examples)
        {
            //Get all valid example tickets
            (bool correct, int error) = ticket.GetError(this.Data.fields);
            if (correct)
            {
                valid.Add(ticket);
            }
            else
            {
                totalError += error;
            }
        }
        AoCUtils.LogPart1(totalError);

        int length = this.Data.fields.Count;
        Field[] order = new Field[length];
        HashSet<int> toPlace = new(Enumerable.Range(0, length));
        //While there are fields to place
        while (this.Data.fields.Count > 0)
        {
            //Loop through positions to fill in
            foreach (int i in toPlace)
            {
                //Get fields valid in this position
                Field[] correct = this.Data.fields.Where(f => valid.All(t => t.IsValid(f, i))).Take(2).ToArray();
                //If there is only one, assign it
                if (correct.Length is 1)
                {
                    order[i] = correct[0];
                    this.Data.fields.Remove(correct[0]);
                    toPlace.Remove(i);
                    break;
                }
            }
        }

        long result = 1L;
        foreach (int i in ..length)
        {
            //If starts with the right word, multiply
            if (order[i].Name.StartsWith(TARGET_START))
            {
                result *= this.Data.ticket.values[i];
            }
        }

        AoCUtils.LogPart2(result);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (HashSet<Field>, Ticket, Ticket[]) Convert(string[] rawInput)
    {
        int i = 0;
        string line;
        HashSet<Field> fields = new(rawInput.Length / 2);
        RegexFactory<Field> fieldFactory = new(Field.PATTERN);
        for (line = rawInput[i++]; !string.IsNullOrEmpty(line); line = rawInput[i++])
        {
            fields.Add(fieldFactory.ConstructObject(line));
        }

        Ticket ticket = new(rawInput[++i]);
        List<Ticket> examples = new(rawInput.Length);
        for (i += 3; i < rawInput.Length; i++)
        {
            examples.Add(new Ticket(rawInput[i]));
        }

        return (fields, ticket, examples.ToArray());
    }
    #endregion
}