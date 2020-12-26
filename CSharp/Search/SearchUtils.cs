using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids.Vectors;

namespace AdventOfCode.Search
{
    /// <summary>
    /// Searching utility methods
    /// </summary>
    public static class SearchUtils
    {
        /// <summary>
        /// Neighbours exploration function
        /// </summary>
        /// <typeparam name="T">Type of object to explore</typeparam>
        /// <param name="node">Node to explore</param>
        /// <returns>An enumerable of all neighbouring node values and their distances</returns>
        public delegate IEnumerable<(T value, double distance)> Neighbours<T>(T node);

        #region Methods
        /// <summary>
        /// A* Search<br/>
        /// Searches from the starting node to the goal node, and retrieves the optimal path
        /// </summary>
        /// <typeparam name="T">Type of node being searched</typeparam>
        /// <param name="start">Starting node</param>
        /// <param name="goal">Goal node</param>
        /// <param name="heuristic">Heuristic function on the nodes</param>
        /// <param name="neighbours">Function finding neighbours for a given node</param>
        /// <param name="comparer">Comparer between different search nodes</param>
        /// <returns>The optimal found path, or null if no path was found</returns>
        public static T[]? Search<T>(T start, T goal, SearchNode<T>.Heuristic heuristic, Neighbours<T> neighbours, IComparer<SearchNode<T>> comparer) where T : IEquatable<T>
        {
            SearchNode<T>? foundGoal = null;
            PriorityQueue<SearchNode<T>> search = new(comparer) { new SearchNode<T>(start) };
            Dictionary<SearchNode<T>, double> explored = new();
            while (search.TryPop(out SearchNode<T> current))
            {
                //If we found the goal
                if (current == goal)
                {
                    foundGoal = current;
                    break;
                }

                //Look through all neighbouring nodes
                foreach (SearchNode<T> neighbour in neighbours(current.Value).Select(n => new SearchNode<T>(current.CostSoFar + n.distance, n.value, heuristic, current)))
                {
                    //Check if it's in the closed list
                    if (explored.TryGetValue(neighbour, out double distance))
                    {
                        //If it is, check if we found a quicker way
                        if (distance > neighbour.CostSoFar)
                        {
                            //If so, remove from closed list, and add back to open list
                            explored.Remove(neighbour);
                            search.Add(neighbour);
                        }
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
                        search.Add(neighbour);
                    }
                }
                
                //Add to the closed list after exploring
                explored.Add(current, current.CostSoFar);
            }

            //If we found the goal
            if (foundGoal is not null)
            {
                //Trace the path and backtrack
                Stack<T> path = new();
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

            //If the path is not found, return null
            return null;
        }

        /// <summary>
        /// A* Search<br/>
        /// Searches from the starting node to the goal node, and retrieves the optimal path<br/>
        /// The search only cares about final path length, and the distances in each node of the final path is cached in the <paramref name="distances"/> dictionary
        /// </summary>
        /// <typeparam name="T">Type of node being searched</typeparam>
        /// <param name="start">Starting node</param>
        /// <param name="goal">Goal node</param>
        /// <param name="heuristic">Heuristic function on the nodes</param>
        /// <param name="neighbours">Function finding neighbours for a given node</param>
        /// <param name="comparer">Comparer between different search nodes</param>
        /// <param name="distances">Cached distances dictionary</param>
        /// <returns>The optimal found path, or null if no path was found</returns>
        public static int? GetPathLength<T>(T start, T goal, SearchNode<T>.Heuristic heuristic, Neighbours<T> neighbours, IComparer<SearchNode<T>> comparer, Dictionary<(T, T), int> distances) where T : IEquatable<T>
        {
            int foundDistance = 0;
            SearchNode<T>? foundGoal = null;
            PriorityQueue<SearchNode<T>> search = new(comparer) { new SearchNode<T>(start) };
            Dictionary<SearchNode<T>, double> explored = new();
            while (search.TryPop(out SearchNode<T> current))
            {
                //If we found the goal or the distance is cached
                if (current == goal || distances.TryGetValue((current.Value, goal), out foundDistance))
                {
                    foundGoal = current;
                    break;
                }

                //Look through all neighbouring nodes
                foreach (SearchNode<T> neighbour in neighbours(current.Value).Select(n => new SearchNode<T>(current.CostSoFar + n.distance, n.value, heuristic, current)))
                {
                    //Check if it's in the closed list
                    if (explored.TryGetValue(neighbour, out double distance))
                    {
                        //If it is, check if we found a quicker way
                        if (distance > neighbour.CostSoFar)
                        {
                            //If so, remove from closed list, and add back to open list
                            explored.Remove(neighbour);
                            search.Add(neighbour);
                        }
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
                        search.Add(neighbour);
                    }
                }
                
                //Add to the closed list after exploring
                explored.Add(current, current.CostSoFar);
            }

            //If we found the goal
            if (foundGoal is not null)
            {
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

            //If the path is not found, return null
            return null;
        }
        #endregion
    }
}
