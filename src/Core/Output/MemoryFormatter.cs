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

using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class renders data in the format expected in a memory dump.
    /// </summary>
    /// <remarks>
    /// A memory dump typically has the format:
    ///   AAAAAAAA BB BB BB BB... BB CCCCCCCCCCCCCCCC
    /// or 
    ///   AAAAAAAA BBBB BBBB... BBBB CCCCCCCCCCCCCCCC
    /// 
    /// where 'A' is the address, 'B' are the individual chunks displayed in 
    /// hexadecimal, and 'C' are the bytes interpreted as characters in some
    /// text encoding.
    /// </remarks>
    public class MemoryFormatter
    {
        private readonly PrimitiveType dtChunk;
        private readonly int chunksize;
        private readonly uint chunksPerLine;
        private readonly string chunkFormat;
        private readonly int unitsPerChunk;

        /// <summary>
        /// Create a formatter that will render data in chunks with the
        /// size of <paramref name="dtChunk"/>.
        /// </summary>
        /// <remarks>
        /// Note the distinction between address units and chunks here.
        /// Address units are the smallest addressable memory units of a 
        /// memory (typically bytes, but exceptions do exist). A chunk is 
        /// the term used for grouping one or more such addressable units for
        /// the purpose of rendering.
        /// </remarks>
        /// <param name="dtChunk">Datatype representing the size of the chunks
        /// to render.</param>
        /// <param name="chunksPerLine">Number of chunks per line. </param>
        /// <param name="unitsPerChunk">Number of units per chunk.</param>
        public MemoryFormatter(
            PrimitiveType dtChunk,
            int chunksPerLine,
            int unitsPerChunk)
        {
            this.dtChunk = dtChunk;
            this.chunksize = dtChunk.Size;
            this.chunksPerLine = (uint)chunksPerLine;
            this.chunkFormat = "{0:" + $"X{chunksize * 2}" + "}";
            this.unitsPerChunk = unitsPerChunk;
        }

        /// <summary>
        /// Render some memory, reading from the provided <see cref="ImageReader"/>, into
        /// a suitable output device <paramref name="output" />.
        /// </summary>
        /// <param name="rdr">Imagereader to read the data from.</param>
        /// <param name="enc">Text encoding used to render textual data.</param>
        /// <param name="output">Output device to which the rendered strings
        /// are emitted.</param>
        public void RenderMemory(EndianImageReader rdr, Encoding enc, IMemoryFormatterOutput output)
        {
            while (RenderLine(rdr, enc, output))
                ;
        }

        /// <summary>
        /// Render some memory, reading from the provided <see cref="ImageReader"/>, into
        /// a suitable output device <paramref name="output" />. This method
        /// takes into account that the image reader may not be positioned at 
        /// the beginning of a logical line; in that case it will render 
        /// filler space to ensure the display lines up.
        /// </summary>
        /// <param name="rdr">Imagereader to read the data from.</param>
        /// <param name="enc">Text encoding used to render textual data.</param>
        /// <param name="output">Output device to which the rendered strings
        /// are emitted.</param>
        public bool RenderLine(EndianImageReader rdr, Encoding enc, IMemoryFormatterOutput output)
        {
            Constant[] chunksRead = new Constant[chunksPerLine];
            output.BeginLine();
            var offStart = rdr.Offset;
            var addr = rdr.Address;
            output.RenderAddress(addr);

            var addrStart = Align(addr, chunksPerLine);
            var prePaddingUnits = (int) (addr - addrStart);
            var offsetEndLine = (rdr.Offset - prePaddingUnits) + chunksPerLine;

            output.RenderFillerSpan(PaddingCells(prePaddingUnits));

            bool moreData = true;
            int postPaddingUnits = 0;
            int iUnit = prePaddingUnits;
            while (moreData && rdr.Offset < offsetEndLine)
            {
                addr = rdr.Address;
                moreData = rdr.TryRead(dtChunk, out var c);
                if (moreData)
                {
                    DoRenderUnit(addr, c, output);
                    chunksRead[iUnit++] = c;
                }
                else
                {
                    postPaddingUnits = (int) (offsetEndLine - rdr.Offset);
                }
            }

            output.RenderFillerSpan(PaddingCells(postPaddingUnits));

            var offSaved = rdr.Offset;
            var cb = offSaved - offStart;
            rdr.Offset = offStart;
            var sBytes = DoRenderAsText(rdr, (int)cb, enc);
            rdr.Offset = offSaved;
            output.RenderUnitsAsText(prePaddingUnits * this.unitsPerChunk, sBytes, postPaddingUnits * this.unitsPerChunk);

            output.EndLine(chunksRead);

            return moreData && rdr.IsValid;
        }

        protected virtual void DoRenderUnit(Address addr, Expressions.Constant c, IMemoryFormatterOutput output)
        {
            output.RenderUnit(addr, string.Format(chunkFormat, c.GetValue()));
        }

        private int PaddingCells(int addressDifference)
        {
            var bytes = (addressDifference * unitsPerChunk);
            return (1 + 2 * chunksize) * (bytes / chunksize);
        }

        protected virtual string DoRenderAsText(
            ImageReader rdr, 
            int cUnits,
            Encoding enc)
        {
            var bytes = rdr.ReadBytes(cUnits);
            var chars = enc.GetChars(bytes);
            for (int i = 0; i < chars.Length; ++i)
            {
                char ch = chars[i];
                if (char.IsControl(ch) || char.IsSurrogate(ch) || (0xE000 <= ch && ch <= 0xE0FF))
                    chars[i] = '.';
            }
            return new string(chars);
        }

        private static ulong Align(ulong ul, uint alignment)
        {
            return alignment * (ul / alignment);
        }

        private static Address Align(Address addr, uint alignment)
        {
            var lin = addr.ToLinear();
            var linAl = Align(lin, alignment);
            return addr - (int) (lin - linAl);
        }
    }
}
