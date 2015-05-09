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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteFitod()
        {
            var dst = (RegisterOperand) instrCur.Op2;
            var r0 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var r1 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number + 1));
            var dt = PrimitiveType.Real64;
            var fpDst = frame.EnsureSequence(r0, r1, dt);
            emitter.Assign(fpDst, emitter.Cast(dt, RewriteOp(instrCur.Op1)));
        }

        private void RewriteFitoq()
        {
            throw new NotSupportedException("Sequences don't work well with multiple segments");
            //var dst = (RegisterOperand) di.Instr.Op2;
            //var r0 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            //var r1 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number + 1));
            //var dt = PrimitiveType.Real64;
            //var fpDst = frame.EnsureSequence(r0, r1, dt);
            //emitter.Assign(fpDst, emitter.Cast(dt, RewriteOp(src)));
        }

        private void RewriteFitos()
        {
            var dst = (RegisterOperand) instrCur.Op2;
            var fpDst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var dt = PrimitiveType.Real32;
            emitter.Assign(fpDst, emitter.Cast(dt, RewriteOp(instrCur.Op1)));
        }
    }
}
