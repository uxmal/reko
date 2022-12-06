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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class AeonRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly AeonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<AeonInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private AeonInstruction instr;
        private InstrClass iclass;

        public AeonRewriter(AeonArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            dasm = new AeonDisassembler(arch, rdr).GetEnumerator();
            rtls = new List<RtlInstruction>();
            m = new RtlEmitter(rtls);
            instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.l_addi: RewriteAddOri(m.IAdd); break;
                case Mnemonic.l_addi__: RewriteUnknown(); break;
                case Mnemonic.l_andi: RewriteLogicalImm(m.And); break;
                case Mnemonic.l_andi__: RewriteUnknown(); break;
                case Mnemonic.beqi__: RewriteBxxi(m.Eq); break;
                case Mnemonic.ble__i__: RewriteBxxi(m.Le); break;
                case Mnemonic.l_jal__: RewriteJal(); break;
                case Mnemonic.l_jr: RewriteJr(); break;
                case Mnemonic.l_lhz: RewriteLoadZex(PrimitiveType.UInt16); break;
                case Mnemonic.l_lwz__: RewriteLoadZex(PrimitiveType.Word32); break;
                case Mnemonic.l_movhi: RewriteMovhi(); break;
                case Mnemonic.l_movhi__: RewriteMovhi(); break;
                case Mnemonic.l_nop: RewriteNop(); break;
                case Mnemonic.l_mul: RewriteArithmetic(m.IMul); break;
                case Mnemonic.l_or__: RewriteArithmetic(m.Or); break;
                case Mnemonic.l_ori: RewriteAddOri(m.Or); break;
                case Mnemonic.l_sfeqi: RewriteSfxxi(m.Eq); break;
                case Mnemonic.l_sfgtui: RewriteSfxxi(m.Ugt); break;
                case Mnemonic.l_slli__: RewriteShifti(m.Shl); break;
                case Mnemonic.l_srli__: RewriteShifti(m.Shr); break;
                case Mnemonic.l_syncwritebuffer: RewriteSideEffect(syncwritebuffer_intrinsic); break;
                case Mnemonic.l_sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.l_sw__: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.mov__: RewriteUnknown(); break;
                case Mnemonic.Nyi: RewriteUnknown(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("AeonRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea = binder.EnsureRegister(mem.Base!);
            if (mem.Offset != 0)
            {
                ea = m.AddSubSignedInt(ea, mem.Offset);
            }
            return ea;
        }

        private Expression Op(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.Width, ea);
            default:
                throw new NotImplementedException($"Not impemented: {op.GetType().Name}.");
            }
        }

        private Expression OpOrZero(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                if (reg.Number == 0)
                    return m.Word32(0);
                return binder.EnsureRegister(reg);
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.Width, ea);
            default:
                throw new NotImplementedException($"Not impemented: {op.GetType().Name}.");
            }
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = OpOrZero(2);
            var dst = Op(0);
            m.Assign(dst, fn(left, right));
        }


        private void RewriteBxxi(Func<Expression, Expression, Expression> cmp)
        {
            var left = OpOrZero(0);
            var right = Op(1);
            var target = ((AddressOperand) instr.Operands[2]).Address;
            m.Branch(cmp(left, right), target);
        }

        private void RewriteLoadZex(PrimitiveType dt)
        {
            var src = Op(1);
            if (src.DataType.BitSize < 32)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, src);
                var dst = Op(0);
                m.Assign(dst, m.Convert(tmp, tmp.DataType, dst.DataType));
            }
            else
            {
                var dst = Op(0);
                m.Assign(dst, src);
            }
        }

        private void RewriteMovhi()
        {
            var src = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32();
            var dst = Op(0);
            m.Assign(dst, m.Word32(src << 16));
        }

        private void RewriteNop()
        {
            m.Nop();
        }


        private void RewriteAddOri(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = ((ImmediateOperand) instr.Operands[2]).Value.ToUInt32();
            Expression src;
            if (left.IsZero)
            {
                src = m.Word32(right);
            }
            else
            {
                src = fn(left, m.Word32(right));
            }
            m.Assign(Op(0), src);
        }

        private void RewriteLogicalImm(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = ((ImmediateOperand) instr.Operands[2]).Value.ToUInt32();
            var dst = Op(0);
            m.Assign(dst, fn(left, m.Word32(right)));
        }

        private void RewriteJal()
        {
            var target = Op(0);
            m.Call(target, 0);
        }

        private void RewriteJr()
        {
            var op0 = (Identifier)Op(0);
            var op1 = (Identifier)Op(1);
            if ((int)op0.Storage.Domain == 9 && (int)op1.Storage.Domain == 9)
            {
                //$REVIEW: this is a weird encoding for a return
                iclass = InstrClass.Transfer | InstrClass.Return;
                m.Return(0, 0);
                return;
            }
            throw new NotImplementedException();
        }

        private void RewriteSfxxi(Func<Expression, Expression, Expression> fn) 
        {
            var left = OpOrZero(0);
            var right = OpOrZero(1);
            var dst = binder.EnsureFlagGroup(Registers.F);
            m.Assign(dst, fn(left, right));
        }

        private void RewriteShifti(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var dst = Op(0);
            m.Assign(dst, fn(left, m.Int32(right)));
        }

        private void RewriteSideEffect(IntrinsicProcedure intrinsic)
        {
            var args = new List<Expression> { };
            for (int iop = 0; iop < instr.Operands.Length; ++iop)
            {
                args.Add(Op(iop));
            }
            m.SideEffect(m.Fn(intrinsic, args.ToArray()));
        }

        private void RewriteStore(PrimitiveType dt)
        {
            var src = OpOrZero(1);
            if (dt.BitSize < src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(src, dt));
                src = tmp;
            }
            var dst = Op(0);
            m.Assign(dst, src);
        }

        //$TODO: remove this once all instructions are known. It
        // is a globally mutable cache, which will cause race
        // conditions in multithreaded environments.
        private readonly static ConcurrentDictionary<Mnemonic, IntrinsicProcedure> intrinsics = new();

        private void RewriteUnknown()
        {
            var args = new List<Expression> { };
            for (int iop = 0; iop < instr.Operands.Length; ++iop)
            {
                args.Add(Op(iop));
            }
            IntrinsicProcedure? intrinsic;
            while (!intrinsics.TryGetValue(instr.Mnemonic, out intrinsic))
            {
                var ib = new IntrinsicBuilder(instr.MnemonicAsString, true);
                foreach (var e in args)
                {
                    ib.Param(e.DataType);
                }
                intrinsic = ib.Void();
                if (intrinsics.TryAdd(instr.Mnemonic, intrinsic))
                    break;
            }
            m.SideEffect(m.Fn(intrinsic, args.ToArray()));
        }

        private static readonly IntrinsicProcedure syncwritebuffer_intrinsic = new IntrinsicBuilder("__syncwritebuffer", true)
            .Void();
    }
}
