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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections;

namespace Decompiler.Analysis
{
	public class InductionVariableCollection
	{
		private Hashtable mpproctocoll;

		public InductionVariableCollection()
		{
			mpproctocoll = new Hashtable();
		}

		public LinearInductionVariable this[Procedure proc, Identifier id]
		{
			get 
			{
				Hashtable h = (Hashtable) mpproctocoll[proc];
				if (h == null)
					return null;
				return (LinearInductionVariable) h[id];
			}
		}


		public void Add(Procedure proc, Identifier id, LinearInductionVariable iv)
		{
			Hashtable h = (Hashtable) mpproctocoll[proc];
			if (h == null)
			{
				h = new Hashtable();
				mpproctocoll[proc] = h;
			}
			h[id] = iv;
		}
	}
}
