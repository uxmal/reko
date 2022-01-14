#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Core.Output
{
    public interface IMemoryFormatterOutput
    {
        void BeginLine();
        void RenderAddress(Address addr);
        void RenderFillerSpan(int nCells);

        /// <summary>
        /// Render a memory unit corresponding to the address <paramref name="addr"/>.
        /// <param name="addr"></param>
        /// <param name="sUnit"></param>
        void RenderUnit(Address addr, string sUnit);

        /// <summary>
        /// Attempt to render the units as text, with a margin first.
        /// </summary>
        /// <param name="sBytes"></param>
        void RenderUnitsAsText(int prePadding, string sBytes, int postPadding);

        void EndLine(byte[] bytes);
    }
}
