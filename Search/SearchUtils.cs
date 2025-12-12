using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Utils.Pooling;
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
public readonly record struct MoveData<TValue, TCost>(TValue Value, TCost Cost) where TCost : INumber<TCost>;

/// <summary>
/// Searching utility methods
/// </summary>
[PublicAPI]
public static class SearchUtils
{
    /// <summary>
    /// Neighbours exploration function
    /// </summary>
    /// <typeparam name="T">Type of object to explore</typeparam>
    /// <param name="node">Node to explore</param>
    /// <returns>An enumerable of all neighbouring node values</returns>
    public delegate IEnumerable<T> Neighbours<T>(T node);

    /// <summary>
    /// Neighbours exploration function
    /// </summary>
    /// <typeparam name="TValue">Type of object to explore</typeparam>
    /// <typeparam name="TCost">Cost type</typeparam>
    /// <param name="node">Node to explore</param>
    /// <returns>An enumerable of all neighbouring node values and their distances</returns>
    public delegate IEnumerable<MoveData<TValue, TCost>> WeightedNeighbours<TValue, TCost>(TValue node) where TCost : INumber<TCost>;

    /// <summary>
    /// Success state check function
    /// </summary>
    /// <typeparam name="TValue">Type of value being explored</typeparam>
    /// <returns><see langword="true"/> if <paramref name="current"/> is deemed to be equivalent to <paramref name="goal"/>, otherwise <see langword="false"/></returns>
    public delegate bool GoalFoundCheck<in TValue>(TValue current, TValue goal);

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
    /// <param name="totalCost">Total path cost output</param>
    /// <param name="goalFound">A function that compares the current and end nodes to test if the goal node has been reached</param>
    /// <returns>The optimal found path, or null if no path was found</returns>
    /// ReSharper disable once CognitiveComplexity
    public static TValue[]? Search<TValue, TCost>(TValue start, TValue goal,
                                                  [InstantHandle] SearchNode<TValue, TCost>.Heuristic? heuristic,
                                                  [InstantHandle] WeightedNeighbours<TValue, TCost> neighbours,
                                                  IComparer<SearchNode<TValue, TCost>> comparer,
                                                  out TCost totalCost,
                                                  [InstantHandle] GoalFoundCheck<TValue>? goalFound = null)
        where TValue : IEquatable<TValue>
        where TCost : INumber<TCost>
    {
        SearchNode<TValue, TCost>? foundGoal = null;
        Pooled<PriorityQueue<SearchNode<TValue, TCost>>> search = PriorityQueueObjectPool<SearchNode<TValue, TCost>>.PoolForComparer(comparer).Get();
        search.Ref.Enqueue(new SearchNode<TValue, TCost>(start));
        Pooled<Dictionary<SearchNode<TValue, TCost>, TCost>> explored = DictionaryObjectPool<SearchNode<TValue, TCost>, TCost>.Shared.Get();
        goalFound ??= (a, b) => a.Equals(b);

        while (search.Ref.TryDequeue(out SearchNode<TValue, TCost>? current))
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
                if (explored.Ref.TryGetValue(neighbour, out TCost? distance))
                {
                    //If it is, check if we found a quicker way
                    if (distance <= neighbour.CostSoFar) continue;

                    //If so, remove from closed list, and add back to open list
                    explored.Ref.Remove(neighbour);
                    search.Ref.Enqueue(neighbour);
                }
                //Check if it is in the open list
                else if (search.Ref.Contains(neighbour))
                {
                    //If so, update the value if necessary
                    search.Ref.Replace(neighbour);
                }
                else
                {
                    //Otherwise just add it
                    search.Ref.Enqueue(neighbour);
                }
            }

            //Add to the closed list after exploring
            explored.Ref.Add(current, current.CostSoFar);
        }

        //If the path is not found, return null
        if (foundGoal is null)
        {
            totalCost = TCost.Zero;
            return null;
        }

        totalCost = foundGoal.CostSoFar;

        //Trace the path and backtrack
        Pooled<Stack<TValue>> path = StackObjectPool<TValue>.Shared.Get();
        //While the parent is not null
        while (foundGoal.Parent is not null)
        {
            //Push back and go deeper
            path.Ref.Push(foundGoal.Value);
            foundGoal = foundGoal.Parent;
        }

        //Copy the path back to an array and return
        return path.Ref.ToArray();
    }

    /// <summary>
    /// A* Search<br/>
    /// Searches from the starting node to the goal node, and retrieves the optimal path<br/>
    /// All the various paths that can reach the target node are also returned
    /// </summary>
    /// <typeparam name="TValue">Type of node being searched</typeparam>
    /// <typeparam name="TCost">Cost type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="goal">Goal node</param>
    /// <param name="heuristic">Heuristic function on the nodes</param>
    /// <param name="neighbours">Function finding neighbours for a given node</param>
    /// <param name="comparer">Comparer between different search nodes</param>
    /// <param name="goalFound">A function that compares the current and end nodes to test if the goal node has been reached</param>
    /// <returns>A tuple containing the optimal found path as well as a set of all nodes on a other optimal paths, or null if no path was found</returns>
    /// ReSharper disable once CognitiveComplexity
    public static (TValue[]?, HashSet<TValue>?) SearchEquivalentPaths<TValue, TCost>(TValue start, TValue goal,
                                                                                     [InstantHandle] SearchNode<TValue, TCost>.Heuristic? heuristic,
                                                                                     [InstantHandle] WeightedNeighbours<TValue, TCost> neighbours,
                                                                                     IComparer<SearchNode<TValue, TCost>> comparer,
                                                                                     [InstantHandle] GoalFoundCheck<TValue>? goalFound = null)
        where TValue : IEquatable<TValue>
        where TCost : INumber<TCost>
    {
        SearchNode<TValue, TCost>? foundGoal = null;
        Pooled<PriorityQueue<SearchNode<TValue, TCost>>> search = PriorityQueueObjectPool<SearchNode<TValue, TCost>>.PoolForComparer(comparer).Get();
        search.Ref.Enqueue(new SearchNode<TValue, TCost>(start));
        Pooled<Dictionary<SearchNode<TValue, TCost>, TCost>> explored = DictionaryObjectPool<SearchNode<TValue, TCost>, TCost>.Shared.Get();
        Pooled<Dictionary<SearchNode<TValue, TCost>, List<SearchNode<TValue, TCost>>>> equivalentNodes =
            DictionaryObjectPool<SearchNode<TValue, TCost>, List<SearchNode<TValue, TCost>>>.Shared.Get();
        goalFound ??= (a, b) => a.Equals(b);

        while (search.Ref.TryDequeue(out SearchNode<TValue, TCost>? current))
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
                if (explored.Ref.TryGetValue(neighbour, out TCost? distance))
                {
                    //If it is, check if we found a quicker way
                    if (distance <= neighbour.CostSoFar) continue;

                    //If so, remove from closed list, and add back to open list
                    explored.Ref.Remove(neighbour);
                    search.Ref.Enqueue(neighbour);
                    equivalentNodes.Ref[neighbour] = [neighbour];
                }
                //Check if it is in the open list
                else if (search.Ref.Contains(neighbour))
                {
                    //If so, update the value if necessary
                    search.Ref.Replace(neighbour);

                    List<SearchNode<TValue, TCost>> otherBranches = equivalentNodes.Ref[neighbour];
                    SearchNode<TValue, TCost> previous = otherBranches[0];
                    if (previous.CostSoFar == neighbour.CostSoFar)
                    {
                        otherBranches.Add(neighbour);
                    }
                    else if (previous.CostSoFar > neighbour.CostSoFar)
                    {
                        equivalentNodes.Ref[neighbour] = [neighbour];
                    }
                }
                else
                {
                    //Otherwise just add it
                    search.Ref.Enqueue(neighbour);
                    equivalentNodes.Ref[neighbour] = [neighbour];
                }
            }

            //Add to the closed list after exploring
            explored.Ref.Add(current, current.CostSoFar);
        }

        //If the path is not found, return null
        if (foundGoal is null) return (null, null);

        //Trace the path and backtrack
        Pooled<Stack<TValue>> path = StackObjectPool<TValue>.Shared.Get();
        Pooled<HashSet<TValue>> unique = HashSetObjectPool<TValue>.Shared.Get();
        Pooled<Queue<SearchNode<TValue, TCost>>> branchesQueue = QueueObjectPool<SearchNode<TValue, TCost>>.Shared.Get();

        //While the parent is not null
        while (foundGoal.Parent is not null)
        {
            //Push back and go deeper
            path.Ref.Push(foundGoal.Value);
            unique.Ref.Add(foundGoal.Value);
            foundGoal = foundGoal.Parent;
            if (!equivalentNodes.Ref.TryGetValue(foundGoal, out List<SearchNode<TValue, TCost>>? branches) || branches.Count <= 1) continue;

            foreach (SearchNode<TValue, TCost> branch in branches.Where(branch => !ReferenceEquals(branch, foundGoal)))
            {
                branchesQueue.Ref.Enqueue(branch);
            }
        }

        unique.Ref.Add(foundGoal.Value);

        while (branchesQueue.Ref.TryDequeue(out foundGoal))
        {
            //While the parent is not null
            while (foundGoal.Parent is not null)
            {
                //Push back and go deeper
                unique.Ref.Add(foundGoal.Value);
                foundGoal = foundGoal.Parent;

                if (!equivalentNodes.Ref.TryGetValue(foundGoal, out List<SearchNode<TValue, TCost>>? branches) || branches.Count <= 1) continue;

                foreach (SearchNode<TValue, TCost> branch in branches.Where(branch => !ReferenceEquals(branch, foundGoal)))
                {
                    branchesQueue.Ref.Enqueue(branch);
                }
            }

            unique.Ref.Add(foundGoal.Value);
        }

        //Copy the path back to an array and return
        return (path.Ref.ToArray(), unique.Ref);
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
    /// <param name="distances">Cached distances dictionary, creates a temporary one with 100 base capacity if not passed</param>
    /// <param name="goalFound">A function that compares the current and end nodes to test if the goal node has been reached</param>
    /// <returns>The optimal found path, or null if no path was found</returns>
    /// ReSharper disable once CognitiveComplexity
    public static int? GetPathLength<TValue, TCost>(TValue start, TValue goal,
                                                    [InstantHandle] SearchNode<TValue, TCost>.Heuristic? heuristic,
                                                    [InstantHandle] WeightedNeighbours<TValue, TCost> neighbours,
                                                    IComparer<SearchNode<TValue, TCost>> comparer,
                                                    Dictionary<(TValue, TValue), int>? distances = null,
                                                    [InstantHandle] GoalFoundCheck<TValue>? goalFound = null)
        where TValue : IEquatable<TValue>
        where TCost : INumber<TCost>
    {
        int foundDistance = 0;
        SearchNode<TValue, TCost>? foundGoal = null;
        Pooled<PriorityQueue<SearchNode<TValue, TCost>>> search = PriorityQueueObjectPool<SearchNode<TValue, TCost>>.PoolForComparer(comparer).Get();
        search.Ref.Enqueue(new SearchNode<TValue, TCost>(start));
        Pooled<Dictionary<SearchNode<TValue, TCost>, TCost>> explored = DictionaryObjectPool<SearchNode<TValue, TCost>, TCost>.Shared.Get();
        goalFound ??= (a, b) => a.Equals(b);

        using Pooled<Dictionary<(TValue, TValue), int>> pooledDistances = distances is null ? DictionaryObjectPool<(TValue, TValue), int>.Shared.Get() : default;
        distances ??= pooledDistances.Ref;

        while (search.Ref.TryDequeue(out SearchNode<TValue, TCost>? current))
        {
            //If we found the goal or the distance is cached
            if (goalFound(current.Value, goal) || distances.TryGetValue((current.Value, goal), out foundDistance))
            {
                foundGoal = current;
                break;
            }

            //Look through all neighbouring nodes
            foreach (SearchNode<TValue, TCost> neighbour in neighbours(current.Value).Select(n => new SearchNode<TValue, TCost>(current.CostSoFar + n.Cost,
                                                                                                                                n.Value, heuristic, current)))
            {
                //Check if it's in the closed list
                if (explored.Ref.TryGetValue(neighbour, out TCost? distance))
                {
                    //If it is, check if we found a quicker way
                    if (!(distance > neighbour.CostSoFar)) continue;

                    //If so, remove from closed list, and add back to open list
                    explored.Ref.Remove(neighbour);
                    search.Ref.Enqueue(neighbour);
                }
                //Check if it is in the open list
                else if (search.Ref.Contains(neighbour))
                {
                    //If so, update the value if necessary
                    search.Ref.Replace(neighbour);
                }
                else
                {
                    //Otherwise just add it
                    search.Ref.Enqueue(neighbour);
                }
            }

            //Add to the closed list after exploring
            explored.Ref.Add(current, current.CostSoFar);
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
        Pooled<Queue<SearchNode<T>>> search = QueueObjectPool<SearchNode<T>>.Shared.Get();
        search.Ref.Enqueue(new SearchNode<T>(start));
        Pooled<HashSet<SearchNode<T>>> explored = HashSetObjectPool<SearchNode<T>>.Shared.Get();

        while (search.Ref.TryDequeue(out SearchNode<T>? current))
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
                if (explored.Ref.Contains(neighbour) || search.Ref.Contains(neighbour)) continue;

                //Otherwise just add it
                search.Ref.Enqueue(neighbour);
            }

            // Add to the closed list after exploring
            explored.Ref.Add(current);
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
        Pooled<Stack<SearchNode<T>>> search = StackObjectPool<SearchNode<T>>.Shared.Get();
        search.Ref.Push(new SearchNode<T>(start));
        Pooled<HashSet<SearchNode<T>>> explored = HashSetObjectPool<SearchNode<T>>.Shared.Get();

        while (search.Ref.TryPop(out SearchNode<T>? current))
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
                if (explored.Ref.Contains(neighbour) || search.Ref.Contains(neighbour)) continue;

                //Otherwise just add it
                search.Ref.Push(neighbour);
            }

            // Add to the closed list after exploring
            explored.Ref.Add(current);
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
        Pooled<List<SearchNode<T>>> foundEndNodes = ListObjectPool<SearchNode<T>>.Shared.Get();
        Pooled<Stack<SearchNode<T>>> search = StackObjectPool<SearchNode<T>>.Shared.Get();
        search.Ref.Push(new SearchNode<T>(start));

        while (search.Ref.TryPop(out SearchNode<T>? current))
        {
            // If we found the goal or the distance is cached
            if (current == goal)
            {
                foundEndNodes.Ref.Add(current);
                continue;
            }

            // Look through all neighbouring nodes
            foreach (SearchNode<T> neighbour in neighbours(current.Value).Where(n => !current.HasParent(n))
                                                                         .Select(n => new SearchNode<T>(current.CostSoFar + 1, n, current)))
            {
                // Otherwise just add it
                search.Ref.Push(neighbour);
            }
        }

        // Return path length
        return !foundEndNodes.Ref.IsEmpty ? foundEndNodes.Ref.Max(n => n.Cost) : null;
    }

    /// <summary>
    /// Counts all possible pathes from start through to the goal
    /// </summary>
    /// <typeparam name="T">Type of element to search for</typeparam>
    /// <param name="start">Starting point</param>
    /// <param name="goal">Ending point</param>
    /// <param name="neighbours">Neighbours function</param>
    /// <returns>The total count of pathes found</returns>
    public static int CountPossiblePaths<T>(T start, T goal, Neighbours<T> neighbours) where T : IEquatable<T>
    {
        Pooled<Queue<T>> search = QueueObjectPool<T>.Shared.Get();
        search.Ref.Enqueue(start);

        int possiblePaths = 0;
        while (search.Ref.TryDequeue(out T? current))
        {
            // If we found the goal
            if (current.Equals(goal))
            {
                possiblePaths++;
                continue;
            }

            // Look through all neighbouring nodes
            foreach (T neighbour in neighbours(current))
            {
                search.Ref.Enqueue(neighbour);
            }
        }

        // Return path length
        return possiblePaths;
    }
}
