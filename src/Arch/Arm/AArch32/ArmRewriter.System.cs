/*
* Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private static readonly StringType labelType = StringType.NullTerminated(PrimitiveType.Byte);

        private void RewriteBkpt()
        {
            if (instr.Operands.Length > 0)
            {
                m.SideEffect(m.Fn(bkpt_arg_intrinsic, Operand(0)));
            }
            else
            {
                m.SideEffect(m.Fn(bkpt_intrinsic));
            }
        }

        private void RewriteCdp(IntrinsicProcedure intrinsic)
        {
            var ops = Enumerable.Range(0, instr.Operands.Length)
                .Select(i => Operand(i))
                .ToArray();
            m.SideEffect(m.Fn(intrinsic, ops));
        }

        private void RewriteClrex()
        {
            m.SideEffect(m.Fn(clrex_intrinsic));
        }

        private void RewriteCps(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
        }

        private void RewriteDmb()
        {
            var memBarrier = (BarrierOperand)instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(dmb_intrinsic, Constant.String(label, labelType)));
        }

        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(dsb_intrinsic, Constant.String(label, labelType)));
        }

        private void RewriteEret()
        {
            m.Return(0, 0);
        }

        private void RewriteHlt()
        {
            m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
        }

        private void RewriteHvc()
        {
            var n = Operand(0);
            m.SideEffect(m.Fn(hvc_intrinsic, n));
        }

        private void RewriteIsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(isb_intrinsic, Constant.String(label, labelType)));
        }

        private void RewriteLdc(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(
                intrinsic,
                Operand(0),
                Operand(1),
                m.AddrOf(PrimitiveType.Ptr32, Operand(2))));
        }

        private void RewriteMcr(IntrinsicProcedure intrinsic)
        {
            var args = new List<Expression>();
            for (int i = 0; i < instr.Operands.Length; ++i)
            {
                args.Add(Operand(i));
            }
            var intrinsicCall = m.Fn(intrinsic, args.ToArray());
            m.SideEffect(intrinsicCall);
        }

        private void RewriteMcrr()
        {
            var cop = Operand(0);
            var cmd = Operand(1);
            var cr = Operand(4);
            var rhi = (RegisterStorage) instr.Operands[2];
            var rlo = (RegisterStorage) instr.Operands[3];
            var nBits = (int) (rhi.BitSize + rlo.BitSize);
            var rseq = binder.EnsureSequence(PrimitiveType.CreateWord(nBits), rhi, rlo);
            m.Assign(rseq, m.Fn(mcrr_intrinsic, cop, cmd, cr, rseq));
        }

        private void RewriteMrc(IntrinsicProcedure intrinsic)
        {
            int cArgs = 0;
            Expression? dst = null;
            var args = new List<Expression>();
            for (int i = 0; i < instr.Operands.Length; ++i)
            {
                var a = Operand(i);
                if (cArgs == 2)
                {
                    dst = a;
                }
                else
                {
                    args.Add(a);
                }
                ++cArgs;
            }
            Debug.Assert(dst is not null);
            var intrinsicCall = m.Fn(intrinsic, args.ToArray());
            m.Assign(dst, intrinsicCall);
        }

        private void RewriteMrrc()
        {
            var cop = Operand(0);
            var cmd = Operand(1);
            var cr = Operand(4);
            var rhi = (RegisterStorage) instr.Operands[2];
            var rlo = (RegisterStorage) instr.Operands[3];
            var nBits = (int) (rhi.BitSize + rlo.BitSize);
            var rseq = binder.EnsureSequence(PrimitiveType.CreateWord(nBits), rhi, rlo);
            m.Assign(rseq, m.Fn(mrrc_intrinsic.MakeInstance(rseq.DataType), cop, cmd, cr));
        }

        private void RewriteMrs()
        {
            m.Assign(Operand(0), m.Fn(mrs_intrinsic, Operand(1)));
        }

        private void RewriteMsr()
        {
            m.SideEffect(m.Fn(msr_intrinsic, Operand(0), Operand(1)));
        }

        private void RewriteRfe(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
            m.Return(0, 0);
        }

        private void RewriteSetend()
        {
            var endianness = (EndiannessOperand)instr.Operands[0];
            var intrisic = m.Fn(setend_intrinsic, Constant.Bool(endianness.BigEndian));
            m.SideEffect(intrisic);
        }

        private void RewriteSmc()
        {
            var n = Operand(0);
            m.SideEffect(m.Fn(smc_intrinsic, n));
        }

        private void RewriteStc(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(
                intrinsic,
                Operand(0),
                Operand(1),
                m.AddrOf(PrimitiveType.Ptr32, Operand(2))));
        }

        private void RewriteSvc()
        {
            var intrinsic = m.Fn(CommonOps.Syscall_1, Operand(0));
            m.SideEffect(intrinsic);
        }

        private void RewriteTrap()
        {
            throw new NotImplementedException();
            /*
	var trapNo = m.UInt32(instr.bytes[0]);
	var intrinsic = host.Intrinsic("__syscall", PrimitiveType.Word32, 1);
	m.AddArg(trapNo);
	m.SideEffect(m.Fn(intrinsic));
    */
        }

        private void RewriteUdf()
        {
            var trapNo = (Constant)instr.Operands[0];
            var intrinsic = m.Fn(CommonOps.Syscall_1, trapNo);
            m.SideEffect(intrinsic);
        }

        private void RewriteWfi()
        {
            m.SideEffect(m.Fn(wfi_intrinsic));
        }

        /*

string MemBarrierName(arm_mem_barrier barrier)
{

	switch (barrier)
	{
		//case ARM_MB_INVALID = 0,
		//case 	ARM_MB_RESERVED_0,
	case 	ARM_MB_OSHLD: return "oshld";
	case 	ARM_MB_OSHST: return "oshst";
	case 	ARM_MB_OSH: return "osh";
		//case 	ARM_MB_RESERVED_4,
	case 	ARM_MB_NSHLD: return "nshld";
	case 	ARM_MB_NSHST: return "nshst";
	case 	ARM_MB_NSH: return "nsh";
		//case 	ARM_MB_RESERVED_8,
	case 	ARM_MB_ISHLD: return "ishld";
	case 	ARM_MB_ISHST: return "ishst";
	case 	ARM_MB_ISH: return "ish";
		//case 	ARM_MB_RESERVED_12,
	case 	ARM_MB_LD: return "ld";
	case 	ARM_MB_ST: return "st";
	case 	ARM_MB_SY: return "sy";
	}
	return "NOT_IMPLEMENTED";
    */
    }
}