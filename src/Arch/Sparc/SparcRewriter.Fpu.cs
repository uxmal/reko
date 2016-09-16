#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteFabss()
        {
            var dst = RewriteOp(instrCur.Op2);
            var src = RewriteOp(instrCur.Op1);
            emitter.Assign(dst, host.PseudoProcedure("fabs", PrimitiveType.Real32, src));
        }

        private void RewriteFadds()
        {
            var dst = (RegisterOperand)instrCur.Op3;
            var src1 = (RegisterOperand)instrCur.Op1;
            var src2 = (RegisterOperand)instrCur.Op2;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = frame.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = frame.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            emitter.Assign(fdst, emitter.FAdd(fsrc1, fsrc2));
        }

        private void RewriteFcmpes()
        {
            var r1 = (RegisterOperand)instrCur.Op1;
            var r2 = (RegisterOperand)instrCur.Op2;

            var f1 = frame.EnsureRegister(Registers.GetFpuRegister(r1.Register.Number)); 
            var f2 = frame.EnsureRegister(Registers.GetFpuRegister(r2.Register.Number));
            var grf = frame.EnsureFlagGroup(arch.GetFlagGroup("ELGU"));
            emitter.Assign(grf, emitter.Cond(emitter.FSub(f1, f2)));
        }

        private void RewriteFdivs()
        {
            var dst = (RegisterOperand)instrCur.Op3;
            var src1 = (RegisterOperand)instrCur.Op1;
            var src2 = (RegisterOperand)instrCur.Op2;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = frame.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = frame.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            emitter.Assign(fdst, emitter.FDiv(fsrc1, fsrc2));
        }

        private void RewriteFitod()
        {
            var dst = (RegisterOperand) instrCur.Op2;
            var r0 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var r1 = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number + 1));
            var dt = PrimitiveType.Real64;
            var fpDst = frame.EnsureSequence(r0.Storage, r1.Storage, dt);
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

        private void RewriteFmovs()
        {
            var dst = (RegisterOperand)instrCur.Op2;
            var src = (RegisterOperand)instrCur.Op1;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc = frame.EnsureRegister(Registers.GetFpuRegister(src.Register.Number));
            emitter.Assign(fdst, fsrc);
        }

        private void RewriteFmuls()
        {
            var dst = (RegisterOperand)instrCur.Op3;
            var src1 = (RegisterOperand)instrCur.Op1;
            var src2 = (RegisterOperand)instrCur.Op2;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = frame.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = frame.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            emitter.Assign(fdst, emitter.FMul(fsrc1, fsrc2));
        }

        private void RewriteFnegs()
        {
            var dst = (RegisterOperand)instrCur.Op2;
            var src = (RegisterOperand)instrCur.Op1;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc = frame.EnsureRegister(Registers.GetFpuRegister(src.Register.Number));
            emitter.Assign(fdst, emitter.Neg(fsrc));
        }

        private void RewriteFsubs()
        {
            var dst = (RegisterOperand)instrCur.Op3;
            var src1 = (RegisterOperand)instrCur.Op1;
            var src2 = (RegisterOperand)instrCur.Op2;
            var fdst = frame.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = frame.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = frame.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            emitter.Assign(fdst, emitter.FSub(fsrc1, fsrc2));
        }
    }
}
