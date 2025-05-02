#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.IO;

namespace Reko.Core.Output
{
    /// <summary>
    /// A TextWriter that maintains a notion of indentation.
    /// </summary>
	public class IndentingTextWriter
	{
		private readonly TextWriter writer;
		private readonly bool useTabs;
		private readonly int tabWidth;
		private int tabStops;
		private string prefix;
		private bool writePrefix;

        /// <summary>
        /// Constructs an instance of the <see cref="IndentingTextWriter"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        /// <param name="useTabs">
        /// If true, use tab characters (U+0009) for indentation;
        /// if false, use space characters (U+0020) for indentation.</param>
        /// <param name="tabWidth">Width of each indentation step in characters/
        /// </param>
		public IndentingTextWriter(TextWriter writer, bool useTabs, int tabWidth)
		{
			this.writer = writer;
			this.useTabs = useTabs;
			this.tabWidth = tabWidth;
			this.tabStops = 0;
			this.prefix = "";
		}

        /// <summary>
        /// Indent one tab stop.
        /// </summary>
		public void Indent()
		{
			++tabStops;
			MakePrefix();
		}

        /// <summary>
        /// De-indent one tab stop.
        /// </summary>
		public void Outdent()
		{
			--tabStops;
			if (tabStops < 0)
			{
				tabStops = 0;
				throw new InvalidOperationException("Can't have a negative indentation.");
			}
			MakePrefix();
		}

		private void MakePrefix()
		{
			prefix = useTabs ? new String('\t', tabStops) : new String(' ', tabStops * tabWidth);
		}

        /// <summary>
        /// Write the <paramref name="s"/> string to the output.
        /// </summary>
        /// <param name="s">String to output.</param>
		public void Write(string s)
		{
			WriteIndentation();
			writer.Write(s);
		}

        /// <summary>
        /// Write the <paramref name="formatString"/> format string to the output.
        /// </summary>
        /// <param name="formatString">Format string to output.</param>
        /// <param name="objs">Values to use in the format string.</param>
		public void Write(string formatString, params object [] objs)
		{
			WriteIndentation();
			writer.Write(formatString, objs);
		}

		private void WriteIndentation()
		{
			if (writePrefix)
			{
				writer.Write(prefix);
				writePrefix = false;
			}
		}

        /// <summary>
        /// Writes a new line.
        /// </summary>
		public void WriteLine()
		{
			WriteIndentation();
			writer.WriteLine();
			writePrefix = true;
		}

        /// <summary>
        /// Writes a string, followed by a new line.
        /// </summary>
		public void WriteLine(string s)
		{
			WriteIndentation();
			writer.WriteLine(s);
			writePrefix = true;
		}

        /// <summary>
        /// Write the <paramref name="formatString"/> format string to the output,
        /// followed by a newline.
        /// </summary>
        /// <param name="formatString">Format string to output.</param>
        /// <param name="objs">Values to use in the format string.</param>
		public void WriteLine(string formatString, params object [] objs)
		{
			WriteIndentation();
			writer.WriteLine(formatString, objs);
			writePrefix = true;
		}
	}
}
