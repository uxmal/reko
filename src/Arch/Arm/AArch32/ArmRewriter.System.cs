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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private void RewriteBkpt()
        {
            if (instr.Operands.Length > 0)
            {
                m.SideEffect(host.Intrinsic("__breakpoint", false, VoidType.Instance, Operand(instr.Operands[0])));
            }
            else
            {
                m.SideEffect(host.Intrinsic("__breakpoint", false, VoidType.Instance));
            }
        }

        private void RewriteCdp(string name)
        {
            var ops = instr.Operands.Select(o => Operand(o)).ToArray();
            m.SideEffect(host.Intrinsic("__cdp", false, VoidType.Instance, ops));
        }

        private void RewriteClrex()
        {
            m.SideEffect(host.Intrinsic("__clrex", false, VoidType.Instance));
        }

        private void RewriteCps(string name)
        {
            m.SideEffect(host.Intrinsic(name, false, VoidType.Instance));
        }

        private void RewriteDmb()
        {
            var memBarrier = (BarrierOperand)instr.Operands[0];
            var name = $"__dmb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, false, VoidType.Instance));
        }

        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__dsb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, false, VoidType.Instance));
        }

        private void RewriteEret()
        {
            m.Return(0, 0);
        }

        private void RewriteHlt()
        {
            m.SideEffect(host.Intrinsic(
                "__hlt",
                false,
                new Core.Serialization.ProcedureCharacteristics
                {
                    Terminates = true
                },
                VoidType.Instance),
                InstrClass.Terminates);
        }

        private void RewriteHvc()
        {
            var n = Operand(instr.Operands[0]);
            m.SideEffect(host.Intrinsic("__hypervisor", false, VoidType.Instance, n));
        }

        private void RewriteIsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__isb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, false, VoidType.Instance));
        }

        private void RewriteLdc(string fnName)
        {
            var src2 = Operand(Src2());
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, src2);
            var  intrinsic = host.Intrinsic(fnName, false, PrimitiveType.Word32, 
                Operand(Src1()),
                tmp);
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            m.Assign(dst, intrinsic);
        }

        private void RewriteMcr()
        {
            var args = new List<Expression>();
            foreach (var op in instr.Operands)
            {
                args.Add(Operand(op));
            }
            var intrinsicCall = host.Intrinsic("__mcr", false, VoidType.Instance, args.ToArray());
            m.SideEffect(intrinsicCall);
        }

        private void RewriteMcrr()
        {
            var cop = Operand(instr.Operands[0]);
            var cmd = Operand(instr.Operands[1]);
            var cr = Operand(instr.Operands[4]);
            var rhi = ((RegisterOperand) instr.Operands[2]).Register;
            var rlo = ((RegisterOperand) instr.Operands[3]).Register;
            var nBits = (int) (rhi.BitSize + rlo.BitSize);
            var rseq = binder.EnsureSequence(PrimitiveType.CreateWord(nBits), rhi, rlo);
            m.Assign(rseq, host.Intrinsic("__mcrr", false, VoidType.Instance, cop, cmd, cr, rseq));
        }

        private void RewriteMrc()
        {
            int cArgs = 0;
            Expression? dst = null;
            var args = new List<Expression>();
            foreach (var op in instr.Operands)
            {
                var a = Operand(op);
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
            var intrinsicCall = host.Intrinsic("__mrc", false, dst!.DataType, args.ToArray());
            m.Assign(dst, intrinsicCall);
        }

        private void RewriteMrrc()
        {
            var cop = Operand(instr.Operands[0]);
            var cmd = Operand(instr.Operands[1]);
            var cr = Operand(instr.Operands[4]);
            var rhi = ((RegisterOperand) instr.Operands[2]).Register;
            var rlo = ((RegisterOperand) instr.Operands[3]).Register;
            var nBits = (int) (rhi.BitSize + rlo.BitSize);
            var rseq = binder.EnsureSequence(PrimitiveType.CreateWord(nBits), rhi, rlo);
            m.Assign(rseq, host.Intrinsic("__mrrc", false, rseq.DataType, cop, cmd, cr));
        }

        private void RewriteMrs()
        {
            var intrinsic = host.Intrinsic("__mrs", false, PrimitiveType.Word32, Operand(Src1()));
            m.Assign(Operand(Dst()), intrinsic);
        }

        private void RewriteMsr()
        {
            var intrinsic = host.Intrinsic("__msr", false, PrimitiveType.Word32, Operand(Dst()), Operand(Src1()));
            m.SideEffect(intrinsic);
        }

        private void RewriteSetend()
        {
            var endianness = (EndiannessOperand)instr.Operands[0];
            var intrisic = host.Intrinsic("__set_bigendian", false, VoidType.Instance, Constant.Bool(endianness.BigEndian));
            m.SideEffect(intrisic);
        }

        private void RewriteSmc()
        {
            var n = Operand(instr.Operands[0]);
            m.SideEffect(host.Intrinsic("__smc", false, VoidType.Instance, n));
        }

        private void RewriteStc(string name)
        {
            var intrinsic = host.Intrinsic("__stc", false, PrimitiveType.Word32,
                Operand(Dst()),
                Operand(Src1()),
                Operand(Src2()));
            m.SideEffect(intrinsic);
        }

        private void RewriteSvc()
        {
            var intrinsic = host.Intrinsic("__syscall", false, VoidType.Instance, Operand(Dst()));
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
            var intrinsic = host.Intrinsic("__syscall", false, PrimitiveType.Word32, trapNo);
            m.SideEffect(intrinsic);
        }

        private void RewriteWfi()
        {
            var intrinsic = host.Intrinsic("__wait_for_interrupt", false, VoidType.Instance);
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