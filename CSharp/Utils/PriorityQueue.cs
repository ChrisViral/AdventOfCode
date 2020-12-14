using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/* ==================================================================================== *\
 * Copyright © 2020 Christophe Savard                                                   *
 *                                                                                      *
 * Permission is hereby granted, free of charge, to any person obtaining a copy         *
 * of this software and associated documentation files (the "Software"),                *
 * to deal in the Software without restriction, including without limitation            *
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,             *
 * and/or sell copies of the Software, and to permit persons to whom the                *
 * Software is furnished to do so, subject to the following conditions:                 *
 *                                                                                      *
 * The above copyright notice and this permission notice shall be included              *
 * in all copies or substantial portions of the Software.                               *
 *                                                                                      *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,  *
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR          *
 * A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR           *
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN *
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION         *
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                      *
\* ==================================================================================== */

namespace AdventOfCode.Utils
{
    /// <summary>
    /// A generic Priority Queue implementation using a binary heap.
    /// Most operations are O(log n), while full enumeration or copying to an array is O(n log n)
    /// </summary>
    /// <typeparam name="T">Type of the queue</typeparam>
    public class PriorityQueue<T> : ICollection<T> where T : notnull
    {
        #region Constants
        /// <summary>
        /// Base capacity of the heap list
        /// </summary>
        private const int BASE_CAPACITY = 4;
        #endregion

        #region Fields
        //List containing the binary heap
        private readonly List<T> heap;
        //Item-index map
        private readonly Dictionary<T, int> indices;
        //Comparer to sort the items
        private readonly IComparer<T> comparer;
        #endregion

        #region Properties
        /// <summary>
        /// Amount of items stored in the queue
        /// </summary>
        public int Count => this.heap.Count;

        /// <summary>
        /// If the queue is currently empty
        /// </summary>
        public bool IsEmpty => this.heap.Count is 0;

        /// <summary>
        /// Current capacity of the queue
        /// </summary>
        public int Capacity => this.heap.Capacity;

        /// <summary>
        /// If the collection is read only. Since we are using List{T}, it never is.
        /// </summary>
        bool ICollection<T>.IsReadOnly { get; } = false;

        /// <summary>
        /// Index of the last member
        /// </summary>
        private int Last => this.heap.Count - 1;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty PriorityQueue using the default IComparer of <typeparamref name="T"/>
        /// </summary>
        public PriorityQueue() : this(BASE_CAPACITY, Comparer<T>.Default) { }

        /// <summary>
        /// Creates a PriorityQueue of the given capacity with the default IComparer of <typeparamref name="T"/>
        /// </summary>
        /// <param name="capacity">Capacity of the PriorityQueue</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="capacity"/> is lower or equal to zero</exception>
        public PriorityQueue(int capacity) : this(capacity, Comparer<T>.Default) { }

        /// <summary>
        /// Creates an empty PriorityQueue with the provided IComparer of <typeparamref name="T"/>
        /// </summary>
        /// <param name="comparer">Comparer to sort the Queue</param>
        public PriorityQueue(IComparer<T> comparer) : this(BASE_CAPACITY, comparer) { }

        /// <summary>
        /// Creates an PriorityQueue of the given size with the IComparer of <typeparamref name="T"/> provided
        /// </summary>
        /// <param name="capacity">Capacity of the queue</param>
        /// <param name="comparer">Comparer to sort the Queue</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="capacity"/> is lower or equal to zero</exception>
        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = comparer;
            this.heap = new List<T>(capacity);
            this.indices = new Dictionary<T, int>(capacity);
        }

        /// <summary>
        /// Creates the PriorityQueue from the IEnumerable provided with the default IComparer of <typeparamref name="T"/>
        /// This constructor runs in O(n log n)
        /// </summary>
        /// <param name="enumerable">Enumerable to make the StopQueue from</param>
        public PriorityQueue(IEnumerable<T> enumerable) : this(enumerable, Comparer<T>.Default) { }

        /// <summary>
        /// Creates the PriorityQueue from the IEnumerable provided with the IComparer of <typeparamref name="T"/> provided
        /// This constructor runs in O(n log n)
        /// </summary>
        /// <param name="enumerable">Enumerable to make the PriorityQueue from</param>
        /// <param name="comparer">Comparer to sort the values</param>
        public PriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer)
        {
            //Create the comparer and heap
            this.comparer = comparer;
            this.heap = new List<T>(enumerable);
            this.indices = new Dictionary<T, int>(this.heap.Count);

            //Heapify the list
            for (int i = this.heap.Count / 2; i >= 1; i--)
            {
                HeapDown(i);
            }
            //Setup index map
            for (int i = 0; i < this.Count; i++)
            {
                this.indices.Add(this.heap[i], i);
            }
        }

        /// <summary>
        /// Copy constructor, creates a new PriorityQueue of <typeparamref name="T"/> from an existing one
        /// This constructor runs in O(n)
        /// </summary>
        /// <param name="queue">Queue to copy from</param>
        public PriorityQueue(PriorityQueue<T> queue)
        {
            this.heap = new List<T>(queue.heap);
            this.indices = new Dictionary<T, int>(queue.indices);
            this.comparer = queue.comparer;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Compares the values at indices i and j and finds out if i must be moved upwards
        /// </summary>
        /// <param name="i">Index of the bottom node</param>
        /// <param name="j">Index of the top node</param>
        /// <returns>If i must be moved to j</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareUp(int i, int j) => i > 0 && i < this.heap.Count && this.comparer.Compare(this.heap[i], this.heap[j]) < 0;

        /// <summary>
        /// Compares the values at indices i and j and finds out if i must be moved downwards
        /// </summary>
        /// <param name="i">Index of top value</param>
        /// <param name="j">Index of bottom value</param>
        /// <returns>If i must be moved to j</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareDown(int i, int j) => j < this.heap.Count && this.comparer.Compare(this.heap[i], this.heap[j]) > 0;

        /// <summary>
        /// Moves the target <typeparamref name="T"/> up until it satisfies heap priority
        /// Heaping up is an O(log n) operation
        /// </summary>
        /// <param name="i">Index of the value to move</param>
        private void HeapUp(int i)
        {
            //If index is zero it's already at the top
            if (i is 0) { return; }

            //Store value to move
            T value = this.heap[i];
            int j = Parent(i);
            while (CompareUp(i, j))
            {
                //Swap values downwards
                this.heap[i] = this.heap[j];
                this.heap[j] = value;
                i = j;
                j = Parent(i);
            }
            //Set value to final index
            this.heap[i] = value;
            this.indices[value] = i;
        }

        /// <summary>
        /// Moves the <typeparamref name="T"/> down until it satisfies heap priority
        /// Heaping down is an O(log n) operation
        /// </summary>
        /// <param name="i">Index of the value to move</param>
        private void HeapDown(int i)
        {
            //Move down until priority is satisfied
            bool moving;
            do
            {
                //Reset moving flag
                moving = false;
                //Index of the left, right, and current nodes
                int l = LeftChild(i), r = RightChild(i), largest = i;
                if (CompareDown(i, l))
                {
                    //If left smaller, possibly move this way
                    largest = l;
                }

                if (CompareDown(largest, r))
                {
                    //If right smaller, move this way
                    largest = r;
                }

                //Check if the new target index is further than the current index
                if (largest > i)
                {
                    //Swap and keep moving down
                    T temp = this.heap[i];
                    this.heap[i] = this.heap[largest];
                    this.heap[largest] = temp;
                    i = largest;
                    moving = true;
                }
                else if (i >= 0 && i < this.heap.Count)
                {
                    //Otherwise set
                    this.indices[this.heap[i]] = i;
                }
            }
            while (moving);
        }

        /// <summary>
        /// Adds a value in the queue by priority
        /// This operation is O(log n)
        /// </summary>
        /// <param name="value"><typeparamref name="T"/> to add. Cannot be null as it isn't sortable</param>
        public void Add(T value)
        {
            //Add to the end and heap upwards
            this.heap.Add(value);
            this.indices.Add(value, 0);
            HeapUp(this.Last);
        }

        /// <summary>
        /// Removes and returns the first element of the queue by priority
        /// This operation is O(log n)
        /// </summary>
        /// <returns>The first element of the queue</returns>
        /// <exception cref="InvalidOperationException">Cannot pop if the queue is empty</exception>
        public T Pop()
        {
            //Make sure the queue isn't empty
            if (this.IsEmpty) throw new InvalidOperationException("Queue empty, operation invalid");

            //Remove the top value and return it
            T value = this.heap[0];
            RemoveAt(0, value);
            //If needed, heap the top value back down to it's new place
            if (this.Count > 0)
            {
                HeapDown(0);
            }
            return value;
        }

        /// <summary>
        /// Returns without removing the first <typeparamref name="T"/> in the queue
        /// This is an O(1) operation
        /// </summary>
        /// <returns>First Item in the queue</returns>
        /// <exception cref="InvalidOperationException">Cannot peek if the queue is empty</exception>
        public T Peek() => !this.IsEmpty ? this.heap[0] : throw new InvalidOperationException("Queue empty, operation invalid");

        /// <summary>
        /// Removes the given value at the given index, and puts the last element of the list in it's place
        /// This operation is O(1)
        /// </summary>
        /// <param name="i">Index of the value to remove</param>
        /// <param name="value">Value to remove</param>
        private void RemoveAt(int i, T value)
        {
            //Swap the value to the end then remove it
            this.heap[i] = this.heap[this.Last];
            this.heap.RemoveAt(this.Last);
            this.indices.Remove(value);
        }

        /// <summary>
        /// Removes the provided <typeparamref name="T"/> from the queue, if null is passed, the function returns false
        /// This is an O(log n) operation
        /// </summary>
        /// <param name="value">Value to remove</param>
        /// <returns>If the value was successfully removed</returns>
        public bool Remove(T value)
        {
            //Get the index in the list of the value
            if (this.indices.TryGetValue(value, out int i))
            {
                //Remove it from the heap and update the heap position of the displaced object
                RemoveAt(i, value);
                Update(i);
                return true;
            }
            //If not found, return false
            return false;
        }

        /// <summary>
        /// If the queue contains the given value
        /// This is an O(1) operation
        /// </summary>
        /// <param name="value">Value to find</param>
        /// <returns>True when the queue contains the value, false otherwise</returns>
        public bool Contains(T value) => this.indices.ContainsKey(value);

        /// <summary>
        /// Clears the memory of the queue
        /// </summary>
        public void Clear()
        {
            this.heap.Clear();
            this.indices.Clear();
        }

        /// <summary>
        /// Updates the position of an element that has been modified
        /// </summary>
        /// <param name="value">Element that has been modified to move</param>
        /// <returns>If the element was found and updated correctly</returns>>
        public bool Update(T value)
        {
            //Makes sure object is within
            if (this.indices.TryGetValue(value, out int i))
            {
                //Update the position
                Update(i);
                return true;
            }
            //If the object was not found, return false
            return false;
        }

        /// <summary>
        /// Updates the <typeparamref name="T"/> at the index i until it satisfies heap priority
        /// This is an O(log n) operation
        /// </summary>
        /// <param name="i">Index of the element to update</param>
        private void Update(int i)
        {
            //Get the parent of this index
            int j = Parent(i);
            //If needs to be moved up
            if (j >= 0 && j < this.heap.Count && CompareUp(i, j))
            {
                //Then heap up
                HeapUp(i);
            }
            else
            {
                //Else heap down
                HeapDown(i);
            }
        }

        /// <summary>
        /// Copies the PriorityQueue to a generic array
        /// This is an O(n log n) operation.
        /// </summary>
        /// <param name="array">Array to copy to</param>
        public void CopyTo(T[] array) => CopyTo(array, 0);

        /// <summary>
        /// Copies the PriorityQueue to a generic array
        /// This is an O(n log n) operation.
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Starting index in the target array to put the objects in</param>
        public void CopyTo(T[] array, int index)
        {
            this.heap.CopyTo(array, index);
            Array.Sort(array, this.comparer);
        }

        /// <summary>
        /// Returns this queue in a sorted array
        /// This is an O(n log n) operation.
        /// </summary>
        /// <returns>Array of the queue</returns>
        public T[] ToArray()
        {
            if (this.IsEmpty) return Array.Empty<T>();
            
            T[] a = this.heap.ToArray();
            Array.Sort(a, this.comparer);
            return a;
        }

        /// <summary>
        /// Returns this queue in a sorted List of <typeparamref name="T"/>
        /// This is an O(n log n) operation.
        /// </summary>
        /// <returns>List of this queue</returns>
        public List<T> ToList()
        {
            if (this.IsEmpty) return new List<T>();
            
            List<T> l = new(this.heap);
            l.Sort(this.comparer);
            return l;
        }

        /// <summary>
        /// Trims the memory of the PriorityQueue to it's size if more than 10% of the memory is unused
        /// </summary>
        public void TrimExcess() => this.heap.TrimExcess();

        /// <summary>
        /// Returns an IEnumerator of <typeparamref name="T"/> from this PriorityQueue
        /// WARNING: Obtaining the first element of the iterator is O(n). Every subsequent elements is O(log n)
        /// </summary>
        /// <returns>Iterator going through this sequence</returns>
        public IEnumerator<T> GetEnumerator()
        {
            //If the queue is empty, we have nothing to return
            if (this.Count is 0) yield break;

            //Clone the queue and pop everything
            PriorityQueue<T> s = new(this);
            while (!s.IsEmpty)
            {
                yield return s.Pop();
            }
        }

        /// <summary>
        /// Returns an IEnumerator from this PriorityQueue
        /// WARNING: Obtaining the first element of the iterator is O(n). Every subsequent elements is O(log n)
        /// </summary>
        /// <returns>Iterator going through this sequence</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
        
        #region Static methods
        /// <summary>
        /// Returns the index of the parent node
        /// </summary>
        /// <param name="i">Index of the child node</param>
        /// <returns>Index of the parent</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Parent(int i) => (i - 1) / 2;

        /// <summary>
        /// Returns the index of the left child node
        /// </summary>
        /// <param name="i">Index of the parent node</param>
        /// <returns>Index of the left child node</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeftChild(int i) => (2 * i) + 1;

        /// <summary>
        /// Returns the index of the right child node
        /// </summary>
        /// <param name="i">Index of the parent node</param>
        /// <returns>Index of the right child node</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RightChild(int i) => (2 * i) + 2;
        #endregion
    }
}