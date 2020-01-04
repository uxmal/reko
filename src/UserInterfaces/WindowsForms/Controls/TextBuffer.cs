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
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class TextBuffer<T>
    {
        private T[] elements;
        private int iGapStart;
        private int iGapEnd;
        private int iLast;

        public TextBuffer() : this(new T[4096])
        {
            iGapStart = 0;
            iGapEnd = elements.Length;
        }

        public TextBuffer(T[] elements)
        {
            iGapStart = 0;
            iGapEnd = elements.Length;
            iLast = elements.Length;
            this.elements = elements;
        }

        public int Count
        {
            get { return iGapStart + (elements.Length - iGapEnd); }
        }

        public void Insert(T elem)
        {
            elements[iGapStart++] = elem;
        }
    }
}
