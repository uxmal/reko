/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using System;
using System.Collections;
using System.IO;
namespace Decompiler.Analysis
{
	public class InterferenceGraph
	{
		private Hashtable intf;

		public InterferenceGraph()
		{
			intf = new Hashtable();
		}

		public void Add(Identifier id1, Identifier id2)
		{
			if (id1 != id2)
			{
				Interference i = new Interference(id1, id2);
				if (!intf.Contains(i))
					intf.Add(i, i);
			}
		}

		public bool Interfere(Identifier id1, Identifier id2)
		{
			Interference i = new Interference(id1, id2);
			return intf.Contains(i);
		}

		public void Write(TextWriter sb)
		{
			SortedList sl = new SortedList(intf);
			foreach (Interference i in sl.Values)
			{
				sb.WriteLine("{0} interferes with {1}", i.Identifier1, i.Identifier2);
			}
		}
	}
}
