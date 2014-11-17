#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
    public interface TextViewModel
    {
        /// <summary>
        /// When this event fires, the listener should update.
        /// </summary>
        event EventHandler ModelChanged;

        /// <summary>
        /// The total number of lines of the model.
        /// </summary>
        int LineCount { get; }

        /// <summary>
        /// Retrieves Line spans starting at the CurrentLine.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEnumerable<TextSpan> GetLineSpans(int index);

        /// <summary>
        /// Gives the view model a hint that the <paramref name="count"/> items starting
        /// at <paramref name="index"/> will soon be fetched.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        void CacheHint(int index, int count);
    }

    public interface TextViewModel2
    {
        object CurrentPosition { get; }

        object StartPosition { get; }

        object EndPosition { get; }

    /// <summary>
        /// Estimated number of lines in the model.
        /// </summary>
        int LineCount { get; } 

        /// <summary>
        /// Move the current position relative to the parameter <paramref name="position"/>, offet
        /// by <paramref name="offset"/> lines
        /// </summary>
        /// If the position would have overshot either the beginning or the end of the lines, clamp the
        /// position to Beginning or End.
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
        public event EventHandler ModelChanged;

        public int LineCount { get { return 0; } }

        public IEnumerable<TextSpan> GetLineSpans(int index)
        {
            yield break;
        }

        public void CacheHint(int index, int count)
        {
        }
    }

    /// <summary>
    /// An TextSpan describes a span of text that has the same
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

        public virtual SizeF GetSize(string text, Font font, Graphics g)
        {
            return g.MeasureString(text, font, 0, stringFormat);
        }
    }
}
