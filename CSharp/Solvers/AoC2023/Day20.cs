﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 20
/// </summary>
public class Day20 : Solver<Dictionary<string, Day20.Module>>
{
    public enum Pulse
    {
        LOW,
        HIGH
    }

    public record struct Transmission(Module Sender, Pulse Pulse, string Label);

    public abstract class Module(string label, string listeners) : IEquatable<Module>
    {
        public string Label { get; } = label;

        public virtual string DisplayName => this.Label;

        public ReadOnlyCollection<string> Listeners { get; } = new(listeners.Split(',', DEFAULT_OPTIONS));

        public abstract Pulse? HandlePulse(Module sender, Pulse pulse);

        public virtual void Reset() { }

        /// <inheritdoc />
        public bool Equals(Module? other) => !ReferenceEquals(null, other)
                                              && (ReferenceEquals(this, other)
                                               || this.Label == other.Label);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Module module && Equals(module);

        /// <inheritdoc />
        public override int GetHashCode() => this.Label.GetHashCode();

        public override string ToString() => $"{this.DisplayName} → {string.Join(", ", this.Listeners)}";

        public static Module ParseModule(string data)
        {
            string[] splits = data.Split("->", DEFAULT_OPTIONS);
            if (splits is not [{ } label, { } listeners]) throw new UnreachableException("Invalid module data");

            return label[0] switch
            {
                BroadcastModule.BROADCAST     => new BroadcastModule(label, listeners),
                FlipFlopModule.FLIPFLOP       => new FlipFlopModule(label[1..], listeners),
                ConjunctionModule.CONJUNCTION => new ConjunctionModule(label[1..], listeners),
                _                             => throw new UnreachableException("Unknown module detected")
            };
        }
    }

    public class FlipFlopModule(string label, string listeners) : Module(label, listeners)
    {
        public const char FLIPFLOP = '%';

        public override string DisplayName => $"{FLIPFLOP}{this.Label}";

        public bool State { get; private set; }

        public override Pulse? HandlePulse(Module _, Pulse pulse)
        {
            if (pulse is Pulse.HIGH) return null;

            this.State = !this.State;
            return this.State ? Pulse.HIGH : Pulse.LOW;
        }

        public override void Reset() => this.State = false;
    }

    public class ConjunctionModule(string label, string listeners) : Module(label, listeners)
    {
        public const char CONJUNCTION = '&';

        public override string DisplayName => $"{CONJUNCTION}{this.Label}";

        private readonly Dictionary<Module, Pulse> memory = new();

        public override Pulse? HandlePulse(Module sender, Pulse pulse)
        {
            this.memory[sender] = pulse;
            return this.memory.Values.All(m => m == Pulse.HIGH) ? Pulse.LOW : Pulse.HIGH;
        }

        public override void Reset() => this.memory.Keys.ForEach(m => this.memory[m] = Pulse.LOW);

        public void FetchInputs(ICollection<Module> modules) => modules.Where(m => m.Listeners.Contains(this.Label))
                                                                           .ForEach(m => this.memory.Add(m, Pulse.LOW));
    }

    public class BroadcastModule(string label, string listeners) : Module(label, listeners)
    {
        public const string BROADCASTER = "broadcaster";
        public const char   BROADCAST   = 'b';

        public override Pulse? HandlePulse(Module sender, Pulse pulse) => pulse;
    }

    public class ButtonModule(string label, string listeners) : Module(label, listeners)
    {
        public const string BUTTON = "button";

        public override Pulse? HandlePulse(Module sender, Pulse pulse) => Pulse.LOW;

        public static ButtonModule CreateButton() => new(BUTTON, BroadcastModule.BROADCASTER);
    }

    private const int CYCLES    = 1000;
    private const string TARGET = "rx";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day20(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int lowPulses = 0, highPulses = 0;
        Queue<Transmission> transmissions = new();
        foreach (int _ in ..CYCLES)
        {
            lowPulses--;
            transmissions.Enqueue(new(null!, Pulse.LOW, ButtonModule.BUTTON));
            while (transmissions.TryDequeue(out Transmission transmission))
            {
                if (transmission.Pulse is Pulse.LOW)
                {
                    lowPulses++;
                }
                else
                {
                    highPulses++;
                }

                if (!this.Data.TryGetValue(transmission.Label, out Module? current)) continue;

                Pulse? sent = current.HandlePulse(transmission.Sender, transmission.Pulse);
                if (!sent.HasValue) continue;

                current.Listeners.ForEach(l => transmissions.Enqueue(new(current, sent.Value, l)));
            }
        }

        AoCUtils.LogPart1((long)lowPulses * highPulses);

        this.Data.Values.ForEach(m => m.Reset());
        Module final = this.Data.Values.First(m => m.Listeners.Contains(TARGET));
        HashSet<Module> triggers = [..this.Data.Values.Where(m => m.Listeners.Contains(final.Label))];
        Dictionary<Module, int> firstTriggerHit = new(triggers.Count);

        int buttonPresses = 0;
        while (firstTriggerHit.Count != triggers.Count)
        {
            buttonPresses++;
            transmissions.Enqueue(new(null!, Pulse.LOW, ButtonModule.BUTTON));
            while (transmissions.TryDequeue(out Transmission transmission))
            {
                if (!this.Data.TryGetValue(transmission.Label, out Module? current)) continue;

                Pulse? sent = current.HandlePulse(transmission.Sender, transmission.Pulse);
                if (!sent.HasValue) continue;

                if (triggers.Contains(current) && sent.Value is Pulse.HIGH)
                {
                    firstTriggerHit.TryAdd(current, buttonPresses);
                }

                current.Listeners.ForEach(l => transmissions.Enqueue(new(current, sent.Value, l)));
            }
        }

        long total = MathUtils.LCM(firstTriggerHit.Values.Select(h => (long)h).ToArray());
        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Module> Convert(string[] rawInput)
    {
        Dictionary<string, Module> modules = new(rawInput.Length + 1)
        {
            [ButtonModule.BUTTON] = ButtonModule.CreateButton()
        };

        foreach (string line in rawInput)
        {
            Module module = Module.ParseModule(line);
            modules.Add(module.Label, module);
        }

        modules.Values.Where(m => m is ConjunctionModule)
               .Cast<ConjunctionModule>()
               .ForEach(m => m.FetchInputs(modules.Values));

        return modules;
    }
    #endregion
}
