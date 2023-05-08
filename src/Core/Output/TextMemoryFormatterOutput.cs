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
using System.Text;

namespace Reko.Core.Output
{
    public class TextMemoryFormatterOutput : IMemoryFormatterOutput
    {
        private readonly Formatter stm;
        private readonly StringBuilder sb = new StringBuilder(0x12);
        private readonly StringBuilder sbHex = new StringBuilder();
        private Constant[]? prevLine = null;
        private bool showEllipsis = true;

        public TextMemoryFormatterOutput(Formatter stm)
        {
            this.stm = stm;
        }

        public void BeginLine()
        {
        }

        public void RenderAddress(Address addr)
        {
            sbHex.AppendFormat("{0}", addr);
        }

        public void RenderFillerSpan(int nChunks, int nCellsPerChunk)
        {
            // Extra 1 cell for padding between chunks.
            int nCells = (1 + nCellsPerChunk) * nChunks;
            for (int i = 0; i < nCells; ++i)
                sbHex.Append(' ');
        }

        public void RenderUnit(Address addr, string sUnit)
        {
            sbHex.Append(' ');
            sbHex.Append(sUnit);
        }

        public void RenderUnitAsText(Address addr, string sUnit)
        {
            sb.Append(sUnit);
        }

        public void RenderTextFillerSpan(int padding)
        {
            sb.Append(' ', padding);
        }

        public void EndLine(Constant[] chunks)
        {
            if (!HaveSameZeroBytes(prevLine, chunks))
            {
                stm.Write(sbHex.ToString());
                stm.Write(' ');
                stm.WriteLine(sb.ToString());
                showEllipsis = true;
            }
            else
            {
                if (showEllipsis)
                {
                    stm.WriteLine("; ...");
                    showEllipsis = false;
                }
            }
            prevLine = chunks;
            sbHex.Clear();
            sb.Clear();
        }

        private bool HaveSameZeroBytes(Constant[]? prevLine, Constant[] line)
        {
            if (prevLine == null)
                return false;
            if (prevLine.Length != line.Length)
                return false;
            for (int i = 0; i < line.Length; ++i)
            {
                var prev = prevLine[i];
                var cur = line[i];
                if (prev is null || cur is null)
                    return false;
                if (prev.ToUInt64() != cur.ToUInt64() || cur.ToUInt64() != 0)
                    return false;
            }
            return true;
        }
    }
}
