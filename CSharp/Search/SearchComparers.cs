using System.Collections.Generic;

namespace AdventOfCode.Search
{
    /// <summary>
    /// Minimum value search comparer
    /// </summary>
    public sealed class MinSearchComparer : IComparer<ISearchNode>
    {
        #region Static properties
        /// <summary>
        /// Comparer instance
        /// </summary>
        public static MinSearchComparer Comparer { get; } = new();
        #endregion
        
        #region Constructors
        /// <summary>
        /// Private constructor, prevents instantiation
        /// </summary>
        private MinSearchComparer() { }
        #endregion

        #region Methods
        /// <inheritdoc cref="IComparer{T}"/>
        public int Compare(ISearchNode? a, ISearchNode? b) => a?.Cost.CompareTo(b?.Cost) ?? 0;
        #endregion
    }
    
    /// <summary>
    /// Maximum value search comparer
    /// </summary>
    public sealed class MaxSearchComparer : IComparer<ISearchNode>
    {
        #region Static properties
        /// <summary>
        /// Comparer instance
        /// </summary>
        public static MaxSearchComparer Comparer { get; } = new();
        #endregion
        
        #region Constructors
        /// <summary>
        /// Private constructor, prevents instantiation
        /// </summary>
        private MaxSearchComparer() { }
        #endregion

        #region Methods
        /// <inheritdoc cref="IComparer{T}"/>
        public int Compare(ISearchNode? a, ISearchNode? b) => b?.Cost.CompareTo(a?.Cost) ?? 0;
        #endregion
    }
}
