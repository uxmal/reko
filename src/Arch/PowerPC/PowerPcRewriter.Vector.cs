#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
                host.Intrinsic(
                    "__vaddfp",
                    false,
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
                host.Intrinsic(
                    "__vadduwm",
                    false,
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        private void RewriteVectorBinOp(string intrinsic, bool hasSideEffect, DataType elemType)
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
                host.Intrinsic(intrinsic, hasSideEffect, arrayType, tmp1, tmp2));
        }

        private void RewriteVectorTernaryOp(string intrinsic, bool hasSideEffect, DataType elemType)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            var tmp3 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, vra);
            m.Assign(tmp2, vrb);
            m.Assign(tmp3, vrc);
            m.Assign(
                vrt,
                host.Intrinsic(intrinsic, hasSideEffect, arrayType, tmp1, tmp2, tmp3));
        }

        private void RewriteVectorPairOp(string intrinsic, bool hasSideEffect, DataType elemType)
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
                host.Intrinsic(intrinsic, hasSideEffect, arrayType, tmp1, tmp2));
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
                host.Intrinsic(
                    fnName,
                    false,
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
                host.Intrinsic(
                    fnName,
                    false,
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
                host.Intrinsic(name, false, d.DataType, a, b));
        }

        private void RewriteVcsxwfp(string name)
        {
            var d = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(d,
                host.Intrinsic(name, false, d.DataType, a, b));
        }

        public void RewriteVct(string name, PrimitiveType dt)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vrb = RewriteOperand(instr.Operands[1]);
            var uim = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.Intrinsic(
                    name,
                    true,
                    new ArrayType(dt, 4),
                    vrb,
                    uim));
        }

        private void RewriteVectorUnary(string intrinsic, bool hasSideEffect)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(vrt, host.Intrinsic(intrinsic, hasSideEffect, vrt.DataType, vra));
        }

        public void RewriteVmaddfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            var vrc = RewriteOperand(instr.Operands[3]);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vmaddfp",
                    false,
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
                host.Intrinsic(
                    "__vmrghw",
                    false,
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
                host.Intrinsic(
                    "__vmrglw",
                    false,
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
                host.Intrinsic(
                    "__vnmsubfp",
                    false,
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
                host.Intrinsic(
                    "__vperm",
                    false,
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
                host.Intrinsic("__vpkd3d", false, vt.DataType, va, vb, vc, vd));
        }

        public void RewriteVrefp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vrefp",
                    false,
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
                host.Intrinsic(
                    "__vrlimi",
                    false,
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
                host.Intrinsic(
                    "__vrsqrtefp",
                    true,
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
                host.Intrinsic(
                    "__vsel",
                    false,
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
                host.Intrinsic(
                    "__vsldoi",
                    true,
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
                host.Intrinsic(
                    intrinsic,
                    true,
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
                host.Intrinsic(
                    "__vspltisw",
                    false,
                    PrimitiveType.Word128,
                    sha));
        }
        
        public void RewriteVspltw()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var opI = RewriteOperand(instr.Operands[2]);

            m.Assign(opD, host.Intrinsic("__vspltw", false, opD.DataType, opS, opI));
        }

        public void RewriteVsubfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vsubfp",
                    false,
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

            m.Assign(opD, host.Intrinsic("__lvlx", false, opD.DataType, opS, opI));
        }

        public void RewriteLvrx()
        {
            //$TODO: can't find any documentation of the LVLX instruction or what it does.
            // assuming an instrinsic is used for this.
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var opI = RewriteOperand(instr.Operands[2]);

            m.Assign(opD, host.Intrinsic("__lvrx", false, opD.DataType, opS, opI));
        }

        private void RewriteMtvsrws()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, host.Intrinsic("__mtvsrws", false, opD.DataType, opS));
        }

        // Very specific to XBOX 360

        private void RewriteVupkd3d()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opA = RewriteOperand(instr.Operands[1]);
            var opB = RewriteOperand(instr.Operands[2]);
            m.Assign(opD, host.Intrinsic("__vupkd3d", false, opD.DataType, opA, opB));
        }

    }
}
