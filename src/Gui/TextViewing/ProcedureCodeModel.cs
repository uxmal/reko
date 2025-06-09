#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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

using Reko.Core;
using Reko.Core.Output;
using Reko.Gui.Services;
using System;
using System.Diagnostics;

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// Provides a text model that use to show code of procedure.
    /// </summary>
    public class ProcedureCodeModel : ITextViewModel
    {
        private readonly Procedure proc;
        protected readonly TextSpanFactory factory;
        private int position;
        private LineSpan[]? lines;      // The procedure, rendered into line spans
        private readonly int numLines;
        private readonly ISelectedAddressService selSvc;

        public ProcedureCodeModel(Procedure proc, TextSpanFactory factory, ISelectedAddressService selSvc)
        {
            this.proc = proc;
            this.factory = factory;
            this.numLines = CountLines();
            this.selSvc = selSvc;
        }

        public object CurrentPosition { get { return position; } }

        public object EndPosition { get { return LineCount; } }

        public int LineCount { get { return numLines + NumEmptyLinesAfter; } }

        public object StartPosition { get { return 0; } }

        public int NumEmptyLinesAfter { get; set; }

        public int ComparePositions(object a, object b)
        {
            return ((int)a).CompareTo((int)b);
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var lines = EnsureLines();
            int p = (int)position;
            int c = Math.Min(count, LineCount - p);
            if (c <= 0)
                return Array.Empty<LineSpan>();
            var spans = new LineSpan[c];
            for (int i = 0; i < c; ++i)
            {
                LineSpan line;
                if ((p + i) < lines.Length)
                {
                    line = lines[p + i];
                    if (line.Tag is ulong uAddr && 
                        this.selSvc.SelectedAddressRange is not null &&
                        uAddr == this.selSvc.SelectedAddressRange.Address.ToLinear())
                    {
                        line.Style = "mirrored";
                    }
                }
                else
                    line = new LineSpan(p + i, null, new ITextSpan[] { factory.CreateEmptyTextSpan() });
                Debug.Assert((int) line.Position == p + i);
                spans[i] = line;
            }
            position = p + c;
            return spans;
        }

        public (int, int) GetPositionAsFraction()
        {
            return (position, LineCount);
        }

        public int MoveToLine(object position, int offset)
        {
            int orig = (int)position;
            this.position = orig + offset;
            if (this.position < 0)
                this.position = 0;
            if (this.position >= LineCount)
                this.position = LineCount;
            return this.position - orig;
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
            position = (int)(Math.BigMul(numer, LineCount) / denom);
        }

        private LineSpan[] EnsureLines()
        {
            if (lines is not null)
                return lines;
            var tsf = new TextSpanFormatter(factory);
            WriteCode(tsf);
            this.lines = tsf.GetLines();
            return this.lines;
        }

        private int CountLines()
        {
            var lineCounter = new LineCounterFormatter();
            WriteCode(lineCounter);
            return lineCounter.NumLines;
        }

        private void WriteCode(Formatter tsf)
        {
            var fmt = new AbsynCodeFormatter(tsf);
            fmt.InnerFormatter.UseTabs = false;
            fmt.Write(proc);
        }

        private class LineCounterFormatter : NullFormatter
        {
            public int NumLines { get; private set; }

            public override void Terminate()
            {
                NumLines++;
            }

            public override void WriteLine()
            {
                NumLines++;
            }

            public override void WriteLine(string s)
            {
                NumLines++;
            }

            public override void WriteLine(string format, params object[] arguments)
            {
                NumLines++;
            }
        }
    }
}
