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
        private ArrayType MakeArrayType(DataType bitvectorType, DataType elemType)
        {
            var cElems = bitvectorType.BitSize / elemType.BitSize;
            return new ArrayType(elemType, cElems);
        }

        private void MaybeEmitCr6(Expression e)
        {
            if (!instr.setsCR0)
                return;
            var cr6 = binder.EnsureRegister(arch.CrRegisters[6]);
            m.Assign(cr6, m.Cond(e));
        }

        public void RewriteVaddfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vaddfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVadduwm()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vadduwm",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        private void RewriteVectorBinOp(string intrinsic, DataType elemType)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, vra);
            m.Assign(tmp2, vrb);
            m.Assign(
                vrt,
                host.PseudoProcedure(intrinsic, arrayType, tmp1, tmp2));
        }

        private void RewriteVectorPairOp(string intrinsic, DataType elemType)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, vra);
            m.Assign(tmp2, vrb);
            m.Assign(
                vrt,
                host.PseudoProcedure(intrinsic, arrayType, tmp1, tmp2));
            m.Assign(
                binder.EnsureRegister(arch.acc),
                vrt);
        }


        public void RewriteVcmpfp(string fnName)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    fnName,
                    new ArrayType(PrimitiveType.Int32, 4),
                    vra,
                    vrb));
            MaybeEmitCr6(vrt);
        }

        public void RewriteVcmpu(string fnName, DataType elemType)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    fnName,
                    MakeArrayType(vra.DataType, elemType),
                    vra,
                    vrb));
            MaybeEmitCr6(vrt);
        }

        private void RewriteVcfpsxws(string name)
        {
            var d = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(d,
                host.PseudoProcedure(name, d.DataType, a, b));
        }

        private void RewriteVcsxwfp(string name)
        {
            var d = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(d,
                host.PseudoProcedure(name, d.DataType, a, b));
        }

        public void RewriteVct(string name, PrimitiveType dt)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vrb = RewriteOperand(instr.Operands[1]);
            var uim = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    name,
                    new ArrayType(dt, 4),
                    vrb,
                    uim));
        }

        private void RewriteVectorUnary(string intrinsic)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(vrt, host.PseudoProcedure(intrinsic, vrt.DataType, vra));
        }

        public void RewriteVmaddfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vmaddfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        public void  RewriteVmrghw()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vmrghw",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVmrglw()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vmrglw",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVnmsubfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vnmsubfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        private void RewriteVor()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            if (vra == vrb)
            {
                m.Assign(vrt, vra);
            }
            else
            {
                m.Assign(vrt, m.Or(vra, vrb));
            }
        }

        public void RewriteVperm()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vperm",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb,
                    vrc));
        }

        private void RewriterVpkD3d()
        {
            var vt = RewriteOperand(instr.Operands[0]);
            var va = RewriteOperand(instr.Operands[1]);
            var vb = RewriteOperand(instr.Operands[2]);
            var vc = RewriteOperand(instr.Operands[3]);
            var vd = RewriteOperand(instr.Operands[4]);
            m.Assign(
                vt,
                host.PseudoProcedure("__vpkd3d", vt.DataType, va, vb, vc, vd));
        }

        public void RewriteVrefp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vrefp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra));
        }

        private void RewriteVrlimi()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vrlimi",
                    PrimitiveType.Word128,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVrsqrtefp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vrsqrtefp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra));
        }

        public void RewriteVsel()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vsel",
                    PrimitiveType.Word128,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVsldoi()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vsldoi",
                    PrimitiveType.Word128,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVsxw(string intrinsic)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    intrinsic,
                    new ArrayType(PrimitiveType.Word32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVspltisw()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var sha = RewriteOperand(instr.Operands[1]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vspltisw",
                    PrimitiveType.Word128,
                    sha));
        }
        
        public void RewriteVspltw()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var opI = RewriteOperand(instr.Operands[2]);

            m.Assign(opD, host.PseudoProcedure("__vspltw", opD.DataType, opS, opI));
        }

        public void RewriteVsubfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__vsubfp",
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteLvlx()
        {
            //$TODO: can't find any documentation of the LVLX instruction or what it does.
            // assuming an instrinsic is used for this.
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var opI = RewriteOperand(instr.Operands[2]);

            m.Assign(opD, host.PseudoProcedure("__lvlx", opD.DataType, opS, opI));
        }

        public void RewriteLvrx()
        {
            //$TODO: can't find any documentation of the LVLX instruction or what it does.
            // assuming an instrinsic is used for this.
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var opI = RewriteOperand(instr.Operands[2]);

            m.Assign(opD, host.PseudoProcedure("__lvrx", opD.DataType, opS, opI));
        }

        // Very specific to XBOX 360

        private void RewriteVupkd3d()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opA = RewriteOperand(instr.Operands[1]);
            var opB = RewriteOperand(instr.Operands[2]);
            m.Assign(opD, host.PseudoProcedure("__vupkd3d", opD.DataType, opA, opB));
        }

    }
}
