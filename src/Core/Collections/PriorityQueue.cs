#region License
/* Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Implementation of the priority queue ADT.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : ICollection<T>
    {
        private HeapItem[] heap;
        private int count;

        private struct HeapItem
        {
            public int Priority;
            public T Value;
        }

        /// <summary>
        /// Creates an empty priority queue.
        /// </summary>
        public PriorityQueue()
        {
            heap = new HeapItem[4];
        }

        private void GrowHeap()
        {
            HeapItem [] newHeap = new HeapItem[heap.Length * 2 + 1];
            heap.CopyTo(newHeap, 0);
            heap = newHeap;
        }

        private void BubbleUp(int idx, HeapItem item)
        {
            int iParent = (idx - 1) / 2;
            while (idx > 0 && heap[iParent].Priority < item.Priority)
            {
                heap[idx] = heap[iParent];
                idx = iParent;
                iParent = (idx - 1) / 2;
            }
            heap[idx] = item;
        }

        private void TrickleDown(int idx, HeapItem item)
        {
            int iChild = idx * 2 + 1;
            while (iChild < count)
            {
                if (iChild+1 < count && 
                    heap[iChild].Priority < heap[iChild+1].Priority)
                {
                    ++iChild;
                }
                heap[idx] = heap[iChild];
                idx = iChild;
                iChild  =idx * 2 + 1;
            }
            BubbleUp(idx, item);
        }

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Add an item with a given priority to the queue.
        /// </summary>
        /// <param name="priority">The priority of the item.</param>
        /// <param name="value">The item to add to the queue.</param>
        public void Enqueue(int priority, T value)
        {
            ++count;
            if (count >= heap.Length)
                GrowHeap();
            BubbleUp(count - 1, new HeapItem { Priority = priority, Value = value });
        }

        /// <summary>
        /// Dequeues the item with the highest priority, removing it from
        /// the priority queue.
        /// </summary>
        /// <returns>The item with the highest priority.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Dequeue()
        {
            if (count <= 0)
                throw new InvalidOperationException("Priority queue is empty.");
            T value = heap[0].Value;
            --count;
            TrickleDown(0, heap[count]);
            return value;
        }

        /// <summary>
        /// Attempts to dequeue the item with the highest priority, removing it from
        /// the priority queue.
        /// </summary>
        /// <param name="value">The item with the highest priority (if there is one in
        /// the priority queue).</param>
        /// <returns>True if an item was dequeued; otherwise false.</returns>
        public bool TryDequeue([MaybeNullWhen(false)] out T value)
        {
            if (count <= 0)
            {
                value = default;
                return false;
            }
            value = heap[0].Value;
            --count;
            TrickleDown(0, heap[count]);
            return true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            count = 0;
            heap = new HeapItem[4];
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            for (int i = 0; i < count; ++i)
            {
                if (heap[i].Value!.Equals(item))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; ++i, ++arrayIndex)
            {
                array[arrayIndex] = heap[i].Value;
            }
        }

        /// <inheritdoc/>
        public int Count => count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; ++i)
            {
                yield return heap[i].Value;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
