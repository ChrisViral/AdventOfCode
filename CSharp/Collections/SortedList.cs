using System.Collections.Generic;

namespace AdventOfCode.Collections
{
    /// <summary>
    /// Sorted list where the value is it's own key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedList<T> : SortedList<T, T>, IEnumerable<T> where T : notnull
    {
        /// <summary>
        /// Gets the value at the given index in the list
        /// </summary>
        /// <param name="index">Index to get at</param>
        /// <returns>The value at the given index</returns>
        public T this[int index] => base.Keys[index];

        /// <summary>
        /// Adds the given value to the sorted list
        /// </summary>
        /// <param name="value">Value to add</param>
        public void Add(T value) => base.Add(value, value);

        /// <summary>
        /// Checks if the value is present within the list
        /// </summary>
        /// <param name="value">Value to find</param>
        /// <returns>True if the value is in the list, false otherwise</returns>
        public bool Contains(T value) => base.ContainsKey(value);

        /// <summary>
        /// Index of the value in the list
        /// </summary>
        /// <param name="value">Value to get the index for</param>
        /// <returns>The index of the value, or -1 if the value is not within the list</returns>
        public int IndexOf(T value) => base.IndexOfKey(value);

        /// <summary>
        /// Iterates over the values of the list, in sorted order
        /// </summary>
        /// <returns>An enumerator over the sorted list</returns>
        public new IEnumerator<T> GetEnumerator() => base.Keys.GetEnumerator();

        #region Hidden methods
        private new IList<T> Keys => base.Keys;
        private new IList<T> Values => base.Values;
        private new void Add(T key, T value) => base.Add(key, value);
        private new bool ContainsKey(T key) => base.ContainsKey(key);
        private new bool ContainsValue(T value) => base.ContainsValue(value);
        private new int IndexOfKey(T key) => base.IndexOfKey(key);
        private new int IndexOfValue(T value) => base.IndexOfValue(value);
        private new bool TryGetValue(T key, out T value) => base.TryGetValue(key, out value!);
        #endregion
    }
}
