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
using Decompiler.Core;

namespace Decompiler.Scanning
{
	/// <summary>
	/// In the absence of any other information, scans address ranges in search of code sequences that may represent
	/// valid procedures. It needs help from the processor architecture to specify what byte patterns to look for.
	/// </summary>
	public class HeuristicScanner
	{
		private Program prog;

		public HeuristicScanner(Program prog)
		{
			this.prog = prog;
		}
	}
}
