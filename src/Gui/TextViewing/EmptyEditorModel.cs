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

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// A null object to use when there is no Editor.
    /// </summary>
    /// 
    public class EmptyEditorModel : ITextViewModel
    {
        public object CurrentPosition => this;
        public object StartPosition => this;
        public object EndPosition => this;
        public int LineCount => 0;

        public EmptyEditorModel()
        {
        }

        public int MoveToLine(object position, int offset)
        {
            return 0;
        }

        public int ComparePositions(object a, object b)
        {
            return 0;
        }

        public LineSpan[] GetLineSpans(int count)
        {
            return Array.Empty<LineSpan>();
        }

        public (int, int) GetPositionAsFraction()
        {
            return (0, 1);
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
        }
    }
}