using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Extensions;

/// <summary>
/// Regex extension methods
/// </summary>
public static class RegexExtensions
{
    #region Extension methods
    /// <summary>
    /// Gets all the captured groups of the match
    /// </summary>
    /// <param name="match">Match to get the groups from</param>
    /// <returns>Enumerable of the captured groups</returns>
    public static IEnumerable<Group> GetCapturedGroups(this Match match) => match.Groups
                                                                                 .Cast<Group>()
                                                                                 .Skip(1)
                                                                                 .Where(g => !string.IsNullOrEmpty(g.Value));
    #endregion
}
