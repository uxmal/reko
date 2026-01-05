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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;

namespace Reko.Arch.MN103;

public class MN103State : ProcessorState
{
    private readonly MN103Architecture arch;
    private readonly Constant[] regs;

    public MN103State(MN103Architecture arch)
    {
        this.arch = arch;
        this.regs = new Constant[Registers.RegistersByDomain.Count];
    }

    public override IProcessorArchitecture Architecture => arch;

    public override ProcessorState Clone()
    {
        return new MN103State(this.arch);
    }

    public override Constant GetRegister(RegisterStorage r)
    {
        var ireg = (uint) r.Domain;
        if (ireg >= (uint) regs.Length)
        {
            return InvalidConstant.Create(r.DataType);
        }
        return regs[ireg];
    }

    public override void OnAfterCall(FunctionType? sigCallee)
    {
    }

    public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
    {
        return new CallSite(returnAddressSize, 0);
    }

    public override void OnProcedureEntered(Address addr)
    {
        Array.Clear(regs);
    }

    public override void OnProcedureLeft(FunctionType procedureSignature)
    {
        Array.Clear(regs);
    }

    public override void SetRegister(RegisterStorage r, Constant v)
    {
        var ireg = (uint) r.Domain;
        if (ireg >= (uint) regs.Length)
        {
            return;
        }
        regs[ireg] = v;
    }
}
