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

using Reko.Core;
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// A heuristic procedure is a procedure that has been tentatively identified.
    /// </summary>
    public class HeuristicProcedure
    {
        /// <summary>
        /// Constructs a heuristic procedure with the given address range and frame.
        /// </summary>
        /// <param name="addrBegin">Start address of the procedure.</param>
        /// <param name="addrEnd">End address of the procedure.</param>
        /// <param name="frame"><see cref="Frame"/> used for the identifiers
        /// of the procedure.
        /// </param>
        public HeuristicProcedure(
            Address addrBegin,
            Address addrEnd,
            Frame frame)
        {
            this.BeginAddress = addrBegin;
            this.EndAddress = addrEnd;
            this.Frame = frame;
        }

        /// <summary>
        /// Control flow graph of the procedure.
        /// </summary>
        public DiGraph<RtlBlock> Cfg = new();

        /// <summary>
        /// <see cref="Core.Frame"/> of the procedure.
        /// </summary>
        public Frame Frame;

        /// <summary>
        /// Beginning address of the procedure.
        /// </summary>
        public Address BeginAddress;

        /// <summary>
        /// End address of the procedure.
        /// </summary>
        public Address EndAddress;

        /// <summary>
        /// Determines whether the given address is within the heurstic procedure's
        /// address range.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the address is contained withi the procedure's address
        /// range; false if not.
        /// </returns>
        public bool IsValidAddress(Address addr)
        {
            return this.BeginAddress <= addr && addr < this.EndAddress;
        }
    }
}
