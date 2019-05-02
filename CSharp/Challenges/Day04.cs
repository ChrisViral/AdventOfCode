using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Tools;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Day 4 challenge
    /// </summary>
    public class Day04 : Challenge
    {
        #region Properties
        /// <summary>
        /// Day ID
        /// </summary>
        public override int ID { get; } = 4;
        #endregion

        #region Methods
        /// <summary>
        /// Challenge solver
        /// </summary>
        public override void Solve()
        {
            Regex pattern = new Regex(@".+(\d{2}-\d{2} \d{2}:\d{2})[^#]+#?(wakes|falls|\d+).+", RegexOptions.Compiled);
            SortedDictionary<DateTime, string> timestamps = new SortedDictionary<DateTime, string>();
            
            foreach (string line in GetLines())
            {
                string[] data = pattern.ParseData(line);
                timestamps.Add(DateTime.ParseExact(data[0], "MM-dd HH:mm", CultureInfo.InvariantCulture), data[1]);
            }
            
            int start = 0;
            int[] timesheet = null;
            Dictionary<int, int[]> schedules = new Dictionary<int, int[]>();
            foreach (KeyValuePair<DateTime, string> info in timestamps)
            {
                switch (info.Value)
                {
                    case "falls":
                        start = info.Key.Minute;
                        break;

                    case "wakes":
                        for (int i = start; i < info.Key.Minute; i++)
                        {   
                            //This won't happen by puzzle design
                            //ReSharper disable once PossibleNullReferenceException
                            timesheet[i]++;
                        }
                        break;

                    default:
                        int guard = int.Parse(info.Value);
                        if (!schedules.TryGetValue(guard, out timesheet))
                        {
                            timesheet = new int[60];
                            schedules.Add(guard, timesheet);
                        }
                        break;
                }
            }

            KeyValuePair<int, int[]> candidate = schedules.MaxValue(p => p.Value.Sum());
            Print("Part one hash: " + (candidate.Key * candidate.Value.MaxIndex()));

            candidate = schedules.MaxValue(p => p.Value.Max());
            Print("Part two hash: " + (candidate.Key * candidate.Value.MaxIndex()));
        }
        #endregion
    }
}
