#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core.Lib
{
    /// <summary>
    /// Defines the main interface to a dequeue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDequeue<T> : IEnumerable<T>
    {
        T PeekFront();
        T PopFront();
        void PushFront(T item);
        T PeekBack();
        T PopBack();
        void PushBack(T item);
        void Clear();
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

        public Dequeue()
        {
            storage = new T[16];
        }

        public Dequeue(int capacity)
        {
            storage = new T[capacity];
        }

        #region IDequeue<T> Members

        public T PeekFront()
        {
            EnsureNotEmpty();
            return storage[iFront];
        }

        public T PopFront()
        {
            EnsureNotEmpty();
            T item = storage[iFront];
            storage[iFront] = default(T);
            iFront = BoundIndex(iFront + 1);
            return item;
        }

        public void PushFront(T item)
        {
            int iFrontNew = (storage.Length + (iFront - 1)) % storage.Length;
            if (iFrontNew == iBack)
                Grow();
            storage[iFrontNew] = item;
            iFront = iFrontNew;
        }

        public T PeekBack()
        {
            EnsureNotEmpty();
            return storage[iBack - 1];
        }

        public T PopBack()
        {
            EnsureNotEmpty();
            int iBackNew = BoundIndex(iBack - 1);
            T item = storage[iBackNew];
            storage[iBackNew] = default(T);
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

        public void Clear()
        {
            Array.Clear(storage, 0, storage.Length);
            iFront = iBack = 0;
        }

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
