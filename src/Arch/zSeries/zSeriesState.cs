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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.zSeries
{
    public class zSeriesState : ProcessorState
    {
        private zSeriesArchitecture arch;
        private Dictionary<RegisterStorage, Constant> values;

        public zSeriesState(zSeriesArchitecture arch)
        {
            this.arch = arch;
        }

        public zSeriesState(zSeriesState that)
        {
            this.arch = that.arch;
            this.values = new Dictionary<RegisterStorage, Constant>();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override ProcessorState Clone()
        {
            return new zSeriesState(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            return Constant.Invalid;
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(0, 0);
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
            throw new System.NotImplementedException();
        }

        public override void SetInstructionPointer(Address addr)
        {
            //$TODO: is the IP exposed, architecturally for s390 / zSeries?
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
        }
    }
}