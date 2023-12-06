using System;

namespace AdventOfCode.Utils
{
    /// <summary>
    /// Thread-safe object reference
    /// </summary>
    /// <typeparam name="T">Reference type</typeparam>
    public class Safe<T>
    {
        private readonly object locker = new();

        private T? value;
        /// <summary>
        /// Thread-safe access to the stored value.
        /// </summary>
        /// <remarks>
        /// <b>CAREFUL</b>: Once the value is returned, the lock is released.
        /// Updates to this value which require knowing the value beforehand
        /// should be made using <see cref="Update(Func{T, T})"/>
        /// </remarks>
        public T? Value
        {
            get
            {
                lock (locker)
                {
                    return this.value;
                }
            }
            set
            {
                lock (locker)
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Creates a new default value safe reference
        /// </summary>
        public Safe() => this.value = default;

        /// <summary>
        /// Creates a new safe reference of the specified value
        /// </summary>
        /// <param name="value">Initial value</param>
        public Safe(T? value) => this.value = value;

        /// <summary>
        /// Allows updating the stored value without the risk of another object grabbing the lock.
        /// </summary>
        /// <param name="updater"></param>
        public void Update(Func<T?, T?> updater)
        {
            lock (locker)
            {
                this.value = updater(this.value);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            lock (locker)
            {
                return this.value?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Extracts the value from the safe reference
        /// </summary>
        /// <param name="safeValue">Safe reference to get the value from</param>
        public static implicit operator T?(Safe<T?> safeValue)
        {
            lock (safeValue.locker)
            {
                return safeValue.value;
            }
        }
    }
}