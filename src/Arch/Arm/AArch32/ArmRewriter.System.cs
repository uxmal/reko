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
        private void RewriteBkpt()
        {
            if (instr.Operands.Length > 0)
            {
                m.SideEffect(host.Intrinsic("__breakpoint", true, VoidType.Instance, Operand(0)));
            }
            else
            {
                m.SideEffect(host.Intrinsic("__breakpoint", true, VoidType.Instance));
            }
        }

        private void RewriteCdp(string name)
        {
            var ops = Enumerable.Range(0, instr.Operands.Length)
                .Select(i => Operand(i))
                .ToArray();
            m.SideEffect(host.Intrinsic("__cdp", true, VoidType.Instance, ops));
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
            var name = $"__dmb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance));
        }

        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__dsb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance));
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
            m.SideEffect(host.Intrinsic("__hypervisor", true, VoidType.Instance, n));
        }

        private void RewriteIsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__isb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance));
        }

        private void RewriteLdc(string fnName)
        {
            var src2 = Operand(2);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, src2);
            var  intrinsic = host.Intrinsic(fnName, false, PrimitiveType.Word32, 
                Operand(1),
                tmp);
            var dst = Operand(0, PrimitiveType.Word32, true);
            m.Assign(dst, intrinsic);
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
            m.Assign(rseq, host.Intrinsic("__mcrr", true, VoidType.Instance, cop, cmd, cr, rseq));
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
            Debug.Assert(dst != null);
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
            m.Assign(rseq, host.Intrinsic("__mrrc", true, rseq.DataType, cop, cmd, cr));
        }

        private void RewriteMrs()
        {
            var intrinsic = host.Intrinsic("__mrs", true, PrimitiveType.Word32, Operand(1));
            m.Assign(Operand(0), intrinsic);
        }

        private void RewriteMsr()
        {
            var intrinsic = host.Intrinsic("__msr", true, PrimitiveType.Word32, Operand(0), Operand(1));
            m.SideEffect(intrinsic);
        }

        private void RewriteRfe(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
            m.Return(0, 0);
        }

        private void RewriteSetend()
        {
            var endianness = (EndiannessOperand)instr.Operands[0];
            var intrisic = host.Intrinsic("__set_bigendian", true, VoidType.Instance, Constant.Bool(endianness.BigEndian));
            m.SideEffect(intrisic);
        }

        private void RewriteSmc()
        {
            var n = Operand(0);
            m.SideEffect(host.Intrinsic("__smc", true, VoidType.Instance, n));
        }

        private void RewriteStc(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(
                intrinsic,
                Operand(0),
                Operand(1),
                Operand(2)));
        }

        private void RewriteSvc()
        {
            var intrinsic = host.Intrinsic("__syscall", true, VoidType.Instance, Operand(0));
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
            var trapNo = ((ImmediateOperand)instr.Operands[0]).Value;
            var intrinsic = host.Intrinsic("__syscall", true, PrimitiveType.Word32, trapNo);
            m.SideEffect(intrinsic);
        }

        private void RewriteWfi()
        {
            var intrinsic = host.Intrinsic("__wait_for_interrupt", true, VoidType.Instance);
            m.SideEffect(intrinsic);
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