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

using Reko.Core.Expressions;

namespace Reko.Core.Output
{
    /// <summary>
    /// Interface implemented for output devices used in conjunction with
    /// the <see cref="MemoryFormatter"/>.
    /// </summary>
    public interface IMemoryFormatterOutput
    {
        /// <summary>
        /// Signals the beginning of a line to be rendered.
        /// </summary>
        void BeginLine();

        /// <summary>
        /// Requests an address to be rendered.
        /// </summary>
        /// <param name="addr"></param>
        void RenderAddress(Address addr);

        /// <summary>
        /// Requests blank space corresponding to <paramref name="nChunks"/> 
        /// chunks, each consisting of <paramref name="nCharsPerChunk"/> 
        /// letters or digits.
        /// </summary>
        /// <param name="nCells"></param>
        void RenderFillerSpan(int nChunks, int nCharsPerChunk);

        /// <summary>
        /// Render a memory unit corresponding to the address <paramref name="addr"/>.
        /// <param name="addr">The address of the memory unit.</param>
        /// <param name="sUnit">The numeric representation of the memory unit.</param>
        void RenderUnit(Address addr, string sUnit);

        /// <summary>
        /// Render the memory unit as text.
        /// </summary>
        /// <param name="addr">The address of the memory unit.</param>
        /// <param name="sUnit">The text representation of the unit.</param>
        void RenderUnitAsText(Address addr, string sUnit);

        /// <summary>
        /// Render the padding of the memory textual area.
        /// </summary>
        /// <param name="padding">Number of character positions to pad with.</param>
        void RenderTextFillerSpan(int padding);

        /// <summary>
        /// Signals the end of the line.
        /// </summary>
        /// <param name="units">The units rendered in this line.</param>
        void EndLine(Constant[] units);
    }
}
