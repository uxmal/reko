#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.Core.Hll.C
{
    public class LookAheadLexer
    {
        private readonly StringConcatenator sc;
        private readonly List<CToken> queue;
        private int iRead;

        public LookAheadLexer(StringConcatenator sc)
        {
            this.sc = sc;
            this.queue = new List<CToken>();
            iRead= 0;
        }

        public int LineNumber => sc.LineNumber;

        public CToken Read()
        {
            if (iRead < queue.Count)
            {
                var token = queue[iRead++];
                if (iRead == queue.Count)
                {
                    queue.Clear();
                    iRead = 0;
                }
                return token;
            }
            return sc.Read();
        }

        public CToken Peek(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Can only peek ahead in the stream of tokens.");
            int i = iRead + n;
            while (IsSlotEmpty(i))
            {
                queue.Add(sc.Read());
            }
            return queue[i];
        }

        private bool IsSlotEmpty(int n)
        {
            return queue.Count <= n;
        }
    }
}
