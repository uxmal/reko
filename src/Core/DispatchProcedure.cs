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

using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core
{
    /// <summary>
    /// A dispatch procedure is a procedure which, based on one
    /// or more parameters, dispatches to sub-services with potentially
    /// different signatures.
    /// </summary>
    /// An example of a dispatch procedure is the CP/M BDOS service located
    /// at address 0x0005.
    /// </remarks>
    public class DispatchProcedure : ProcedureBase
    {
        private List<(SyscallInfo, ExternalProcedure)> services;
        private FunctionType dummySig;

        /// <summary>
        /// Constructs a <see cref="DispatchProcedure"/> instance.
        /// </summary>
        /// <param name="name">The name of the procedure.</param>
        /// <param name="services">A list of (<see cref="SyscallInfo"/>, 
        /// <see cref="ExternalProcedure"/>) pairs.
        /// </param>
        public DispatchProcedure(string name, List<(SyscallInfo, ExternalProcedure)> services) : base(name, true)
        {
            this.services = services;
            this.dummySig = new FunctionType();
        }

        /// <summary>
        /// The signature is a fallback if a sub-service cannot be determined.
        /// </summary>
        public override FunctionType Signature 
        {
            get { return dummySig; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Given a <see cref="ProcessorState"/>, find the service that matches
        /// that state.
        /// </summary>
        /// <param name="state"><see cref="ProcessorState"/> to use to determine
        /// the specific procedure dispatched to.
        /// </param>
        /// <returns>An <see cref="ExternalProcedure"/> if enough information 
        /// was supplied to determine the procedure; otherwise null.
        /// </returns>
        public ExternalProcedure? FindService(ProcessorState state)
        {
            foreach (var (si, proc) in services)
            {
                if (si.Matches(state))
                    return proc;
            }
            return null;
        }
    }
}
