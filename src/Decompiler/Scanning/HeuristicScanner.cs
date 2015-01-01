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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        /// <summary>
        /// Determines the locations of all instructions that perform a 
        /// CALL / JSR / BL to a _known_ procedure address.
        /// </summary>
        /// <param name="knownProcedureAddresses">A sequence of addresses
        /// that are known to be procedures.</param>
        /// <returns>A sequence of linear addresses where those call 
        /// instructions are.</returns>
        public IEnumerable<uint> FindCallOpcodes(IEnumerable<Address> knownProcedureAddresses)
        {
            var procEntryLinearAddresses = 
                knownProcedureAddresses
                .Select(addr => addr.Linear)
                .ToHashSet();
            return prog.Architecture.CreatePointerScanner(
                prog.Architecture.CreateImageReader(prog.Image, 0),
                procEntryLinearAddresses,
                PointerScannerFlags.Calls);
        }
    }
}
