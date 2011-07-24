#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    class Pdp11ProcessorState : ProcessorState
    {
        public ProcessorState Clone()
        {
            throw new NotImplementedException();
        }

        public void SetRegister(MachineRegister r, Constant v)
        {
            throw new NotImplementedException();
        }

        public void SetInstructionPointer(Address addr)
        {
            throw new NotImplementedException();
        }

        public Constant GetRegister(MachineRegister r)
        {
            throw new NotImplementedException();
        }

        #region ProcessorState Members


        public void OnProcedureEntered()
        {
            throw new NotImplementedException();
        }

        public void OnProcedureLeft(ProcedureSignature procedureSignature)
        {
            throw new NotImplementedException();
        }

        public CallSite OnBeforeCall(int returnSize)
        {
            throw new NotImplementedException();
        }

        public void OnAfterCall(ProcedureSignature sigCallee)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
