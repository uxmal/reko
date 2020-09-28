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
            var dst = RewriteOp(instrCur.Operands[1]);
            var src = RewriteOp(instrCur.Operands[0]);
            m.Assign(dst, host.PseudoProcedure("fabs", PrimitiveType.Real32, src));
        }

        private void RewriteFadds()
        {
            var dst = (RegisterOperand)instrCur.Operands[2];
            var src1 = (RegisterOperand)instrCur.Operands[0];
            var src2 = (RegisterOperand)instrCur.Operands[1];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = binder.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = binder.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            m.Assign(fdst, m.FAdd(fsrc1, fsrc2));
        }

        private void RewriteFcmpes()
        {
            var r1 = (RegisterOperand)instrCur.Operands[0];
            var r2 = (RegisterOperand)instrCur.Operands[1];

            var f1 = binder.EnsureRegister(Registers.GetFpuRegister(r1.Register.Number)); 
            var f2 = binder.EnsureRegister(Registers.GetFpuRegister(r2.Register.Number));
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU"));
            m.Assign(grf, m.Cond(m.FSub(f1, f2)));
        }

        private void RewriteFcmpd()
        {
            var r1 = RewriteDoubleRegister(instrCur.Operands[0]);
            var r2 = RewriteDoubleRegister(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU"));
            m.Assign(grf, m.Cond(m.FSub(r2, r1)));
        }

        private void RewriteFcmpq()
        {
            var r1 = RewriteQuadRegister(instrCur.Operands[0]);
            var r2 = RewriteQuadRegister(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU"));
            m.Assign(grf, m.Cond(m.FSub(r2, r1)));
        }

        private void RewriteFcmps()
        {
            var r1 = RewriteRegister(instrCur.Operands[0]);
            var r2 = RewriteRegister(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU"));
            m.Assign(grf, m.Cond(m.FSub(r2, r1)));
        }

        private void RewriteFdivd()
        {
            var fdst = RewriteDoubleRegister(instrCur.Operands[2]);
            var fsrc1 = RewriteDoubleRegister(instrCur.Operands[0]);
            var fsrc2 = RewriteDoubleRegister(instrCur.Operands[1]);
            m.Assign(fdst, m.FDiv(fsrc1, fsrc2));
        }

        private void RewriteFdivs()
        {
            var dst = (RegisterOperand)instrCur.Operands[2];
            var src1 = (RegisterOperand)instrCur.Operands[0];
            var src2 = (RegisterOperand)instrCur.Operands[1];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = binder.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = binder.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            m.Assign(fdst, m.FDiv(fsrc1, fsrc2));
        }

        private void RewriteFitod()
        {
            var dst = (RegisterOperand) instrCur.Operands[1];
            var r0 = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var r1 = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number + 1));
            var dt = PrimitiveType.Real64;
            var fpDst = binder.EnsureSequence(dt, r0.Storage, r1.Storage);
            m.Assign(fpDst, m.Cast(dt, RewriteOp(instrCur.Operands[0])));
        }

        private void RewriteFdtos()
        {
            var fpDst = RewriteOp(instrCur.Operands[1]);
            var dt = PrimitiveType.Real32;
            m.Assign(fpDst, m.Cast(dt, RewriteOp(instrCur.Operands[0])));
        }

        private void RewriteFstod()
        {
            var fpDst = RewriteOp(instrCur.Operands[1]);
            var dt = PrimitiveType.Real64;
            m.Assign(fpDst, m.Cast(dt, RewriteOp(instrCur.Operands[0])));
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
            var dst = (RegisterOperand) instrCur.Operands[1];
            var fpDst = binder.EnsureRegister(dst.Register);
            var dt = PrimitiveType.Real32;
            m.Assign(fpDst, m.Cast(dt, RewriteOp(instrCur.Operands[0])));
        }

        private void RewriteFmovs()
        {
            var dst = (RegisterOperand)instrCur.Operands[1];
            var src = (RegisterOperand)instrCur.Operands[0];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc = binder.EnsureRegister(Registers.GetFpuRegister(src.Register.Number));
            m.Assign(fdst, fsrc);
        }

        private void RewriteFmuls()
        {
            var dst = (RegisterOperand)instrCur.Operands[2];
            var src1 = (RegisterOperand)instrCur.Operands[0];
            var src2 = (RegisterOperand)instrCur.Operands[1];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = binder.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = binder.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            m.Assign(fdst, m.FMul(fsrc1, fsrc2));
        }

        private void RewriteFnegs()
        {
            var dst = (RegisterOperand)instrCur.Operands[1];
            var src = (RegisterOperand)instrCur.Operands[0];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc = binder.EnsureRegister(Registers.GetFpuRegister(src.Register.Number));
            m.Assign(fdst, m.Neg(fsrc));
        }

        private void RewriteFsubs()
        {
            var dst = (RegisterOperand)instrCur.Operands[2];
            var src1 = (RegisterOperand)instrCur.Operands[0];
            var src2 = (RegisterOperand)instrCur.Operands[1];
            var fdst = binder.EnsureRegister(Registers.GetFpuRegister(dst.Register.Number));
            var fsrc1 = binder.EnsureRegister(Registers.GetFpuRegister(src1.Register.Number));
            var fsrc2 = binder.EnsureRegister(Registers.GetFpuRegister(src2.Register.Number));
            m.Assign(fdst, m.FSub(fsrc1, fsrc2));
        }
    }
}
