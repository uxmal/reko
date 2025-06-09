#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Opaque pointer into the text view.
    /// </summary>
    public class TextPointer
    {
        internal object Line;
        internal int Span;
        internal int Character;

        public TextPointer(object line, int span, int ch)
        {
            Debug.Assert(line is not null);
            this.Line = line;
            this.Span = span;
            this.Character = ch;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Line, Span, Character);
        }
    }
}
