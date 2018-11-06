/*
* Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private void RewriteBkpt()
        {
            if (instr.ops.Length > 0)
            {
                m.SideEffect(host.PseudoProcedure("__breakpoint", VoidType.Instance, Operand(instr.ops[0])));
            }
            else
            {
                m.SideEffect(host.PseudoProcedure("__breakpoint", VoidType.Instance));
            }
        }

        private void RewriteCdp(string name)
        {
            throw new NotImplementedException();
            //auto cdp = host.PseudoProcedure(name, VoidType.Instance, instr->detail->arm.op_count);
            //auto begin = &instr->detail->arm.operands[0];
            //auto end = begin + instr->detail->arm.op_count;
            //for (auto op = begin; op != end; ++op)
            //{
            //	m.AddArg(Operand(*op));
            //}
            //m.SideEffect(m.Fn(cdp));
        }

        private void RewriteCps()
        {
            m.SideEffect(host.PseudoProcedure("__cps_id", VoidType.Instance));
        }

        private void RewriteDmb()
        {
            var memBarrier = (BarrierOperand)instr.ops[0];
            var name = $"__dmb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.PseudoProcedure(name, VoidType.Instance));
        }

        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.ops[0];
            var name = $"__dsb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.PseudoProcedure(name, VoidType.Instance));
        }

        private void RewriteEret()
        {
            rtlClass = RtlClass.Transfer;
            m.Return(0, 0);
        }

        private void RewriteHvc()
        {
            var n = Operand(instr.ops[0]);
            m.SideEffect(host.PseudoProcedure("__hypervisor", VoidType.Instance, n));
        }

        private void RewriteLdc(string fnName)
        {
            var src2 = Operand(Src2());
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, src2);
            var  intrinsic = host.PseudoProcedure(fnName, PrimitiveType.Word32, 
                Operand(Src1()),
                tmp);
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            m.Assign(dst, intrinsic);
        }

        private void RewriteMcr()
        {
            var args = new List<Expression>();
            foreach (var op in instr.ops)
            {
                args.Add(Operand(op));
            }
            var intrinsicCall = host.PseudoProcedure("__mcr", VoidType.Instance, args.ToArray());
            m.SideEffect(intrinsicCall);
        }

        private void RewriteMrc()
        {
            int cArgs = 0;
            Expression dst = null;
            var args = new List<Expression>();
            foreach (var op in instr.ops)
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
            var intrinsicCall = host.PseudoProcedure("__mrc", VoidType.Instance, args.ToArray());
            m.Assign(dst, intrinsicCall);
        }

        private void RewriteMrs()
        {
            var ppp = host.PseudoProcedure("__mrs", PrimitiveType.Word32, Operand(Src1()));
            m.Assign(Operand(Dst()), ppp);
        }

        private void RewriteMsr()
        {
            var intrinsic = host.PseudoProcedure("__msr", PrimitiveType.Word32, Operand(Dst()), Operand(Src1()));
            m.SideEffect(intrinsic);
        }

        private void RewriteSetend()
        {
            var endianness = (EndiannessOperand)instr.ops[0];
            var intrisic = host.PseudoProcedure("__set_bigendian", VoidType.Instance, Constant.Bool(endianness.BigEndian));
            m.SideEffect(intrisic);
        }

        private void RewriteSmc()
        {
            var n = Operand(instr.ops[0]);
            m.SideEffect(host.PseudoProcedure("__smc", VoidType.Instance, n));
        }

        private void RewriteStc(string name)
        {
            var intrinsic = host.PseudoProcedure("__stc", PrimitiveType.Word32,
                Operand(Dst()),
                Operand(Src1()),
                Operand(Src2()));
            m.SideEffect(intrinsic);
        }

        private void RewriteSvc()
        {
            this.rtlClass = RtlClass.Transfer | RtlClass.Call;
            var intrinsic = host.PseudoProcedure("__syscall", VoidType.Instance, Operand(Dst()));
            m.SideEffect(intrinsic);
        }

        private void RewriteTrap()
        {
            throw new NotImplementedException();
            /*
	var trapNo = m.UInt32(instr.bytes[0]);
	var ppp = host.PseudoProcedure("__syscall", PrimitiveType.Word32, 1);
	m.AddArg(trapNo);
	m.SideEffect(m.Fn(ppp));
    */
        }

        private void RewriteUdf()
        {
            var trapNo = ((ImmediateOperand)instr.ops[0]).Value;
            var ppp = host.PseudoProcedure("__syscall", PrimitiveType.Word32, trapNo);
            m.SideEffect(ppp);
        }

        private void RewriteWfi()
        {
            var intrinsic = host.PseudoProcedure("__wait_for_interrupt", VoidType.Instance);
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