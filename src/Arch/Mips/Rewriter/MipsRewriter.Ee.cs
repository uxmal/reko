#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Arch.Mips.Rewriter;

public partial class MipsRewriter
{
    private void RewriteEfu2(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        // VDIV/VSQRT/RSQRT write their result into the Q register.
        Debug.Assert(arch.q is not null);
        var q = binder.EnsureRegister(arch.q);
        var src1 = RewriteOperand(instr, 1);
        var src2 = RewriteOperand(instr, 2);
        m.Assign(q, m.Fn(intrinsic, src1, src2));
    }

    private void RewriteEfu1(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        Debug.Assert(arch.q is not null);
        var q = binder.EnsureRegister(arch.q);
        var src = RewriteOperand(instr, 0);
        m.Assign(q, m.Fn(intrinsic, src));
    }

    private void RewriteFpuCmp(MipsInstruction instr, BinaryOperator cmpOp)
    {
        if (instr.Operands.Length == 3)
        {
            var src1 = RewriteOperand(instr, 1);
            var src2 = RewriteOperand(instr, 2);
            var dst = RewriteOperand(instr, 0);
            m.Assign(dst, m.Bin(cmpOp, src1, src2));
            return;
        }
        else
        {
            var src1 = RewriteOperand(instr, 0);
            var src2 = RewriteOperand(instr, 1);
            var cc = binder.EnsureRegister(Registers.cc1);
            m.Assign(cc, m.Bin(cmpOp, src1, src2));
        }
    }

    private void RewriteLqc2(MipsInstruction instr)
    {
        var opSrc = RewriteOperand(instr, 1);
        var opDst = RewriteOperand(instr, 0);
        m.Assign(opDst, opSrc);
    }

    private void RewriteMf0(MipsInstruction instr)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(intrinsics.mf0));
    }

    private void RewriteMfsa(MipsInstruction instr)
    {
        Debug.Assert(arch.sa is not null);
        AssignS(
            RewriteOperand(instr, 0),
            binder.EnsureRegister(arch.sa));
    }

    private void RewriteMtsa(MipsInstruction instr)
    {
        Debug.Assert(arch.sa is not null);
        var tmp = binder.CreateTemporary(arch.sa.DataType);
        m.Assign(tmp, RewriteOperand0(instr, 0, tmp.DataType));
        m.Assign(binder.EnsureRegister(arch.sa), tmp);
    }

    private void RewritePcpyh(MipsInstruction instr)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyh,
                RewriteOperand0(instr, 1)));
    }

    private void RewritePcpyld(MipsInstruction instr)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyld,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewritePcpyud(MipsInstruction instr)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyud,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewriteParallelBinary(MipsInstruction instr, IntrinsicProcedure fn)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                fn,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }


    private void RewriteParallelBinary(
        MipsInstruction instr,
        IntrinsicProcedure fn,
        PrimitiveType elementType)
    {
        var at = GetArrayType(elementType);
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                fn.MakeInstance(at),
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewriteParallelShift(MipsInstruction instr, IntrinsicProcedure shift, PrimitiveType elementType)
    {
        var at = GetArrayType(elementType);
        var sht = instr.Operands[2].DataType;
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                shift.MakeInstance(at, sht),
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewriteQfsrv(MipsInstruction instr)
    {
        Debug.Assert(arch.sa is not null);
        var sa = binder.EnsureRegister(arch.sa);
        var tmp = binder.CreateTemporary(PrimitiveType.Word256);
        m.Assign(tmp, m.Seq(this.RewriteOperand(instr, 1), this.RewriteOperand(instr, 2)));
        m.Assign(tmp, m.Shr(tmp, sa)); //$REVIEW: manual is unclear on whether this is 
                                       // arithmetic or logical shift.
        m.Assign(this.RewriteOperand(instr, 0), m.Slice(tmp, PrimitiveType.Word128));
    }

    private void RewriteQmfc2(MipsInstruction instr)
    {
        m.Assign(RewriteOperand(instr, 0), RewriteOperand(instr, 1));
    }

    private void RewriteQmtc2(MipsInstruction instr)
    {
        m.Assign(RewriteOperand(instr, 1), RewriteOperand(instr, 0));
    }

    private void RewriteSimd(
        MipsInstruction instr,
        IntrinsicProcedure simd,
        PrimitiveType elementType)
    {
        var at = GetArrayType(elementType);
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(
                simd.MakeInstance(at),
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private Expression Fsf(MipsInstruction instr, int iop)
    {
        var tmp = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(tmp, m.Slice(RewriteOperand0(instr, iop), tmp.DataType));
        return tmp;
    }

    private static ArrayType GetArrayType(PrimitiveType et)
    {
        var cElems = 128 / et.BitSize;
        return new ArrayType(et, cElems);
    }

    private void RewriteViBinop(MipsInstruction instr, BinaryOperator op)
    {
        var dst = RewriteOperand(instr, 0);
        var src1 = RewriteOperand(instr, 1);
        var src2 = RewriteOperand(instr, 2);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteVcallms(MipsInstruction instr)
    {
        var imm = RewriteOperand0(instr, 0);
        m.SideEffect(m.Fn(intrinsics.vcallms, imm));
    }


    private void RewriteVclipw(MipsInstruction instr)
    {
        var src1 = RewriteOperand(instr, 0);
        var src2 = RewriteOperand(instr, 1);
        m.SideEffect(m.Fn(intrinsics.vclipw, src1, src2));
    }

    private void RewriteVmfir(MipsInstruction instr)
    {
        var vf = RewriteOperand(instr, 0);
        var vi = RewriteOperand(instr, 1);
        m.Assign(vf, vi);
    }

    private void RewriteVmr32(MipsInstruction instr)
    {
        var vf = RewriteOperand(instr, 0);
        var vi = RewriteOperand(instr, 1);
        m.Assign(vf, m.Fn(intrinsics.vmr32, vi));
    }

    private void RewriteVmtir(MipsInstruction instr)
    {
        var vi = RewriteOperand(instr, 0);
        var vf = Fsf(instr, 1);
        m.Assign(vi, vf);
    }

    private void RewriteVrGet(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        var vi = RewriteOperand(instr, 0);
        m.Assign(vi, m.Fn(intrinsic));
    }

    private void RewriteVrSet(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        var vi = RewriteOperand0(instr, 0);
        m.SideEffect(m.Fn(intrinsic, vi));
    }

    private void RewriteVuAccOp2(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        Debug.Assert(arch.acc is not null);
        var acc = binder.EnsureRegister(arch.acc);
        var src1 = RewriteOperand(instr, 1);
        var src2 = RewriteOperand(instr, 2);
        m.Assign(acc, m.Fn(intrinsic, acc, src1, src2));
    }

    /// <summary>
    /// The "A-forms" accumulate into ACC; conversions and absolute value
    /// write a VF register selected by the ft field.
    /// </summary>
    private void RewriteVuAccOp(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        string op = instr.Mnemonic.ToString().ToLowerInvariant();
        switch (instr.Mnemonic)
        {
        case Mnemonic.vaddax:
        case Mnemonic.vadday:
        case Mnemonic.vaddaz:
        case Mnemonic.vaddaw:
        case Mnemonic.vsubax:
        case Mnemonic.vsubay:
        case Mnemonic.vsubaz:
        case Mnemonic.vsubaw:
        case Mnemonic.vmaddax:
        case Mnemonic.vmadday:
        case Mnemonic.vmaddaz:
        case Mnemonic.vmaddaw:
        case Mnemonic.vmsubax:
        case Mnemonic.vmsubay:
        case Mnemonic.vmsubaz:
        case Mnemonic.vmsubaw:
        case Mnemonic.vmulax:
        case Mnemonic.vmulay:
        case Mnemonic.vmulaz:
        case Mnemonic.vmulaw:
        case Mnemonic.vmulaq:
        case Mnemonic.vmulai:
        case Mnemonic.vaddaq:
        case Mnemonic.vmaddaq:
        case Mnemonic.vaddai:
        case Mnemonic.vmaddai:
        case Mnemonic.vsubaq:
        case Mnemonic.vmsubaq:
        case Mnemonic.vsubai:
        case Mnemonic.vmsubai:
        case Mnemonic.vadda:
        case Mnemonic.vmadda:
        case Mnemonic.vmula:
        case Mnemonic.vsuba:
        case Mnemonic.vmsuba:
        {
            Debug.Assert(arch.acc is not null);
            var acc = binder.EnsureRegister(arch.acc);
            var src1 = RewriteOperand(instr, 1);
            var src2 = RewriteOperand(instr, 2);
            m.Assign(acc, m.Fn(intrinsic, src1, src2));
            break;
        }
        case Mnemonic.vopmula:
        {
            Debug.Assert(arch.acc is not null);
            var acc = binder.EnsureRegister(arch.acc);
            var src1 = RewriteOperand(instr, 1);
            var src2 = RewriteOperand(instr, 2);
            m.Assign(acc, m.Fn(intrinsic, src1, src2));
            break;
        }
        case Mnemonic.vopmsub:
        {
            Debug.Assert(arch.acc is not null);
            var dst = RewriteOperand(instr, 0);
            var accPre = binder.EnsureRegister(arch.acc);
            var src1 = RewriteOperand(instr, 1);
            var src2 = RewriteOperand(instr, 2);
            var intrinsicVop = new IntrinsicBuilder("__ee_vopmsub", true)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .Returns("T")
                .MakeInstance(PrimitiveType.Word128);
            m.Assign(dst, m.Fn(intrinsicVop, accPre, src1, src2));
            break;
        }
        default:
        {
            // Conversions (VITOF/VFTOI), VABS, VMOVE, VMR32: unary ops
            // whose destination is the VF in the ft field.
            var dst = RewriteOperand(instr, 0);
            var src = RewriteOperand(instr, 1);
            m.Assign(dst, m.Fn(intrinsic, src));
            break;
        }
        }
    }


    private void RewriteVuLoadOp(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        // VLQI/VSQI/VLQD/VSQD/VILWR/VISWR are memory operations that are
        // only meaningful inside the VU itself; when they appear in macro
        // mode we model them as opaque side effects.
        var op1 = RewriteOperand0(instr, 0);
        var op2 = RewriteOperand0(instr, 1);
        var mask = Constant.Int32(instr.LaneMask);
        m.Assign(op1, m.Fn(intrinsic, op2, mask));
    }

    private void RewriteVuStoreOp(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        // VLQI/VSQI/VLQD/VSQD/VILWR/VISWR are memory operations that are
        // only meaningful inside the VU itself; when they appear in macro
        // mode we model them as opaque side effects.
        var op1 = RewriteOperand0(instr, 0);
        var op2 = RewriteOperand0(instr, 1);
        var mask = Constant.Int32(instr.LaneMask);
        m.SideEffect(m.Fn(intrinsic, op1, op2, mask));
    }

    private void RewriteVuMove(MipsInstruction instr)
    {
        var src = RewriteOperand(instr, 1);
        var dst = RewriteOperand(instr, 0);
        m.Assign(dst, src);
    }

    /// <summary>
    /// Vector arithmetic forms; the destination is a VF register (or ACC
    /// for VOPMSUB), sources are VF/Q/I registers.
    /// </summary>
    private void RewriteVuVectorOp(MipsInstruction instr, IntrinsicProcedure intrinsic)
    {
        int arity = instr.Operands.Length;
        Debug.Assert(arity >= 3);   // fd, fs, ft/q/i + mask
        var dst = RewriteOperand(instr, 0);
        var src1 = RewriteOperand(instr, 1);
        var src2 = RewriteOperand(instr, 2);
        string op = instr.Mnemonic.ToString().ToLowerInvariant();
        m.Assign(dst, m.Fn(intrinsic, src1, src2));
    }


    public void DELETE_ME_GetEnumerator2()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            this.iclass = instr.InstructionClass;
            rtlInstructions.Clear();
            switch (instr.Mnemonic)
            {
            default:
                host.Error(
                    instr.Address,
                    string.Format("EE instruction '{0}' is not supported yet.", instr));
                EmitUnitTest(instr);
                goto case Mnemonic.illegal;
            case Mnemonic.illegal:
                iclass = InstrClass.Invalid; m.Invalid(); break;
            case Mnemonic.nop: m.Nop(); break;

                // Core ALU.
                /*
                case Mnemonic.add:
                case Mnemonic.addi:
                case Mnemonic.addiu:
                case Mnemonic.addu: RewriteAdd(instr, PrimitiveType.Word32); break;
                case Mnemonic.and:
                case Mnemonic.andi: RewriteAnd(instr); break;
                case Mnemonic.nor: RewriteNor(instr); break;
                case Mnemonic.or:
                case Mnemonic.ori: RewriteOr(instr); break;
                case Mnemonic.xor:
                case Mnemonic.xori: RewriteXor(instr); break;
                case Mnemonic.slt:
                case Mnemonic.slti: RewriteScc(instr, m.Lt); break;
                case Mnemonic.sltiu:
                case Mnemonic.sltu: RewriteScc(instr, m.Ult); break;
                case Mnemonic.sub:
                case Mnemonic.subu: RewriteSub(instr); break;
                case Mnemonic.lui: RewriteLui(instr); break;
                case Mnemonic.movz: RewriteMovCc(instr, m.Eq0); break;
                case Mnemonic.movn: RewriteMovCc(instr, m.Ne0); break;
                case Mnemonic.sll:
                case Mnemonic.sllv: RewriteShift(instr, m.Shl, 0, PrimitiveType.Word32); break;
                case Mnemonic.srl:
                case Mnemonic.srlv: RewriteShift(instr, m.Shr, 0, PrimitiveType.Word32); break;
                case Mnemonic.sra:
                case Mnemonic.srav: RewriteShift(instr, m.Sar, 0, PrimitiveType.Word32); break;

                // MIPS III 64-bit operations.
                case Mnemonic.dadd:
                case Mnemonic.daddi:
                case Mnemonic.daddiu:
                case Mnemonic.daddu: RewriteAdd(instr, PrimitiveType.Word64); break;
                case Mnemonic.dsub:
                case Mnemonic.dsubu: RewriteSub64(instr); break;
                case Mnemonic.dsll:
                case Mnemonic.dsllv: RewriteShift(instr, m.Shl, 0, PrimitiveType.Word64); break;
                case Mnemonic.dsrl:
                case Mnemonic.dsrlv: RewriteShift(instr, m.Shr, 0, PrimitiveType.Word64); break;
                case Mnemonic.dsra:
                case Mnemonic.dsrav: RewriteShift(instr, m.Sar, 0, PrimitiveType.Word64); break;
                case Mnemonic.dsll32: RewriteShift(instr, m.Shl, 32, PrimitiveType.Word64); break;
                case Mnemonic.dsrl32: RewriteShift(instr, m.Shr, 32, PrimitiveType.Word64); break;
                case Mnemonic.dsra32: RewriteShift(instr, m.Sar, 32, PrimitiveType.Word64); break;

                // Multiply/divide accumulators; the "1" variants use the
                // second HI/LO pair of the EE core.
                case Mnemonic.mult: RewriteMul(instr, arch.hi, arch.lo, Operator.SMul); break;
                case Mnemonic.multu: RewriteMul(instr, arch.hi, arch.lo, Operator.UMul); break;
                case Mnemonic.mult1: RewriteMul(instr, arch.hi1, arch.lo1, Operator.SMul); break;
                case Mnemonic.multu1: RewriteMul(instr, arch.hi1, arch.lo1, Operator.UMul); break;
                case Mnemonic.div: RewriteDiv(instr, arch.hi, arch.lo, Operator.SDiv, Operator.SMod); break;
                case Mnemonic.divu: RewriteDiv(instr, arch.hi, arch.lo, Operator.UDiv, Operator.UMod); break;
                case Mnemonic.div1: RewriteDiv(instr, arch.hi1, arch.lo1, Operator.SDiv, Operator.SMod); break;
                case Mnemonic.divu1: RewriteDiv(instr, arch.hi1, arch.lo1, Operator.UDiv, Operator.UMod); break;
                case Mnemonic.mfhi: RewriteMoveFrom(instr, arch.hi); break;
                case Mnemonic.mthi: RewriteMoveTo(instr, arch.hi); break;
                case Mnemonic.mflo: RewriteMoveFrom(instr, arch.lo); break;
                case Mnemonic.mtlo: RewriteMoveTo(instr, arch.lo); break;
                case Mnemonic.mfhi1: RewriteMoveFrom(instr, arch.hi1); break;
                case Mnemonic.mthi1: RewriteMoveTo(instr, arch.hi1); break;
                case Mnemonic.mflo1: RewriteMoveFrom(instr, arch.lo1); break;
                case Mnemonic.mtlo1: RewriteMoveTo(instr, arch.lo1); break;

                // Loads and stores.
                case Mnemonic.lb: RewriteLoad(instr, PrimitiveType.SByte); break;
                case Mnemonic.lbu: RewriteLoad(instr, PrimitiveType.Byte); break;
                case Mnemonic.lh: RewriteLoad(instr, PrimitiveType.Int16); break;
                case Mnemonic.lhu: RewriteLoad(instr, PrimitiveType.Word16); break;
                case Mnemonic.lw: RewriteLoad(instr, PrimitiveType.Word32); break;
                case Mnemonic.lwu: RewriteLoad(instr, PrimitiveType.UInt32); break;
                case Mnemonic.ld: RewriteLoad(instr, PrimitiveType.Word64); break;
                case Mnemonic.lq: RewriteQuadLoadStore(instr, true); break;
                case Mnemonic.sb:
                case Mnemonic.sh:
                case Mnemonic.sw: RewriteStore(instr, 32); break;
                case Mnemonic.sd: RewriteStore(instr, 64); break;
                case Mnemonic.sq: RewriteQuadLoadStore(instr, false); break;
                case Mnemonic.lwl: RewriteLwlLwr(instr, intrinsics.lwl); break;
                case Mnemonic.lwr: RewriteLwlLwr(instr, intrinsics.lwr); break;
                case Mnemonic.swl: RewriteSwlSwr(instr, intrinsics.swl); break;
                case Mnemonic.swr: RewriteSwlSwr(instr, intrinsics.swr); break;
                case Mnemonic.ldl: RewriteLdlLdr(instr, intrinsics.ldl); break;
                case Mnemonic.ldr: RewriteLdlLdr(instr, intrinsics.ldr); break;
                case Mnemonic.sdl: RewriteSdlSdr(instr, intrinsics.sdl); break;
                case Mnemonic.sdr: RewriteSdlSdr(instr, intrinsics.sdr); break;
                case Mnemonic.lwc1: RewriteLoadFp(instr); break;
                case Mnemonic.swc1: RewriteStoreFp(instr); break;
                case Mnemonic.cache:
                case Mnemonic.pref: RewriteCache(instr); break;

                // Branches and jumps.
                case Mnemonic.j: RewriteJump(instr); break;
                case Mnemonic.jal: RewriteJal(instr); break;
                case Mnemonic.jr: RewriteJr(instr); break;
                case Mnemonic.jalr: RewriteJalr(instr); break;
                case Mnemonic.beq:
                case Mnemonic.bne:
                case Mnemonic.beql:
                case Mnemonic.bnel: RewriteBranch2(instr); break;
                case Mnemonic.blez: RewriteBranchCmp0(instr, m.Le, false); break;
                case Mnemonic.bgtz: RewriteBranchCmp0(instr, m.Gt, false); break;
                case Mnemonic.blezl: RewriteBranchCmp0(instr, m.Le, false); break;
                case Mnemonic.bgtzl: RewriteBranchCmp0(instr, m.Gt, false); break;
                case Mnemonic.bltz: RewriteBranchCmp0(instr, m.Lt, false); break;
                case Mnemonic.bgez: RewriteBranchCmp0(instr, m.Ge, false); break;
                case Mnemonic.bltzl: RewriteBranchCmp0(instr, m.Lt, false); break;
                case Mnemonic.bgezl: RewriteBranchCmp0(instr, m.Ge, false); break;
                case Mnemonic.bltzal: RewriteBranchCmp0(instr, m.Lt, true); break;
                case Mnemonic.bltzall: RewriteBranchCmp0(instr, m.Lt, true); break;
                case Mnemonic.bgezall: RewriteBranchCmp0(instr, m.Ge, true); break;
                case Mnemonic.bal: RewriteBal(instr); break;
                case Mnemonic.bgezal: RewriteBgezal(instr); break;

                // Traps and exceptions.
                case Mnemonic.tge: RewriteTrap(instr, m.Ge); break;
                case Mnemonic.tgeu: RewriteTrap(instr, m.Uge); break;
                case Mnemonic.tgei: RewriteTrapi(instr, m.Ge); break;
                case Mnemonic.tgeiu: RewriteTrapi(instr, m.Uge); break;
                case Mnemonic.tlt: RewriteTrap(instr, m.Lt); break;
                case Mnemonic.tltu: RewriteTrap(instr, m.Ult); break;
                case Mnemonic.tlti: RewriteTrapi(instr, m.Lt); break;
                case Mnemonic.tltiu: RewriteTrapi(instr, m.Ult); break;
                case Mnemonic.teq: RewriteTrap(instr, m.Eq); break;
                case Mnemonic.teqi: RewriteTrapi(instr, m.Eq); break;
                case Mnemonic.tne: RewriteTrap(instr, m.Ne); break;
                case Mnemonic.tnei: RewriteTrapi(instr, m.Ne); break;
                case Mnemonic.syscall: RewriteSyscall(instr); break;
                case Mnemonic.@break: RewriteBreakInstr(instr); break;
                case Mnemonic.sync: m.SideEffect(m.Fn(intrinsics.sync)); break;
                case Mnemonic.sync_p: m.SideEffect(m.Fn(intrinsics.sync_p)); break;

                // Shift amount register and PCNTL.
                case Mnemonic.mfsa: RewriteMoveFrom(instr, arch.sa); break;
                case Mnemonic.mtsa: RewriteMoveTo(instr, arch.sa); break;
                case Mnemonic.mtsab: RewriteMtsab(instr, 8); break;
                case Mnemonic.mtsah: RewriteMtsab(instr, 16); break;
                case Mnemonic.mfpc:
                case Mnemonic.mtpc:
                case Mnemonic.mfps:
                case Mnemonic.mtps: RewritePcntlMove(instr); break;

                // COP0.
                case Mnemonic.mfc0: RewriteCop0Transfer(instr, true); break;
                case Mnemonic.mtc0: RewriteCop0Transfer(instr, false); break;
                case Mnemonic.bc0f: RewriteCcBranch(instr, Registers.cop0Cc, false); break;
                case Mnemonic.bc0t: RewriteCcBranch(instr, Registers.cop0Cc, true); break;
                case Mnemonic.bc0fl: RewriteCcBranch(instr, Registers.cop0Cc, false); break;
                case Mnemonic.bc0tl: RewriteCcBranch(instr, Registers.cop0Cc, true); break;
                case Mnemonic.tlbr:
                case Mnemonic.tlbwi:
                case Mnemonic.tlbwr:
                case Mnemonic.tlbp: m.SideEffect(m.Fn(intrinsics.tlb_intrinsic)); break;
                case Mnemonic.eret: m.Return(0, 0); break;
                case Mnemonic.ei: m.Assign(binder.EnsureRegister(Registers.ie), Constant.Bool(true)); break;
                case Mnemonic.di: m.Assign(binder.EnsureRegister(Registers.ie), Constant.Bool(false)); break;

                // COP1, the FPU.
                case Mnemonic.mfc1: RewriteMfc1(instr); break;
                case Mnemonic.mtc1: RewriteMtc1(instr); break;
                case Mnemonic.cfc1: RewriteCfcCtc(instr, true); break;
                case Mnemonic.ctc1: RewriteCfcCtc(instr, false); break;
                case Mnemonic.bc1f: RewriteCcBranch(instr, Registers.FpuCc(0), false); break;
                case Mnemonic.bc1t: RewriteCcBranch(instr, Registers.FpuCc(0), true); break;
                case Mnemonic.bc1fl: RewriteCcBranch(instr, Registers.FpuCc(0), false); break;
                case Mnemonic.bc1tl: RewriteCcBranch(instr, Registers.FpuCc(0), true); break;
                case Mnemonic.add_s: RewriteFpuBinop(instr, m.FAdd); break;
                case Mnemonic.sub_s: RewriteFpuBinop(instr, m.FSub); break;
                case Mnemonic.mul_s: RewriteFpuBinop(instr, m.FMul); break;
                case Mnemonic.div_s: RewriteFpuBinop(instr, m.FDiv); break;
                case Mnemonic.sqrt_s: RewriteFpuUnary(instr, intrinsics.sqrt_f32); break;
                case Mnemonic.abs_s: RewriteFpuAbs(instr); break;
                case Mnemonic.mov_s: RewriteFpuMov(instr); break;
                case Mnemonic.neg_s: RewriteFpuNeg(instr); break;
                case Mnemonic.rsqrt_s: RewriteFpuRsqrt(instr); break;
                case Mnemonic.adda_s: RewriteFpuAccBinop(instr, m.FAdd); break;
                case Mnemonic.suba_s: RewriteFpuAccBinop(instr, m.FSub); break;
                case Mnemonic.mula_s: RewriteFpuAccBinop(instr, m.FMul); break;
                case Mnemonic.madd_s: RewriteFpuMac(instr, m.FAdd); break;
                case Mnemonic.msub_s: RewriteFpuMac(instr, m.FSub); break;
                case Mnemonic.madda_s: RewriteFpuMac(instr, m.FAdd); break;
                case Mnemonic.msuba_s: RewriteFpuMac(instr, m.FSub); break;
                case Mnemonic.cvt_w_s: RewriteFpuCvt(instr, PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvt_s_w: RewriteFpuCvt(instr, PrimitiveType.Word32, PrimitiveType.Real32); break;
                case Mnemonic.max_s: RewriteFpuMinMax(instr, intrinsics.max_f32); break;
                case Mnemonic.min_s: RewriteFpuMinMax(instr, intrinsics.min_f32); break;
                case Mnemonic.c_f_s: RewriteFpuCompare(instr, null); break;
                case Mnemonic.c_eq_s: RewriteFpuCompare(instr, m.FEq); break;
                case Mnemonic.c_lt_s: RewriteFpuCompare(instr, m.FLt); break;
                case Mnemonic.c_le_s: RewriteFpuCompare(instr, m.FLe); break;

                // MMI.
                case Mnemonic.madd: RewriteMmiMacc(instr, arch.hi, arch.lo, PrimitiveType.Int64); break;
                case Mnemonic.maddu: RewriteMmiMacc(instr, arch.hi, arch.lo, PrimitiveType.UInt64); break;
                case Mnemonic.madd1: RewriteMmiMacc(instr, arch.hi1, arch.lo1, PrimitiveType.Int64); break;
                case Mnemonic.maddu1: RewriteMmiMacc(instr, arch.hi1, arch.lo1, PrimitiveType.UInt64); break;
                case Mnemonic.plzcw: RewritePlzcw(instr); break;
                case Mnemonic.pmfhl: RewritePmfhl(instr); break;
                case Mnemonic.pmthl: RewritePmthl(instr); break;
                case Mnemonic.pmfhi: RewriteQuadToHiLo(instr, intrinsics.pmfhi); break;
                case Mnemonic.pmthi: RewriteHiLoToQuad(instr, intrinsics.pmthi); break;
                case Mnemonic.pmflo: RewriteQuadToHiLo(instr, intrinsics.pmflo); break;
                case Mnemonic.pmtlo: RewriteHiLoToQuad(instr, intrinsics.pmtlo); break;
                case Mnemonic.psllh:
                case Mnemonic.psrlh:
                case Mnemonic.psrah:
                case Mnemonic.psllw:
                case Mnemonic.psrlw:
                case Mnemonic.psraw: RewriteMmiShiftImm(instr); break;
                case Mnemonic.psllvw:
                case Mnemonic.psrlvw:
                case Mnemonic.psravw: RewriteMmiShiftVar(instr); break;
                case Mnemonic.qfsrv: RewriteQfsrv(instr); break;
                case Mnemonic.pand: RewriteMmiBitwise(instr, m.And); break;
                case Mnemonic.por: RewriteMmiBitwise(instr, m.Or); break;
                case Mnemonic.pxor: RewriteMmiBitwise(instr, m.Xor); break;
                case Mnemonic.pnor: RewriteMmiNor(instr); break;
                case Mnemonic.paddb:
                case Mnemonic.paddh:
                case Mnemonic.paddw:
                case Mnemonic.paddsb:
                case Mnemonic.paddsh:
                case Mnemonic.paddsw:
                case Mnemonic.paddub:
                case Mnemonic.padduh:
                case Mnemonic.padduw:
                case Mnemonic.pcgtb:
                case Mnemonic.pcgth:
                case Mnemonic.pcgtw:
                case Mnemonic.pceqb:
                case Mnemonic.pceqh:
                case Mnemonic.pceqw:
                case Mnemonic.pdivbw:
                case Mnemonic.pdivuw:
                case Mnemonic.pdivw:
                case Mnemonic.phmadh:
                case Mnemonic.phmsbh:
                case Mnemonic.pinteh:
                case Mnemonic.pinth:
                case Mnemonic.pmaxh:
                case Mnemonic.pmaxw:
                case Mnemonic.pminh:
                case Mnemonic.pminw:
                case Mnemonic.pmsubh:
                case Mnemonic.pmsubw:
                case Mnemonic.pmaddh:
                case Mnemonic.pmadduw:
                case Mnemonic.pmaddw:
                case Mnemonic.pmultuw:
                case Mnemonic.pmulth:
                case Mnemonic.pmultw:
                case Mnemonic.prevh:
                case Mnemonic.prot3w:
                case Mnemonic.pexch:
                case Mnemonic.pexcw:
                case Mnemonic.pexeh:
                case Mnemonic.pexew:
                case Mnemonic.pext5:
                case Mnemonic.pextlh:
                case Mnemonic.pextlb:
                case Mnemonic.pextlw:
                case Mnemonic.pextuh:
                case Mnemonic.pextub:
                case Mnemonic.pextuw:
                case Mnemonic.ppacb:
                case Mnemonic.ppach:
                case Mnemonic.ppacw:
                case Mnemonic.ppac5:
                case Mnemonic.pcpyld:
                case Mnemonic.pcpyud:
                case Mnemonic.psubb:
                case Mnemonic.psubh:
                case Mnemonic.psubw:
                case Mnemonic.psubsb:
                case Mnemonic.psubsh:
                case Mnemonic.psubsw:
                case Mnemonic.psubub:
                case Mnemonic.psubuh:
                case Mnemonic.psubuw:
                case Mnemonic.pabsh:
                case Mnemonic.pabsw:
                    RewriteMmiVectorOp(instr); break;

                // COP2 transfers between GPRs and VPU0.
                case Mnemonic.qmfc2: RewriteQmfc2(instr); break;
                case Mnemonic.qmtc2: RewriteQmtc2(instr); break;
                case Mnemonic.cfc2: RewriteCfc2Ctc2(instr, true); break;
                case Mnemonic.ctc2: RewriteCfc2Ctc2(instr, false); break;
                case Mnemonic.lqc2: RewriteVfLoadStore(instr, true); break;
                case Mnemonic.sqc2: RewriteVfLoadStore(instr, false); break;
                case Mnemonic.bc2f: RewriteCcBranch(instr, Registers.vuCc, false); break;
                case Mnemonic.bc2t: RewriteCcBranch(instr, Registers.vuCc, true); break;
                case Mnemonic.bc2fl: RewriteCcBranch(instr, Registers.vuCc, false); break;
                case Mnemonic.bc2tl: RewriteCcBranch(instr, Registers.vuCc, true); break;

                // VU0 macro mode upper instructions.
                case Mnemonic.vcallms: RewriteVcallms(instr); break;
                case Mnemonic.vcallmsr: m.SideEffect(m.Fn(intrinsics.vcallmsr)); break;
                case Mnemonic.vnop: m.Nop(); break;
                case Mnemonic.vwaitq: m.SideEffect(m.Fn(intrinsics.sync_p)); break;
                case Mnemonic.vclipw: RewriteVclipw(instr); break;
                case Mnemonic.vdiv: RewriteEfu2(instr); break;
                case Mnemonic.vsqrt: RewriteEfu1(instr); break;
                case Mnemonic.vrsqrt: RewriteEfu2(instr); break;
                case Mnemonic.vmove:
                case Mnemonic.vmr32:
                case Mnemonic.vabs: RewriteVuMove(instr); break;
                case Mnemonic.vmtir: RewriteVmtir(instr); break;
                case Mnemonic.vmfir: RewriteVmfir(instr); break;
                case Mnemonic.vilwr:
                case Mnemonic.viswr:
                case Mnemonic.vlqi:
                case Mnemonic.vsqi:
                case Mnemonic.vlqd:
                case Mnemonic.vsqd: RewriteVuMemOp(instr); break;
                case Mnemonic.vrnext: RewriteVrGet(instr, intrinsics.vrnext); break;
                case Mnemonic.vrget: RewriteVrGet(instr, intrinsics.vrget); break;
                case Mnemonic.vrinit: RewriteVrSet(instr, intrinsics.vrinit); break;
                case Mnemonic.vrxor: RewriteVrSet(instr, intrinsics.vrxor); break;
                case Mnemonic.viadd: RewriteViBinop(instr, m.IAdd); break;
                case Mnemonic.visub: RewriteViBinop(instr, m.ISub); break;
                case Mnemonic.viand: RewriteViBinop(instr, m.And); break;
                case Mnemonic.vior: RewriteViBinop(instr, m.Or); break;
                case Mnemonic.viaddi: RewriteViAddi(instr); break;

                case Mnemonic.vaddx:
                case Mnemonic.vaddy:
                case Mnemonic.vaddz:
                case Mnemonic.vaddw:
                case Mnemonic.vsubx:
                case Mnemonic.vsuby:
                case Mnemonic.vsubz:
                case Mnemonic.vsubw:
                case Mnemonic.vmaddx:
                case Mnemonic.vmaddy:
                case Mnemonic.vmaddz:
                case Mnemonic.vmaddw:
                case Mnemonic.vmsubx:
                case Mnemonic.vmsuby:
                case Mnemonic.vmsubz:
                case Mnemonic.vmsubw:
                case Mnemonic.vmaxx:
                case Mnemonic.vmaxy:
                case Mnemonic.vmaxz:
                case Mnemonic.vmaxw:
                case Mnemonic.vminix:
                case Mnemonic.vminiy:
                case Mnemonic.vminiz:
                case Mnemonic.vminiw:
                case Mnemonic.vmulx:
                case Mnemonic.vmuly:
                case Mnemonic.vmulz:
                case Mnemonic.vmulw:
                case Mnemonic.vmul_q:
                case Mnemonic.vmax_i:
                case Mnemonic.vmul_i:
                case Mnemonic.vmini_i:
                case Mnemonic.vadd_q:
                case Mnemonic.vmadd_q:
                case Mnemonic.vadd_i:
                case Mnemonic.vmadd_i:
                case Mnemonic.vsub_q:
                case Mnemonic.vmsub_q:
                case Mnemonic.vsub_i:
                case Mnemonic.vmsub_i:
                case Mnemonic.vadd:
                case Mnemonic.vmadd:
                case Mnemonic.vmul:
                case Mnemonic.vmax:
                case Mnemonic.vsub:
                case Mnemonic.vmsub:
                case Mnemonic.vmini:
                    RewriteVuVectorOp(instr); break;

                case Mnemonic.vaddax:
                case Mnemonic.vadday:
                case Mnemonic.vaddaz:
                case Mnemonic.vaddaw:
                case Mnemonic.vsubax:
                case Mnemonic.vsubay:
                case Mnemonic.vsubaz:
                case Mnemonic.vsubaw:
                case Mnemonic.vmaddax:
                case Mnemonic.vmadday:
                case Mnemonic.vmaddaz:
                case Mnemonic.vmaddaw:
                case Mnemonic.vmsubax:
                case Mnemonic.vmsubay:
                case Mnemonic.vmsubaz:
                case Mnemonic.vmsubaw:
                case Mnemonic.vitof0:
                case Mnemonic.vitof4:
                case Mnemonic.vitof12:
                case Mnemonic.vitof15:
                case Mnemonic.vftoi0:
                case Mnemonic.vftoi4:
                case Mnemonic.vftoi12:
                case Mnemonic.vftoi15:
                case Mnemonic.vmulax:
                case Mnemonic.vmulay:
                case Mnemonic.vmulaz:
                case Mnemonic.vmulaw:
                case Mnemonic.vmula_q:
                case Mnemonic.vmula_i:
                case Mnemonic.vaddaq:
                case Mnemonic.vmaddaq:
                case Mnemonic.vaddai:
                case Mnemonic.vmaddai:
                case Mnemonic.vsubaq:
                case Mnemonic.vmsubaq:
                case Mnemonic.vsubai:
                case Mnemonic.vmsubai:
                case Mnemonic.vadda:
                case Mnemonic.vmadda:
                case Mnemonic.vmula:
                case Mnemonic.vsuba:
                case Mnemonic.vmsuba:
                case Mnemonic.vopmula:
                case Mnemonic.vopmsub:
                    RewriteVuAccOp(instr); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }
    }*/
            }
        }
    }
}