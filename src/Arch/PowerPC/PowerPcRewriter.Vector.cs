#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

        private void RewriteBcds()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(vrt, m.Fn(bcds, vra, vrb));
        }

        private void RewriteBcdtrunc()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(vrt, m.Fn(bcdtrunc, vra, vrb));
        }

        private void RewriteBcdus()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(vrt, m.Fn(bcdus, vra, vrb));
        }


        private void RewriteVaddecuq()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrd = RewriteOperand(0);
            m.Assign(vrd, m.Fn(vaddecuq.MakeInstance(vra.DataType), vra, vrb, vrc));
        }

        private void RewriteVaddfp()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            var vrb = RewriteOperand(instr.Operands[2]);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vfp",
                    false,
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        public void RewriteVadduwm()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vadduwm",
                    false,
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra,
                    vrb));
        }

        private void RewriteVbperm(DataType dt)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(vrt, m.Fn(
                vbperm.MakeInstance(MakeArrayType(vra.DataType, dt)),
                vra,
                vrb));
        }

        private void RewriteVectorBinOp(IntrinsicProcedure intrinsic, DataType elemType)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            m.Assign(
                vrt,
                m.Fn(intrinsic.MakeInstance(arrayType), vra, vrb));
        }

        private void RewriteVectorBinOp(string intrinsic, bool hasSideEffect, DataType elemType)
        {
            var vrt = RewriteOperand(0);
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, vra);
            m.Assign(tmp2, vrb);
            m.Assign(
                vrt,
                host.Intrinsic(intrinsic, hasSideEffect, arrayType, tmp1, tmp2));
        }

        private void RewriteVectorTernaryOp(IntrinsicProcedure intrinsic, DataType elemType)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
            var arrayType = MakeArrayType(vrt.DataType, elemType);
            m.Assign(
                vrt,
                m.Fn(intrinsic.MakeInstance(arrayType), vra, vrb, vrc));
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

        public void RewriteVcmp(IntrinsicProcedure intrinsic, DataType elemType)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(
                    intrinsic.MakeInstance(MakeArrayType(vra.DataType, elemType)),
                    vra,
                    vrb));
            MaybeEmitCr6(vrt);
        }

        private void RewriteVcfp(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var a = RewriteOperand(1);
            var b = RewriteOperand(2);
            var d = RewriteOperand(0);
            m.Assign(d, m.Fn(
                vcfp.MakeInstance(
                    MakeArrayType(a.DataType, dtFrom),
                    MakeArrayType(d.DataType, dtTo)),
                a,
                b));
        }

        private void RewriteVcsxwfp(string name)
        {
            var d = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(d,
                host.Intrinsic(name, false, d.DataType, a, b));
        }

        public void RewriteVctfixed(IntrinsicProcedure intrinsic, PrimitiveType dt)
        {
            var vrb = RewriteOperand(1);
            var uim = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(
                    intrinsic.MakeInstance(
                        MakeArrayType(vrb.DataType, dt),
                        MakeArrayType(vrb.DataType, PrimitiveType.Real32)),
                    vrb,
                    uim));
        }

        private void RewriteVectorUnary(IntrinsicProcedure intrinsic, DataType elemType)
        {
            var vrd = RewriteOperand(0);
            var vrs = RewriteOperand(1);
            var arrayType = MakeArrayType(vrd.DataType, elemType);
            m.Assign(
                vrd,
                m.Fn(intrinsic.MakeInstance(arrayType), vrs));
        }

        private void RewriteVectorUnary(string intrinsic, bool hasSideEffect)
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var vra = RewriteOperand(instr.Operands[1]);
            m.Assign(vrt, host.Intrinsic(intrinsic, hasSideEffect, vrt.DataType, vra));
        }

        private void RewriteVextract(IntrinsicProcedure intrinsic, DataType dt)
        {
            var vra = RewriteOperand(1);
            var bit = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Fn(
                intrinsic.MakeInstance(dt),
                vra,
                bit));
            m.Assign(vrt, m.Dpb(
                new BigConstant(vrt.DataType, 0),
                tmp,
                64));
        }

        private void RewriteVmaddfp()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
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

        private void RewriteVmsumm(IntrinsicProcedure intrinsic, PrimitiveType dtElem)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
            var dtSrc = MakeArrayType(vra.DataType, dtElem);
            var dtDst = MakeArrayType(vra.DataType, PrimitiveType.Int32);
            m.Assign(vrt, m.Fn(
                intrinsic.MakeInstance(dtSrc, dtDst),
                vra,
                vrb,
                vrc));
        }

        private void RewriteVmuloe(IntrinsicProcedure intrinsic, PrimitiveType dtElemSrc, PrimitiveType  dtElemDst)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            var dtSrc = MakeArrayType(vra.DataType, dtElemSrc);
            var dtDst = MakeArrayType(vra.DataType, dtElemDst);
            m.Assign(vrt, m.Fn(
                intrinsic.MakeInstance(dtSrc, dtDst),
                vra, vrb));
        }

        public void RewriteVnmsubfp()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
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
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            if (vra == vrb)
            {
                m.Assign(vrt, vra);
            }
            else
            {
                m.Assign(vrt, m.Or(vra, vrb));
            }
        }

        private void RewriterVpkD3d()
        {
            var va = RewriteOperand(1);
            var vb = RewriteOperand(2);
            var vc = RewriteOperand(3);
            var vd = RewriteOperand(4);
            var vt = RewriteOperand(0);
            m.Assign(
                vt,
                host.Intrinsic("__vpkd3d", false, vt.DataType, va, vb, vc, vd));
        }

        private void RewriterVpks(DataType dtElemSrc, DataType dtElemDst)
        {
            var va = RewriteOperand(1);
            var vb = RewriteOperand(2);
            var vt = RewriteOperand(0);
            var dtSrc = MakeArrayType(va.DataType, dtElemSrc);
            var dtDst = MakeArrayType(vt.DataType, dtElemDst);
            m.Assign(
                vt,
                m.Fn(
                    vpks.MakeInstance(dtSrc, dtDst),
                    va, vb));
        }

        private void RewriteVpmsum(DataType dtElemSrc, DataType dtElemDst)
        {
            var va = RewriteOperand(1);
            var vb = RewriteOperand(2);
            var vt = RewriteOperand(0);
            var dtSrc = MakeArrayType(va.DataType, dtElemSrc);
            var dtDst = MakeArrayType(vt.DataType, dtElemDst);
            m.Assign(
                vt,
                m.Fn(
                    vpks.MakeInstance(dtSrc, dtDst),
                        va, vb));
        }

        private void RewriteVrefp()
        {
            var vra = RewriteOperand(1);
            var vrt = RewriteOperand(0);
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
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
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
            var vra = RewriteOperand(1);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                host.Intrinsic(
                    "__vrsqrtefp",
                    true,
                    new ArrayType(PrimitiveType.Real32, 4),
                    vra));
        }

        private void RewriteVsbox()
        {
            var vra = RewriteOperand(1);
            var vrt = RewriteOperand(0);
            m.Assign(vrt, m.Fn(vsbox, vra));
        }

        public void RewriteVsel()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(vsel,
                    vra,
                    vrb,
                    vrc));
        }

        public void RewriteVsldoi()
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(vsldoi,
                    vra,
                    vrb,
                    m.Slice(vrc, PrimitiveType.Byte, 0)));
        }

        public void RewriteVsx(IntrinsicProcedure intrinsic, DataType elementType)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(
                    intrinsic.MakeInstance(new ArrayType(elementType, vra.DataType.BitSize / elementType.BitSize)),
                    vra,
                    m.Slice(vrb, PrimitiveType.Byte)));
        }

        public void RewriteVsplti(PrimitiveType dtElem)
        {
            var sha = RewriteOperand(1);
            var vrt = RewriteOperand(0);
            var cElems = vrt.DataType.BitSize / dtElem.BitSize;
            var aType = new ArrayType(dtElem, cElems);
            m.Assign(vrt, m.Fn(vsplti.MakeInstance(aType), sha));
        }
        
        public void RewriteVsplt(PrimitiveType dtElem)
        {
            var opS = RewriteOperand(1);
            var opI = RewriteOperand(2);
            var opD = RewriteOperand(0);
            var cElems = opD.DataType.BitSize / dtElem.BitSize;
            var aType = new ArrayType(dtElem, cElems);
            m.Assign(opD, m.Fn(vsplt.MakeInstance(aType), opS, opI));
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
            var opS = RewriteOperand(1);
            var opI = RewriteOperand(2);
            var opD = RewriteOperand(0);
            m.Assign(opD, host.Intrinsic("__lvrx", false, opD.DataType, opS, opI));
        }

        private void RewriteMtvsrws()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, host.Intrinsic("__mtvsrws", false, opD.DataType, opS));
        }

        private void RewriteXsaddsp()
        {
            var opA = RewriteOperand(1);
            var opB = RewriteOperand(2);
            var opT = RewriteOperand(0);
            var tmpA = binder.CreateTemporary(PrimitiveType.Real64);
            var tmpB = binder.CreateTemporary(PrimitiveType.Real64);
            var tmpResult = binder.CreateTemporary(PrimitiveType.Real32);
            m.Assign(tmpA, m.Slice(opA, PrimitiveType.Real64, 64));
            m.Assign(tmpB, m.Slice(opB, PrimitiveType.Real64, 64));
            m.Assign(tmpResult, m.Convert(
                m.FAdd(tmpA, tmpB),
                PrimitiveType.Real64,
                PrimitiveType.Real32));
            m.Assign(opT, m.Dpb(opT, tmpResult, 64));
        }

        private void RewriteVsum4(IntrinsicProcedure intrinsic, PrimitiveType dtElemSrc, PrimitiveType dtElemDst)
        {
            var va = RewriteOperand(1);
            var vb = RewriteOperand(2);
            var vt = RewriteOperand(0);
            var dtSrc = MakeArrayType(va.DataType, dtElemSrc);
            var dtDst = MakeArrayType(vt.DataType, dtElemDst);
            m.Assign(
                vt,
                m.Fn(
                    intrinsic.MakeInstance(dtSrc, dtDst),
                    va,
                    vb));
        }
        private void RewriteVupk(IntrinsicProcedure intrinsic, PrimitiveType dtElemSrc, PrimitiveType dtElemDst)
        {
            var va = RewriteOperand(1);
            var vt = RewriteOperand(0);
            var dtSrc = MakeArrayType(va.DataType, dtElemSrc);
            var dtDst = MakeArrayType(vt.DataType, dtElemDst);
            m.Assign(
                vt,
                m.Fn(
                    intrinsic.MakeInstance(dtSrc, dtDst),
                    va));
        }

        // Very specific to XBOX 360

        private void RewriteVupkd3d()
        {
            var opD = RewriteOperand(0);
            var opA = RewriteOperand(1);
            var opB = RewriteOperand(instr.Operands[2]);
            m.Assign(opD, host.Intrinsic("__vupkd3d", false, opD.DataType, opA, opB));
        }
    }
}
