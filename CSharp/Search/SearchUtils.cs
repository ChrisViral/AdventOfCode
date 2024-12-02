using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Extensions.Enumerables;
using JetBrains.Annotations;

namespace AdventOfCode.Search;

/// <summary>
/// Movement data container struct
/// </summary>
/// <param name="Value">Movement value</param>
/// <param name="Cost">Movement cost</param>
/// <typeparam name="TValue">Value type</typeparam>
/// <typeparam name="TCost">Cost numerical type</typeparam>
[PublicAPI]
public record struct MoveData<TValue, TCost>(TValue Value, TCost Cost) where TCost : INumber<TCost>;

/// <summary>
/// Searching utility methods
/// </summary>
[PublicAPI]
public static class SearchUtils
{
    /// <summary>
    /// Neighbours exploration function
    /// </summary>
    /// <typeparam name="TValue">Type of object to explore</typeparam>
    /// <typeparam name="TCost">Cost type</typeparam>
    /// <param name="node">Node to explore</param>
    /// <returns>An enumerable of all neighbouring node values and their distances</returns>
    public delegate IEnumerable<MoveData<TValue, TCost>> WeightedNeighbours<TValue, TCost>(TValue node) where TCost : INumber<TCost>;

    public delegate IEnumerable<T> Neighbours<T>(T node);

    #region Methods
    /// <summary>
    /// A* Search<br/>
    /// Searches from the starting node to the goal node, and retrieves the optimal path
    /// </summary>
    /// <typeparam name="TValue">Type of node being searched</typeparam>
    /// <typeparam name="TCost">Cost type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="goal">Goal node</param>
    /// <param name="heuristic">Heuristic function on the nodes</param>
    /// <param name="neighbours">Function finding neighbours for a given node</param>
    /// <param name="comparer">Comparer between different search nodes</param>
    /// <param name="goalFound">A function that compares the current and end nodes to test if the goal node has been reached</param>
    /// <returns>The optimal found path, or null if no path was found</returns>
    public static TValue[]? Search<TValue, TCost>(TValue start, TValue goal, SearchNode<TValue, TCost>.Heuristic? heuristic,
                                                  WeightedNeighbours<TValue, TCost> neighbours, IComparer<SearchNode<TValue, TCost>> comparer,
                                                  Func<TValue, TValue, bool>? goalFound = null) where TValue : IEquatable<TValue>
                                                                                                where TCost : INumber<TCost>
    {
        SearchNode<TValue, TCost>? foundGoal = null;
        PriorityQueue<SearchNode<TValue, TCost>> search = new(comparer);
        search.Enqueue(new(start));
        Dictionary<SearchNode<TValue, TCost>, TCost> explored = new();
        goalFound ??= (a, b) => a.Equals(b);

        while (search.TryDequeue(out SearchNode<TValue, TCost> current))
        {
            //If we found the goal
            if (goalFound(current.Value, goal))
            {
                foundGoal = current;
                break;
            }

            //Look through all neighbouring nodes
            foreach (SearchNode<TValue, TCost> neighbour in neighbours(current.Value).Select(n => new SearchNode<TValue, TCost>(current.CostSoFar + n.Cost,
                                                                                                                                n.Value, heuristic, current)))
            {
                //Check if it's in the closed list
                if (explored.TryGetValue(neighbour, out TCost? distance))
                {
                    //If it is, check if we found a quicker way
                    if (distance <= neighbour.CostSoFar) continue;

                    //If so, remove from closed list, and add back to open list
                    explored.Remove(neighbour);
                    search.Enqueue(neighbour);
                }
                //Check if it is in the open list
                else if (search.Contains(neighbour))
                {
                    //If so, update the value if necessary
                    search.Replace(neighbour);
                }
                else
                {
                    //Otherwise just add it
                    search.Enqueue(neighbour);
                }
            }

            //Add to the closed list after exploring
            explored.Add(current, current.CostSoFar);
        }

        //If the path is not found, return null
        if (foundGoal is null) return null;

        //Trace the path and backtrack
        Stack<TValue> path = new();
        //While the parent is not null
        while (foundGoal.Parent is not null)
        {
            //Push back and go deeper
            path.Push(foundGoal.Value);
            foundGoal = foundGoal.Parent;
        }

        //Copy the path back to an array and return
        return path.ToArray();
    }

    /// <summary>
    /// A* Search<br/>
    /// Searches from the starting node to the goal node, and retrieves the optimal path<br/>
    /// The search only cares about final path length, and the distances in each node of the final path is cached in the <paramref name="distances"/> dictionary
    /// </summary>
    /// <typeparam name="TValue">Type of node being searched</typeparam>
    /// <typeparam name="TCost">Cost type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="goal">Goal node</param>
    /// <param name="heuristic">Heuristic function on the nodes</param>
    /// <param name="neighbours">Function finding neighbours for a given node</param>
    /// <param name="comparer">Comparer between different search nodes</param>
    /// <param name="distances">Cached distances dictionary</param>
    /// <returns>The optimal found path, or null if no path was found</returns>
    public static int? GetPathLength<TValue, TCost>(TValue start, TValue goal,
                                                    SearchNode<TValue, TCost>.Heuristic? heuristic,
                                                    WeightedNeighbours<TValue, TCost> neighbours,
                                                    IComparer<SearchNode<TValue, TCost>> comparer,
                                                    Dictionary<(TValue, TValue), int> distances)
        where TValue : IEquatable<TValue>
        where TCost : INumber<TCost>
    {
        int foundDistance = 0;
        SearchNode<TValue, TCost>? foundGoal = null;
        PriorityQueue<SearchNode<TValue, TCost>> search = new(comparer);
        search.Enqueue(new(start));
        Dictionary<SearchNode<TValue, TCost>, TCost> explored = new();

        while (search.TryDequeue(out SearchNode<TValue, TCost> current))
        {
            //If we found the goal or the distance is cached
            if (current == goal || distances.TryGetValue((current.Value, goal), out foundDistance))
            {
                foundGoal = current;
                break;
            }

            //Look through all neighbouring nodes
            foreach (SearchNode<TValue, TCost> neighbour in neighbours(current.Value).Select(n => new SearchNode<TValue, TCost>(current.CostSoFar + n.Cost,
                                                                                                                                n.Value, heuristic, current)))
            {
                //Check if it's in the closed list
                if (explored.TryGetValue(neighbour, out TCost? distance))
                {
                    //If it is, check if we found a quicker way
                    if (!(distance > neighbour.CostSoFar)) continue;

                    //If so, remove from closed list, and add back to open list
                    explored.Remove(neighbour);
                    search.Enqueue(neighbour);
                }
                //Check if it is in the open list
                else if (search.Contains(neighbour))
                {
                    //If so, update the value if necessary
                    search.Replace(neighbour);
                }
                else
                {
                    //Otherwise just add it
                    search.Enqueue(neighbour);
                }
            }

            //Add to the closed list after exploring
            explored.Add(current, current.CostSoFar);
        }

        //If we found the goal
        if (foundGoal is null) return null;

        //While the parent is not null
        while (foundGoal.Parent is not null)
        {
            //Push back and go deeper
            foundGoal = foundGoal.Parent;
            distances.Add((foundGoal.Value, goal), ++foundDistance);
        }

        //Copy the path back to an array and return
        return foundDistance;
    }

    /// <summary>
    /// Breadth First Search path finding
    /// </summary>
    /// <typeparam name="T">Type of element to search for</typeparam>
    /// <param name="start">Starting point</param>
    /// <param name="goal">Ending point</param>
    /// <param name="neighbours">Neighbours function</param>
    /// <returns>The length of the path, if found, otherwise <see langword="null"/></returns>
    public static int? GetPathLengthBFS<T>(T start, T goal, Neighbours<T> neighbours) where T : IEquatable<T>
    {
        SearchNode<T>? foundGoal = null;
        Queue<SearchNode<T>> search = new();
        search.Enqueue(new(start));
        HashSet<SearchNode<T>> explored = [];

        while (search.TryDequeue(out SearchNode<T>? current))
        {
            // If we found the goal or the distance is cached
            if (current == goal)
            {
                foundGoal = current;
                break;
            }

            // Look through all neighbouring nodes
            foreach (SearchNode<T> neighbour in neighbours(current.Value).Select(n => new SearchNode<T>(current.CostSoFar + 1, n, current)))
            {
                //Check if it's in the closed or open list
                if (explored.Contains(neighbour) || search.Contains(neighbour)) continue;

                //Otherwise just add it
                search.Enqueue(neighbour);
            }

            // Add to the closed list after exploring
            explored.Add(current);
        }

        // Return path length
        return foundGoal?.Cost;
    }

    /// <summary>
    /// Depth First Search path finding
    /// </summary>
    /// <typeparam name="T">Type of element to search for</typeparam>
    /// <param name="start">Starting point</param>
    /// <param name="goal">Ending point</param>
    /// <param name="neighbours">Neighbours function</param>
    /// <returns>The length of the path, if found, otherwise <see langword="null"/></returns>
    public static int? GetPathLengthDFS<T>(T start, T goal, Neighbours<T> neighbours) where T : IEquatable<T>
    {
        SearchNode<T>? foundGoal = null;
        Stack<SearchNode<T>> search = new();
        search.Push(new(start));
        HashSet<SearchNode<T>> explored = [];

        while (search.TryPop(out SearchNode<T>? current))
        {
            // If we found the goal or the distance is cached
            if (current == goal)
            {
                foundGoal = current;
                break;
            }

            // Look through all neighbouring nodes
            foreach (SearchNode<T> neighbour in neighbours(current.Value).Select(n => new SearchNode<T>(current.CostSoFar + 1, n, current)))
            {
                //Check if it's in the closed or open list
                if (explored.Contains(neighbour) || search.Contains(neighbour)) continue;

                //Otherwise just add it
                search.Push(neighbour);
            }

            // Add to the closed list after exploring
            explored.Add(current);
        }

        // Return path length
        return foundGoal?.Cost;
    }

    /// <summary>
    /// Depth First Search path finding
    /// </summary>
    /// <typeparam name="T">Type of element to search for</typeparam>
    /// <param name="start">Starting point</param>
    /// <param name="goal">Ending point</param>
    /// <param name="neighbours">Neighbours function</param>
    /// <returns>The length of the path, if found, otherwise <see langword="null"/></returns>
    public static double? GetMaxPathLengthDFS<T>(T start, T goal, Neighbours<T> neighbours) where T : IEquatable<T>
    {
        List<SearchNode<T>> foundEndNodes = [];
        Stack<SearchNode<T>> search = new();
        search.Push(new(start));

        while (search.TryPop(out SearchNode<T>? current))
        {
            // If we found the goal or the distance is cached
            if (current == goal)
            {
                foundEndNodes.Add(current);
                continue;
            }

            // Look through all neighbouring nodes
            foreach (SearchNode<T> neighbour in neighbours(current.Value).Where(n => !current.HasParent(n))
                                                                         .Select(n => new SearchNode<T>(current.CostSoFar + 1, n, current)))
            {
                // Otherwise just add it
                search.Push(neighbour);
            }
        }

        // Return path length
        return !foundEndNodes.IsEmpty() ? foundEndNodes.Max(n => n.Cost) : null;
    }
    #endregion
}