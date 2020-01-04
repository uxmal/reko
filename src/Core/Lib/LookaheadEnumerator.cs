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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Core.Lib
{
    /// <summary>
    /// An extension of IEnumerator&lt;T&gt; that wraps an IEnumerator and 
    /// which lets the caller peek ahead in the underlying enumeration.
    /// </summary>
    public class LookaheadEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> e;
        private List<T> peeked;
        private int iCur;

        public LookaheadEnumerator(IEnumerator<T> innerEnumerator)
        {
            if (innerEnumerator == null)
                throw new ArgumentNullException("innerEnumerator");
            this.e = innerEnumerator;
            this.peeked = new List<T>();
            this.iCur = 0;
        }

        public LookaheadEnumerator(IEnumerable<T> collection) : this(collection.GetEnumerator())
        {
        }

        #region IEnumerator<T> Members

        public T Current
        {
            get
            {
                return Peek(0);
            }
        }

        public void Dispose()
        {
            e.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (iCur < peeked.Count-1)
            {
                ++iCur;
                return true;
            }
            iCur = 0;
            peeked.Clear();
            return e.MoveNext();
        }

        public void Reset()
        {
            iCur = 0;
            peeked.Clear();
            e.Reset();
        }

        #endregion

        public T Peek(int ahead)
        {
            int itemsInBuffer = peeked.Count - iCur;
            Debug.Assert(itemsInBuffer >= 0);
            if (itemsInBuffer == 0 && ahead == 0)
            {
                return e.Current;
            }
            if (ahead < itemsInBuffer)
            {
                return peeked[iCur + ahead];
            }
            if (itemsInBuffer == 0)
                peeked.Add(e.Current);
            for (int i = 0; i < ahead; ++i)
            {
                e.MoveNext();
                peeked.Add(e.Current);
            }
            return peeked[ahead];
        }

        public void Skip(int skip)
        {
            int itemsInBuffer = peeked.Count - iCur;
            if (skip < itemsInBuffer)
            {
                iCur += skip;
                return;
            }
        }
    }
}
