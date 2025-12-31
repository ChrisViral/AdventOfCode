using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Linq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Collections;

/// <summary>
/// Collection extension methods
/// </summary>
[PublicAPI]
public static class CollectionExtensions
{
    /// <typeparam name="T">Type of element in the collection</typeparam>
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// Checks if a collection is empty
        /// </summary>
        /// <value>True if the collection is empty, false otherwise</value>
        public bool IsEmpty => collection.Count is 0;

        /// <summary>
        /// Adds a set of values to the collection
        /// </summary>
        /// <param name="values">Values to add</param>
        public void AddRange([InstantHandle] IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                collection.Add(value);
            }
        }
    }

    /// <typeparam name="T">Type of element in the list</typeparam>
    extension<T>(IList<T> list)
    {
        /// <summary>
        /// Applies the given function to all members of the array
        /// </summary>
        /// <param name="modification">Modification function</param>
        public void Apply([InstantHandle] Func<T, T> modification)
        {
            foreach (int i in ..list.Count)
            {
                list[i] = modification(list[i]);
            }
        }

        /// <summary>
        /// Enumerate pairs of items in the given list
        /// </summary>
        /// <returns>An exhaustive list of all item pairs in <paramref name="list"/></returns>
        public IEnumerable<(T, T)> EnumeratePairs()
        {
            if (list.Count <= 1) yield break;

            int end = list.Count - 1;
            for (int i = 0; i < end; i++)
            {
                T first = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    yield return (first, list[j]);
                }
            }
        }

        /// <summary>
        /// Removes an element from the list by swapping the last element of the list in it's spot, and then removing the last element.<br/>
        /// This should technically run in O(1)
        /// </summary>
        /// <param name="index">Index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of the range of the list</exception>
        public void RemoveSwap(int index)
        {
            if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of list");

            int lastIndex = list.Count - 1;
            if (index != lastIndex)
            {
                // Move the last element to the element to remove's spot
                list[index] = list[lastIndex];
            }
            // Remove last element
            list.RemoveAt(lastIndex);
        }
    }

    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        /// <summary>
        /// Converts a generic dictionary to a ValueEnumerable of it's KeyValue pairs
        /// </summary>
        /// <returns>Value enumerable of KeyValue pairs</returns>
        public ValueEnumerable<FromEnumerable<KeyValuePair<TKey, TValue>>, KeyValuePair<TKey, TValue>> AsValueEnumerable()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).AsValueEnumerable();
        }
    }

    /// <typeparam name="T">Type of element in the stack</typeparam>
    extension<T>(Stack<T> stack)
    {
        /// <summary>
        /// Checks if a stack is empty
        /// </summary>
        /// <value>True if the stack is empty, false otherwise</value>
        public bool IsEmpty => stack.Count is 0;

        /// <summary>
        /// Creates a copy of the given stack, preserving order
        /// </summary>
        /// <returns>A shallow copy of the stack</returns>
        public Stack<T> CreateCopy()
        {
            T[] array = new T[stack.Count];
            stack.CopyTo(array, 0);
            return new Stack<T>(array.Reversed());
        }
    }

    /// <typeparam name="T">Type of element in the queue</typeparam>
    extension<T>(Queue<T> stack)
    {
        /// <summary>
        /// Checks if a queue is empty
        /// </summary>
        /// <value>True if the queue is empty, false otherwise</value>
        public bool IsEmpty => stack.Count is 0;
    }

    /// <typeparam name="T">Type of element in the list node</typeparam>
    extension<T>(LinkedListNode<T> node)
    {
        /// <summary>
        /// Returns the next node in a linked list in a circular fashion, wrapping back to the start after getting to the end
        /// </summary>
        /// <returns>The next node in the list, or the first one if at the end</returns>
        public LinkedListNode<T> NextCircular() => node.Next ?? node.List!.First!;

        /// <summary>
        /// Returns the previous node in a linked list in a circular fashion, wrapping back to the end after getting to the start
        /// </summary>
        /// <returns>The previous node in the list, or the last one if at the start</returns>
        public LinkedListNode<T> PreviousCircular() => node.Previous ?? node.List!.Last!;
    }

    /// <typeparam name="T">Type of element in the LinkedList</typeparam>
    extension<T>(LinkedList<T> list)
    {
        /// <summary>
        /// Creates an array containing all the nodes of the LinkedList
        /// </summary>
        /// <returns>An array containing all the <see cref="LinkedListNode{T}"/> contained within <paramref name="list"/></returns>
        public LinkedListNode<T>[] ToNodeArray()
        {
            LinkedListNode<T>[] nodes = new LinkedListNode<T>[list.Count];
            LinkedListNode<T> current = list.First!;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = current;
                current  = current.Next!;
            }

            return nodes;
        }
    }
}
