#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Threading.Tasks;

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// A line span corresponds to a line of text. A line 
    /// of text consists of multiple text spans.
    /// </summary>
    public struct LineSpan
    {
        public readonly object Position;
        public readonly ITextSpan[] TextSpans;
        public readonly object Tag;
        public string Style;

        public LineSpan(object position, object tag, params ITextSpan[] textSpans)
        {
            this.Position = position;
            this.TextSpans = textSpans;
            this.Tag = tag;
            this.Style = default!;
        }
    }
}
