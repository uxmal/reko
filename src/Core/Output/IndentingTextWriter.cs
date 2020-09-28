#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
		private TextWriter writer;
		private bool useTabs;
		private int tabWidth;
		private int tabStops;
		private string prefix;
		private bool writePrefix;

		public IndentingTextWriter(TextWriter writer, bool useTabs, int tabWidth)
		{
			this.writer = writer;
			this.useTabs = useTabs;
			this.tabWidth = tabWidth;
			this.tabStops = 0;
			this.prefix = "";
		}

		public void Indent()
		{
			++tabStops;
			MakePrefix();
		}

		public TextWriter InnerTextWriter
		{
			get { return writer; }
		}

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

		public bool SuspendIndent
		{
			get { return !writePrefix; }
			set { writePrefix = !value; }
		}

		public void Write(string s)
		{
			WriteIndentation();
			writer.Write(s);
		}

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

		public void WriteLine()
		{
			WriteIndentation();
			writer.WriteLine();
			writePrefix = true;
		}

		public void WriteLine(string s)
		{
			WriteIndentation();
			writer.WriteLine(s);
			writePrefix = true;
		}

		public void WriteLine(string formatString, params object [] objs)
		{
			WriteIndentation();
			writer.WriteLine(formatString, objs);
			writePrefix = true;
		}
	}
}
