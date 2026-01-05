#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Types;

namespace Reko.Core.Output
{
	/// <summary>
	/// Base class for all formatting classes. 
	/// </summary>
	public abstract class Formatter
	{
        /// <summary>
        /// Initializes the formatter with default values.
        /// </summary>
		public Formatter()
		{
			this.UseTabs = true;
			this.TabSize = 4;
			this.Indentation = 4;
		}

        /// <summary>
        /// Indents the current line to the level specified by the <see cref="Indentation"/>
        /// property.
        /// </summary>
		public virtual void Indent()
		{
			int n = Indentation;
			while (n >= TabSize)
			{
				if (UseTabs)
				{
					Write("\t");
				}
				else
				{
					WriteSpaces(TabSize);
				}
				n -= TabSize;
			}
			WriteSpaces(n);
		}

        /// <summary>
        /// Current indentation column.
        /// </summary>
		public int Indentation { get; set; }

        /// <summary>
        /// Defines the size of one indentation step.
        /// </summary>
		public int TabSize {get; set; }

        /// <summary>
        /// If true, indentation is formatted using tab characters (U+0009).
        /// If false, indentation is formatted using space characters (U+0020).
        /// </summary>
        public bool UseTabs { get; set; }

        /// <summary>
        /// Begin a new line.
        /// </summary>
        /// <param name="tag">Optional line-specific data object.</param>
        public abstract void Begin(object? tag);

        /// <summary>
        /// Terminate a line using the terminator string.
        /// </summary>
        public abstract void Terminate();

        /// <summary>
        /// Write the string <paramref name="s"/>, then terminate the line.
        /// </summary>
        /// <param name="s"></param>
		public void Terminate(string s)
		{
			Write(s);
            Terminate();
		}

        /// <summary>
        /// Write the string <paramref name="s"/> with no special formatting.
        /// </summary>
        /// <param name="s"></param>
        public abstract void Write(string s);

        /// <summary>
        /// Write the character <paramref name="ch"/> with no special formatting.
        /// </summary>
        /// <param name="ch"></param>
        public abstract Formatter Write(char ch);

        /// <summary>
        /// Writes a string using a format string. 
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="arguments">Arguments to use in the format string.</param>
        public abstract void Write(string format, params object[] arguments);

        /// <summary>
        /// Writes a string using a format string, terminating it with a new line.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="arguments">Arguments to use in the format string.</param>
        public abstract void WriteLine(string format, params object[] arguments);

        /// <summary>
        /// Writes a line comment.
        /// </summary>
        /// <param name="comment">Comment to write.</param>
        public abstract void WriteComment(string comment);

        /// <summary>
        /// Writes a hyperlink.
        /// </summary>
        /// <param name="text">Text of the hyperlink.</param>
        /// <param name="href">Linked object.</param>
        public abstract void WriteHyperlink(string text, object href);

        /// <summary>
        /// Writes a keyword with the appropriate styling.
        /// </summary>
        /// <param name="keyword">Keyword to write.</param>
        public abstract void WriteKeyword(string keyword);

        /// <summary>
        /// Writes a data type.
        /// </summary>
        /// <param name="typeName">String rendering of the data type.</param>
        /// <param name="dt">Data type.</param>
        public abstract void WriteType(string typeName, DataType dt);

        /// <summary>
        /// Writes a newline.
        /// </summary>
        public abstract void WriteLine();

        /// <summary>
        /// Writes a label string.
        /// </summary>
        /// <param name="label">Label string.</param>
        /// <param name="block">Object associated with the label.</param>
        public abstract void WriteLabel(string label, object block);

        /// <summary>
        /// Writes a string, followed by a newline.
        /// </summary>
        /// <param name="s">String to write.</param>
        public abstract void WriteLine(string s);

        /// <summary>
        /// Writes an object.
        /// </summary>
        /// <param name="o">Object to write.</param>
        public void Write(object? o)
        {
            if (o is { })
                Write(o.ToString()!);
        }

        /// <summary>
        /// Writes an object followed by a newline.
        /// </summary>
        /// <param name="o">Object to write.</param>
        public void WriteLine(object? o)
        {
            if (o is { })
                Write(o);
            WriteLine();
        }

        /// <summary>
        /// Writes <paramref name="n"/> spaces.
        /// </summary>
        /// <param name="n">Number of spaces to write.</param>
		public void WriteSpaces(int n)
		{
			while (n > 0)
			{
				Write(" ");
				--n;
			}
		}
    }

    /// <summary>
    /// No-op formatter.
    /// </summary>
    public class NullFormatter : Formatter
    {
        /// <inheritdoc/>
        public override void Begin(object? tag)
        {
        }

        /// <inheritdoc/>
        public override void Terminate()
        {
        }

        /// <inheritdoc/>
        public override Formatter Write(char ch)
        {
            return this;
        }

        /// <inheritdoc/>
        public override void Write(string s)
        {
        }

        /// <inheritdoc/>
        public override void Write(string format, params object[] arguments)
        {
        }

        /// <inheritdoc/>
        public override void WriteComment(string comment)
        {
        }

        /// <inheritdoc/>
        public override void WriteHyperlink(string text, object href)
        {
        }

        /// <inheritdoc/>
        public override void WriteKeyword(string keyword)
        {
        }

        /// <inheritdoc/>
        public override void WriteLabel(string label, object block)
        {
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
        }

        /// <inheritdoc/>
        public override void WriteLine(string s)
        {
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, params object[] arguments)
        {
        }

        /// <inheritdoc/>
        public override void WriteType(string typeName, DataType dt)
        {
        }
    }
}
