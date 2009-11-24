/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.IO;

namespace Decompiler.Core.Output
{
	/// <summary>
	/// Base class for all formatting classes. 
	/// </summary>
	public class Formatter
	{
		protected TextWriter writer;
		private int indentation;
		private int tabSize;
		private bool useTabs;
		private string terminator;

		public Formatter(TextWriter writer)
		{
			this.writer = writer;
			this.useTabs = true;
			this.tabSize = 4;
			this.indentation = 4;
			this.terminator = Environment.NewLine;
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


		public int Indentation
		{
			get { return indentation; }
			set { indentation = value; }
		}

		public int TabSize
		{
			get { return tabSize; }
			set { tabSize = value; }
		}

		public void Terminate()
		{
			Write(terminator);
		}

		public void Terminate(string s)
		{
			Write(s);
			Write(terminator);
		}

		public string Terminator
		{
			get { return terminator; }
			set { terminator = value; }
		}

		public bool UseTabs
		{
			get { return useTabs; }
			set { useTabs = value; }
		}

        public virtual void Write(string s)
        {
            writer.Write(s);
        }

        public virtual void Write(string format, params object[] arguments)
        {
            writer.Write(format, arguments);
        }

	
        public virtual void WriteKeyword(string keyword)
        {
            writer.Write(keyword);
        }

        public virtual void WriteLine()
        {
            writer.WriteLine();
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
