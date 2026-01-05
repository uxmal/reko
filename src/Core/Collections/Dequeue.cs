#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Defines the main interface to a dequeue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDequeue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Peeks at the front item of the dequeue without removing it.
        /// </summary>
        /// <returns>The front item.</returns>
        T PeekFront();

        /// <summary>
        /// Removes the front item from the dequeue and returns it.
        /// </summary>
        /// <returns>The front item.</returns>
        T PopFront();

        /// <summary>
        /// Adds an item to the front of the dequeue.
        /// </summary>
        /// <param name="item">Item to add.</param>
        void PushFront(T item);

        /// <summary>
        /// Peeks at the back item of the dequeue without removing it.
        /// </summary>
        /// <returns>The back item.</returns>
        T PeekBack();

        /// <summary>
        /// Removes the back item from the dequeue and returns it.
        /// </summary>
        /// <returns>The back item.
        /// </returns>
        T PopBack();

        /// <summary>
        /// Adds an item to the back of the dequeue.
        /// </summary>
        /// <param name="item">Item to add.</param>
        void PushBack(T item);

        /// <summary>
        /// Removes all items from the dequeue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the number of items in the dequeue.
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// Implements a dequeue as a circular array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dequeue<T> : IDequeue<T>
    {
        private T[] storage;
        private int iFront;
        private int iBack;

        /// <summary>
        /// Creates an empty dequeue.
        /// </summary>
        public Dequeue()
        {
            storage = new T[16];
        }

        /// <summary>
        /// Creates an empty dequeue with the specified capacity.
        /// </summary>
        /// <param name="capacity">Starting capacity of the dequeue.
        /// </param>
        public Dequeue(int capacity)
        {
            storage = new T[capacity];
        }

        #region IDequeue<T> Members

        /// <inheritdoc/>
        public T PeekFront()
        {
            EnsureNotEmpty();
            return storage[iFront];
        }

        /// <inheritdoc/>
        public T PopFront()
        {
            EnsureNotEmpty();
            T item = storage[iFront];
            storage[iFront] = default!;
            iFront = BoundIndex(iFront + 1);
            return item;
        }

        /// <inheritdoc/>
        public void PushFront(T item)
        {
            int iFrontNew = (storage.Length + (iFront - 1)) % storage.Length;
            if (iFrontNew == iBack)
                Grow();
            storage[iFrontNew] = item;
            iFront = iFrontNew;
        }

        /// <inheritdoc/>
        public T PeekBack()
        {
            EnsureNotEmpty();
            return storage[iBack - 1];
        }

        /// <inheritdoc/>
        public T PopBack()
        {
            EnsureNotEmpty();
            int iBackNew = BoundIndex(iBack - 1);
            T item = storage[iBackNew];
            storage[iBackNew] = default!;
            iBack = iBackNew;
            return item;
        }

        private int BoundIndex(int i)
        {
            return (storage.Length + i) % storage.Length;
        }

        private void EnsureNotEmpty()
        {
            if (iFront == iBack)
                throw new InvalidOperationException("Dequeue empty.");
        }

        /// <inheritdoc/>
        public void PushBack(T item)
        {
            int iBackNew = BoundIndex(iBack + 1);
            if (iBackNew == iFront)
            {
                Grow();
                iBackNew = iBack + 1;
            }
            storage[iBack] = item;
            iBack = iBackNew;
        }

        private void Grow()
        {
            var newStorage = new T[storage.Length * 2];
            int i = 0;
            foreach (T item in this)
            {
                newStorage[i++] = item;
            }
            storage = newStorage;
            iFront = 0;
            iBack = i;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Array.Clear(storage, 0, storage.Length);
            iFront = iBack = 0;
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                int c = iBack - iFront;
                if (c < 0)
                    c += storage.Length;
                return c;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = iFront; i != iBack; i = BoundIndex(i + 1))
            {
                yield return storage[i];
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
