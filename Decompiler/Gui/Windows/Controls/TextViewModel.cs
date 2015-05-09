#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Drawing;

namespace Decompiler.Gui.Windows.Controls
{
    /// <summary>
    /// Describes a source of textual data as a list of lines,
    /// where each line is 0 or more TextSpans.
    /// </summary>
    /// <remarks>
    /// This model has the advantage that it can be used for texts that are defined
    /// vaguely or inconsistently; notably a disassembly of a machine code where instructions
    /// can span differing number of bytes (e.g. x86, M68k). The calculation of an exact 
    /// line count is expensive in such circumstances, so we opt for an estimate.
    /// </remarks>
    public interface TextViewModel
    {
        object CurrentPosition { get; }

        object StartPosition { get; }

        object EndPosition { get; }

        /// <summary>
        /// Estimated number of lines in the model.
        /// </summary>
        int LineCount { get; } 

        /// <summary>
        /// Move the current position relative to the parameter <paramref name="position"/>, offset
        /// by <paramref name="offset"/> lines
        /// </summary>
        /// <remarks>
        /// If the position would have overshot either the beginning or the end of the lines, clamp the
        /// position to Beginning or End.
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        void MoveTo(object position, int offset);

        /// <summary>
        /// Read <paramref name="count"/> lines, starting at the current position.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Array of arrays of text spans.</returns>
        TextSpan[][] GetLineSpans(int count);

        /// <summary>
        /// Returns the current position as a fraction.
        /// </summary>
        /// <returns></returns>
        Tuple<int, int> GetPositionAsFraction();

        /// <summary>
        /// Sets the current position to approximately numer / denom.
        /// </summary>
        /// <param name="numer"></param>
        /// <param name="denom"></param>
        void SetPositionAsFraction(int numer, int denom);
    }

    /// <summary>
    /// A null object to use when there is no Editor.
    /// </summary>
    public class EmptyEditorModel : TextViewModel
    {
        public object CurrentPosition { get { return this; } }
        public object StartPosition { get { return this; } }
        public object EndPosition{ get { return this; } }
        public int LineCount { get { return 0; } }

        public void MoveTo(object position, int offset)
        {
        }

        public TextSpan[][] GetLineSpans(int count)
        {
            return new TextSpan[0][];
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            return Tuple.Create(0, 1);
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
        }
    }

    /// <summary>
    /// A TextSpan describes a span of text that has the same
    /// formatting attributes and behaviour.
    /// </summary>
    public abstract class TextSpan
    {
        private static StringFormat stringFormat;

        static TextSpan()
        {
            stringFormat = new StringFormat(StringFormat.GenericTypographic);
            stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        }

        public abstract string GetText();
        public string Style { get; set; }
        public object Tag { get; set; }
        public int ContextMenuID { get; set; }

        public virtual SizeF GetSize(string text, Font font, Graphics g)
        {
            return g.MeasureString(text, font, 0, stringFormat);
        }
    }
}
