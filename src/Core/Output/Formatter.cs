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
using System.IO;
using System.Globalization;

namespace Decompiler.Core.Output
{
	/// <summary>
	/// Base class for all formatting classes. 
	/// </summary>
	public class Formatter
	{
		public Formatter(TextWriter writer)
		{
			this.TextWriter = writer;
			this.UseTabs = true;
			this.TabSize = 4;
			this.Indentation = 4;
			this.Terminator = Environment.NewLine;
		}

		public void Indent()
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

		public int Indentation { get; set; }
		public int TabSize  {get; set; }
        public string Terminator { get; set; }
        public bool UseTabs { get; set; }
        public TextWriter TextWriter { get; private set; }

        /// <summary>
        /// Terminate a line using the terminator string.
        /// </summary>
        public void Terminate()
		{
			TextWriter.Write(Terminator);
		}

        /// <summary>
        /// Write the string <paramref name="s"/>, then terminate the line.
        /// </summary>
        /// <param name="s"></param>
		public void Terminate(string s)
		{
			Write(s);
			TextWriter.Write(Terminator);
		}

        public virtual void Write(string s)
        {
            TextWriter.Write(s);
        }

        public virtual void Write(string format, params object[] arguments)
        {
            TextWriter.Write(format, arguments);
        }

        public virtual void WriteComment(string comment)
        {
            TextWriter.Write(comment);
        }

        public virtual void WriteKeyword(string keyword)
        {
            TextWriter.Write(keyword);
        }

        public virtual void WriteLine()
        {
            TextWriter.WriteLine();
        }

		public void WriteSpaces(int n)
		{
			while (n > 0)
			{
				Write(" ");
				--n;
			}
		}
    }
}
