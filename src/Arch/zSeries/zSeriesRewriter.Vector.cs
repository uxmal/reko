#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Types;
using System.Linq;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteVectorInstruction(IntrinsicProcedure intrinsic)
        {
            var args = Enumerable.Range(1, instr.Operands.Length - 1)
                .Select(i => Op(i, instr.Operands[i].Width))
                .ToArray();
            var dst = Op(0, instr.Operands[0].Width);
            if (intrinsic.IsGeneric)
            {
                m.Assign(dst, m.Fn(intrinsic.MakeInstance(dst.DataType), args));
            }
            else
            {
                m.Assign(dst, m.Fn(intrinsic, args));
            }
            if (instr.SetConditionCode)
            {
                this.SetCc(dst);
            }
        }

        private void RewriteVectorInstruction3Elem(IntrinsicProcedure genericIntrinsic, bool shrink = false)
        {
            var dtElem = instr.ElementSize;
            if (dtElem is null)
            {
                host.Error(instr.Address, "Expected a valid vector element size for {0}.", instr);
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var src1 = Op(1, instr.Operands[1].Width);
            var src2 = Op(2, instr.Operands[2].Width);
            var dst = Op(0, instr.Operands[0].Width);
            var arraySrc = new ArrayType(dtElem, src1.DataType.BitSize / dtElem.BitSize);
            var arg1 = binder.CreateTemporary(arraySrc);
            var arg2 = binder.CreateTemporary(arraySrc);
            var intrinsic = shrink
                ? genericIntrinsic.MakeInstance(arraySrc, new ArrayType(dtElem, src1.DataType.BitSize / (2 * dtElem.BitSize)))
                : genericIntrinsic.MakeInstance(arraySrc);
            m.Assign(arg1, src1);
            m.Assign(arg2, src2);
            m.Assign(dst, m.Fn(intrinsic, arg1, arg2));
        }

        private void RewriteVectorInstruction4Elem(IntrinsicProcedure genericIntrinsic, bool doubleElements)
        {
            var dtElem = instr.ElementSize;
            if (dtElem is null)
            {
                host.Error(instr.Address, "Expected a valid vector element size for {0}.", instr);
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }

            var src1 = Op(1, instr.Operands[1].Width);
            var src2 = Op(2, instr.Operands[2].Width);
            var src3 = Op(3, instr.Operands[3].Width);
            var dst = Op(0, instr.Operands[0].Width);
            var arraySrc = new ArrayType(dtElem, src1.DataType.BitSize / dtElem.BitSize);
            var arrayDst = arraySrc;
            var intrinsic = genericIntrinsic.GetGenericArguments().Length == 2
                ? genericIntrinsic.MakeInstance(arraySrc, arrayDst)
                : genericIntrinsic.MakeInstance(arraySrc);
            var arg1 = binder.CreateTemporary(arraySrc);
            var arg2 = binder.CreateTemporary(arraySrc);
            var arg3 = binder.CreateTemporary(arraySrc);
            m.Assign(arg1, src1);
            m.Assign(arg2, src2);
            m.Assign(arg3, src3);
            m.Assign(dst, m.Fn(intrinsic, arg1, arg2, arg3));
        }
    }
}
