using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Challenges
{
    public class Day2 : Challenge
    {
        #region Properties
        /// <summary>
        /// Day ID
        /// </summary>
        public override int ID { get; } = 2;
        #endregion

        #region Methods
        /// <summary>
        /// Challenge solver
        /// </summary>
        public override void Solve()
        {
            int twos = 0, threes = 0;
            Stack<string> ids = new Stack<string>();
            foreach (string line in GetLines())
            {
                ids.Push(line);
                HashSet<int> counts = new HashSet<int>(line.GroupBy(c => c).Select(g => g.Count()));
                if (counts.Contains(2)) { twos++; }
                if (counts.Contains(3)) { threes++; }
            }

            Print("Part one checksum: " + (twos * threes));

            while (ids.Count > 0)
            {
                string first = ids.Pop();
                foreach (string second in ids)
                {
                    bool mismatch = false;
                    StringBuilder sb = new StringBuilder(first.Length);
                    for (int i = 0; i < first.Length; i++)
                    {
                        if (first[i] != second[i])
                        {
                            if (mismatch)
                            {
                                mismatch = false;
                                break;
                            }
                            mismatch = true;
                        }
                        else
                        {
                            sb.Append(first[i]);
                        }
                    }

                    if (mismatch)
                    {
                        Print("Part two diff: " + sb);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
