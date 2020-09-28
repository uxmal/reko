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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;

namespace Reko.Core
{
    /// <summary>
    /// A dispatch procedure is a procedure which, based on one
    /// or more parameters, dispatches to sub-services with potentially
    /// different signatures.
    /// </summary>
    /// <remarks>
    public class DispatchProcedure : ProcedureBase
    {
        private List<(SyscallInfo, ExternalProcedure)> services;
        private FunctionType dummySig;

        public DispatchProcedure(string name, List<(SyscallInfo, ExternalProcedure)> services) : base(name)
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
        public ExternalProcedure FindService(ProcessorState state)
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
