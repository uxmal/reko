#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using System;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Provides a text model that use to show code of procedure.
    /// </summary>
    public class ProcedureCodeModel : TextViewModel
    {
        private Procedure proc;
        int position;
        private TextSpan[][] lines;
        private int numLines;

        public ProcedureCodeModel(Procedure proc)
        {
            this.proc = proc;
            this.numLines = CountLines();
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
            EnsureLines();
            int p = (int)position;
            int c = Math.Min(count, LineCount - p);
            if (c <= 0)
                return new LineSpan[0];
            var spans = new LineSpan[c];
            for (int i = 0; i < c; ++i)
            {
                TextSpan[] line;
                if ((p + i) < lines.Length)
                    line = lines[p + i];
                else
                    line = new TextSpan[] { new EmptyTextSpan() };
                spans[i] = new LineSpan(p + i, line);
            }
            position = p + c;
            return spans;
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            return Tuple.Create(position, LineCount);
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

        private void EnsureLines()
        {
            if (lines != null)
                return;
            var tsf = new TextSpanFormatter();
            WriteCode(tsf);
            lines = tsf.GetLines();
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

        private class EmptyTextSpan : TextSpan
        {
            public override string GetText()
            {
                return "";
            }
        }
    }
}
