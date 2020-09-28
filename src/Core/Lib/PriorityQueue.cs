#region License
/* Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core.Lib
{
    /// <summary>
    /// Implementation of the priority queue ADT.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : ICollection<T>
    {
        HeapItem[] heap;
        int count;

        public struct HeapItem
        {
            public int Priority;
            public T Value;
        }

        public PriorityQueue()
        {
            Clear();
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

        public void Enqueue(int priority, T value)
        {
            ++count;
            if (count >= heap.Length)
                GrowHeap();
            BubbleUp(count - 1, new HeapItem { Priority = priority, Value = value });
        }

        public T Dequeue()
        {
            if (count <= 0)
                throw new InvalidOperationException("Priority queue is empty.");
            T value = heap[0].Value;
            --count;
            TrickleDown(0, heap[count]);
            return value;
        }

        public void Clear()
        {
            count = 0;
            heap = new HeapItem[4];
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; ++i)
            {
                if (heap[i].Value.Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; ++i, ++arrayIndex)
            {
                array[arrayIndex] = heap[i].Value;
            }
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

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
