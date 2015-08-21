﻿#region License
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void MaybeEmitCr6(Expression e)
        {
            if (!instr.setsCR0)
                return;
            var cr6 = frame.EnsureRegister(arch.CrRegisters[6]);
            emitter.Assign(cr6, emitter.Cond(e));
        }

        public void RewriteVaddfp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vaddfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVadduwm()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vadduwm",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVcmpfp(string fnName)
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    fnName,
                    new ArrayType(PrimitiveType.Int32, 4),
                    vra,
                    vrb));
            MaybeEmitCr6(vrt);
        }

        public void RewriteVcmpuw(string fnName)
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    fnName,
                    new ArrayType(PrimitiveType.Int32, 4),
                    vra,
                    vrb));
            MaybeEmitCr6(vrt);
        }

        public void RewriteVct(string name, PrimitiveType dt)
        {
            var vrt = RewriteOperand(instr.op1);
            var vrb = RewriteOperand(instr.op2);
            var uim = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    name,
                    new ArrayType(dt, 4),
                    vrb,
                    uim));
        }

        public void RewriteVmaddfp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            var vrc = RewriteOperand(instr.op4);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vmaddfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        public void  RewriteVmrghw()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vmrghw",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVmrglw()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vmrglw",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVnmsubfp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            var vrc = RewriteOperand(instr.op4);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vnmsubfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVperm()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            var vrc = RewriteOperand(instr.op4);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vperm",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVrefp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vrefp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra));
        }

        public void RewriteVrsqrtefp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vrsqrtefp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra));
        }

        public void RewriteVsel()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            var vrc = RewriteOperand(instr.op4);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vsel",
                    PrimitiveType.Word128,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVsldoi()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            var vrc = RewriteOperand(instr.op4);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vsldoi",
                    PrimitiveType.Word128,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVslw()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vslw",
                    new ArrayType(PrimitiveType.Word32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVspltisw()
        {
            var vrt = RewriteOperand(instr.op1);
            var sha = RewriteOperand(instr.op2);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vspltisw",
                    PrimitiveType.Word128,
                    sha));
        }
        
        public void RewriteVspltw()
        {
            var opD = RewriteOperand(instr.op1);
            var opS = RewriteOperand(instr.op2);
            var opI = RewriteOperand(instr.op3);

            emitter.Assign(opD, PseudoProc("__vspltw", opD.DataType, opS, opI));
        }

        public void RewriteVsubfp()
        {
            var vrt = RewriteOperand(instr.op1);
            var vra = RewriteOperand(instr.op2);
            var vrb = RewriteOperand(instr.op3);
            emitter.Assign(
                vrt,
                PseudoProc(
                    "__vsubfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteLvlx()
        {
            //$TODO: can't find any documentation of the LVLX instruction or what it does.
            // assuming an instrinsic is used for this.
            var opD = RewriteOperand(instr.op1);
            var opS = RewriteOperand(instr.op2);
            var opI = RewriteOperand(instr.op3);

            emitter.Assign(opD, PseudoProc("__lvlx", opD.DataType, opS, opI));
        }
    }
}
