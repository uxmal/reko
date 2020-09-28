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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private void RewriteVecBinOp(Func<Expression, Expression, Expression> fn)
        {
            if (instr.Operands.Length == 3)
            {
                var src1 = Operand(Src1());
                var src2 = Operand(Src2());
                var dst = Operand(Dst(), PrimitiveType.Word32, true);
                m.Assign(dst, fn(src1, src2));
            }
            else
            {
                var src1 = Operand(Dst());
                var src2 = Operand(Src1());
                var dst = Operand(Dst(), PrimitiveType.Word32, true);
                m.Assign(dst, fn(src1, src2));
            }
        }

        private void RewriteVcmp()
        {
            var src1 = Operand(Dst(), PrimitiveType.Word32, true);
            var src2 = Operand(Src1());
            var fpscr = binder.EnsureFlagGroup(Registers.fpscr, 0xF0000000, "NZCV", PrimitiveType.Word32);
            m.Assign(fpscr, m.Cond(m.FSub(src1, src2)));
        }

        private void RewriteVcvt()
        {
            var src = Operand(Src1());
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            DataType dstType;
            switch (instr.vector_data)
            {
            case ArmVectorData.F32S32: dstType = PrimitiveType.Real32; break;
            case ArmVectorData.F64S32: dstType = PrimitiveType.Real64; break;
            case ArmVectorData.F64F32: dstType = PrimitiveType.Real64; break;
            default: NotImplementedYet(); return;
            }
            m.Assign(dst, m.Cast(dstType, src));
        }

        private void RewriteVcvtr()
        {
            var src = Operand(Src1());
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            DataType dstType;
            switch (instr.vector_data)
            {
            case ArmVectorData.S32F32: dstType = PrimitiveType.Int32; break;
            default: NotImplementedYet(); return;
            }
            m.Assign(dst, m.Cast(dstType, host.PseudoProcedure("trunc", src.DataType, src)));
        }

        private void RewriteVext()
        {
            var src1 = Operand(Src1());
            var src2 = Operand(Src2());
            var src3 = Operand(Src3());
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var intrinsic = host.PseudoProcedure("__vext", dst.DataType, src1, src2, src3);
            m.Assign(dst, intrinsic);
        }

        private void RewriteVldmia()
        {
            var rSrc = this.Operand(Dst(), PrimitiveType.Word32, true);
            var offset = 0;
            foreach (var r in ((MultiRegisterOperand)Src1()).GetRegisters())
            {
                var dst = Reg(r);
                Expression ea =
                    offset != 0
                    ? m.IAddS(rSrc, offset)
                    : rSrc;
                m.Assign(dst, m.Mem(dst.DataType, ea));
                offset += dst.DataType.Size;
            }
            if (instr.Writeback)
            {
                m.Assign(rSrc, m.IAddS(rSrc, offset));
            }
        }

        private void RewriteVldr()
        {
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var src = this.Operand(Src1());
            m.Assign(dst, src);
        }

        private void RewriteVmov()
        {
            //if (instr.ops.Length > 2) throw new NotImplementedException();
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var src = this.Operand(Src1());
            if (instr.vector_data != ArmVectorData.INVALID && !(src is ArrayAccess))
            {
                var dt = Arm32Architecture.VectorElementDataType(instr.vector_data);
                var dstType = Dst().Width;
                var srcType = Src1().Width;
                var srcElemSize = Arm32Architecture.VectorElementDataType(instr.vector_data);
                var celemSrc = dstType.BitSize / srcElemSize.BitSize;
                var arrDst = new ArrayType(dstType, celemSrc);

                if (Src1() is ImmediateOperand imm)
                {
                    if (dst.DataType.BitSize == 128)
                    {
                        src = m.Seq(src, src);
                    }
                    m.Assign(dst, src);
                    return;
                }
 
                var fname = $"__vmov_{VectorElementType(instr.vector_data)}";
                var ppp = host.PseudoProcedure(fname, Dst().Width, src);
                m.Assign(dst, m.Fn(ppp));
            }
            else
            {
                if (dst.DataType.BitSize != src.DataType.BitSize)
                {
                    src = m.Cast(dst.DataType, src);
                }
                m.Assign(dst, src);
            }
        }

        private void RewriteVmrs()
        {
            var expSysreg = Src1();
            var dst = Operand(Dst());
            RegisterStorage sysreg;
            if (expSysreg is RegisterOperand regSysreg)
            {
                sysreg = regSysreg.Register;
            }
            else
            {
                var nsysreg = ((ImmediateOperand) Src1()).Value.ToInt32();
                switch (nsysreg)
                {
                case 0: sysreg = Registers.fpsid; break;
                case 1:
                    sysreg = Registers.fpscr;
                    dst = NZCV();
                    break;
                case 5: sysreg = Registers.mvfr2; break;
                case 6: sysreg = Registers.mvfr1; break;
                case 7: sysreg = Registers.mvfr0; break;
                case 8: sysreg = Registers.fpexc; break;
                default: Invalid(); return;
                }
            }
            m.Assign(dst, binder.EnsureRegister(sysreg));
        }

        private void RewriteVmvn()
        {
            if (Src1() is ImmediateOperand)
            {
                RewriteVectorUnaryOp("__vmvn_imm_{0}");
            }
            else
            {
                RewriteVectorUnaryOp("__vmvn_{0}");
            }
        }

        private void RewriteVpop()
        {
            throw new NotImplementedException();
            /*
	int offset = 0;
	var begin = &instr.operands[0];
	var end = begin + instr.op_count;
            var sp = Reg(Registers.sp);
	var reg_size = type_sizes[(int)register_types[begin->reg]];
	for (var r = begin; r != end; ++r)
	{
		var dst = Reg(r->reg);
		Expression ea = offset != 0
			? m.IAdd(sp, m.Int32(offset))
			: sp;
		var dt = register_types[r->reg];
		m.Assign(dst, m.Mem(dt, ea));
		offset += type_sizes[(int)dt];
	}
	// Release space used by registers
	m.Assign(sp, m.IAdd(sp, m.Int32(instr.op_count * reg_size)));
    */
        }

        private void RewriteVpush()
        {
            throw new NotImplementedException();
            /*
            int offset = 0;
            var begin = &instr.operands[0];
            var end = begin + instr.op_count;
            var sp = Reg(ARM_REG_SP);
            var reg_size = type_sizes[(int)register_types[begin->reg]];
            // Allocate space for the registers
            m.Assign(sp, m.ISub(sp, m.Int32(instr.op_count * reg_size)));
            for (var r = begin; r != end; ++r)
            {
                var src = Reg(r->reg);
                Expression ea = offset != 0
                    ? m.IAdd(sp, m.Int32(offset))
                    : sp;
                var dt = register_types[r->reg];
                m.Assign(m.Mem(dt, ea), src);
                offset += type_sizes[(int)dt];
            }
            */
        }

        private void RewriteVstmia(bool add, bool writeback)
        {
            var rSrc = this.Operand(Dst(), PrimitiveType.Word32, true);
            var regs = ((MultiRegisterOperand)Src1()).GetRegisters().ToArray();
            int totalRegsize = regs.Length * regs[0].DataType.Size;
            int offset = add ? 0 : -totalRegsize;
            foreach (var r in regs)
            {
                var dst = Reg(r);
                Expression ea =
                    offset != 0
                    ? m.IAddS(rSrc, offset)
                    : rSrc;
                m.Assign(m.Mem(r.DataType, ea), dst);
                offset += r.DataType.Size;
            }
            if (instr.Writeback)
            {
                if (add)
                {
                    m.Assign(rSrc, m.IAddS(rSrc, totalRegsize));
                }
                else
                {
                    m.Assign(rSrc, m.ISubS(rSrc, totalRegsize));
                }
            }
        }

        private void RewriteVsqrt()
        {
            var fnname = instr.vector_data == ArmVectorData.F32 ? "sqrtf" : "sqrt";
            var dt = instr.vector_data == ArmVectorData.F32 ? PrimitiveType.Real32 : PrimitiveType.Real64;
            var src = this.Operand(Src1());
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var ppp = host.PseudoProcedure(fnname, dt, src);
            m.Assign(dst, ppp);
        }

        private void RewriteVdup()
        {
            var src = this.Operand(Src1());
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var dstType = Dst().Width;
            var srcType = Src1().Width;
            int elemBitSize = BitSize(instr.vector_data);
            var celem = dstType.BitSize / elemBitSize;
            var arrType = new ArrayType(srcType, celem);
            var fnName = $"__vdup_{elemBitSize}";
            var intrinsic = host.PseudoProcedure(fnName, arrType, src);
            m.Assign(dst, intrinsic);
        }

        private void RewriteVmul()
        {
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var dstType = Dst().Width;
            var srcType = Src1().Width;
            var srcElemSize = Arm32Architecture.VectorElementDataType(instr.vector_data);
            var celemSrc = srcType.BitSize / srcElemSize.BitSize;
            var arrSrc = new ArrayType(srcType, celemSrc);
            var arrDst = new ArrayType(dstType, celemSrc);
            var fnName = $"__vmul_{VectorElementType(instr.vector_data)}";
            var intrinsic = host.PseudoProcedure(fnName, arrDst, src1, src2);
            m.Assign(dst, m.Fn(intrinsic));
        }

        private void RewriteVectorUnaryOp(string fnNameFormat)
        {
            RewriteVectorUnaryOp(fnNameFormat, instr.vector_data);
        }

        private void RewriteVectorUnaryOp(string fnNameFormat, ArmVectorData elemType)
        {
            var src1 = this.Operand(Src1());
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var dstType = Dst().Width;
            var srcType = Src1().Width;
            var srcElemSize = Arm32Architecture.VectorElementDataType(elemType);
            var celemSrc = srcType.BitSize / srcElemSize.BitSize;
            var arrSrc = new ArrayType(srcType, celemSrc);
            var arrDst = new ArrayType(dstType, celemSrc);
            var fnName = string.Format(fnNameFormat, VectorElementType(elemType));
            var intrinsic = host.PseudoProcedure(fnName, arrDst, src1);
            m.Assign(dst, intrinsic);
        }

        private void RewriteVectorBinOp(string fnNameFormat)
        {
            RewriteVectorBinOp(fnNameFormat, instr.vector_data);
        }

        private void RewriteVectorBinOp(string fnNameFormat, ArmVectorData elemType)
        {
            var src1 = this.Operand(Src1());
            var src2 = this.Operand(Src2());
            var dst = this.Operand(Dst(), PrimitiveType.Word32, true);
            var dstType = Dst().Width;
            var srcType = Src1().Width;
            var srcElemSize = Arm32Architecture.VectorElementDataType(elemType);
            //$BUG: some instructions are returned with srcElemnSize == 0!
            var celemSrc = srcType.BitSize / (srcElemSize.BitSize != 0 ? srcElemSize.BitSize : 8);
            var arrSrc = new ArrayType(srcType, celemSrc);
            var arrDst = new ArrayType(dstType, celemSrc);
            var fnName = string.Format(fnNameFormat, VectorElementType(elemType));
            var intrinsic = host.PseudoProcedure(fnName, arrDst, src1, src2);
            m.Assign(dst, intrinsic);
        }

        private void RewriteVstr()
        {
            var src = this.Operand(Dst(), PrimitiveType.Word32, true);
            var dst = this.Operand(Src1());
            m.Assign(dst, src);
        }

        string VectorElementType(ArmVectorData elemType)
        {
            switch (elemType)
            {
            case ArmVectorData.I8: return "i8";
            case ArmVectorData.S8: return "s8";
            case ArmVectorData.U8: return "u8";
            case ArmVectorData.F16: return "f16";
            case ArmVectorData.I16: return "i16";
            case ArmVectorData.S16: return "s16";
            case ArmVectorData.U16: return "u16";
            case ArmVectorData.F32: return "f32";
            case ArmVectorData.I32: return "i32";
            case ArmVectorData.S32: return "s32";
            case ArmVectorData.U32: return "u32";
            case ArmVectorData.F64: return "f64";
            case ArmVectorData.I64: return "i64";
            case ArmVectorData.S64: return "s64";
            case ArmVectorData.U64: return "u64";
            default: NotImplementedYet(); return "(NYI)";
            }
        }

    }
}