#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Arm
{
    public class ThumbRewriterNew : IEnumerable<RtlInstructionCluster>
    {
        private static HashSet<ArmInstruction> seen = new HashSet<ArmInstruction>();

        private IEnumerator<Arm32Instruction> instrs;
        private Dictionary<int, RegisterStorage> regs;
        private INativeArchitecture nArch;
        private EndianImageReader rdr;
        private IStorageBinder binder;
        private IRewriterHost host;
        private Instruction<ArmInstruction,ArmRegister,ArmInstructionGroup,ArmInstructionDetail> instr;
        private ArmInstructionOperand[] ops;
        private RtlClass rtlc;
        private RtlEmitter m;
        private string itState;     // contains the state needed to rewrite "IT" instructions.
        private int itPos;          // position within the itState above.
        private ArmCodeCondition itStateCondition;

        public ThumbRewriterNew(Dictionary<int, RegisterStorage> regs, INativeArchitecture nArch, EndianImageReader rdr, ArmProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.regs = regs;
            this.nArch = nArch;
            this.rdr = rdr;
            this.binder = binder;
            this.host = host;
            this.itState = null;
            this.itStateCondition = ArmCodeCondition.AL;
        }

        public ArmCodeCondition CurrentItStateCondition
        {
            get
            {
                if (itState != null && itState[itPos] == 'e')
                    return Invert(this.itStateCondition);
                else
                    return this.itStateCondition;
            }
        }

        private ArmCodeCondition Invert(ArmCodeCondition cond)
        {
            switch (cond)
            {
            case ArmCodeCondition.HS: return ArmCodeCondition.LO;
            case ArmCodeCondition.LO: return ArmCodeCondition.HS;
            case ArmCodeCondition.EQ: return ArmCodeCondition.NE;
            case ArmCodeCondition.GE: return ArmCodeCondition.LT;
            case ArmCodeCondition.GT: return ArmCodeCondition.LE;
            case ArmCodeCondition.HI: return ArmCodeCondition.LS;
            case ArmCodeCondition.LE: return ArmCodeCondition.GT;
            case ArmCodeCondition.LS: return ArmCodeCondition.HI;
            case ArmCodeCondition.LT: return ArmCodeCondition.GE;
            case ArmCodeCondition.MI: return ArmCodeCondition.PL;
            case ArmCodeCondition.PL: return ArmCodeCondition.MI;
            case ArmCodeCondition.NE: return ArmCodeCondition.EQ;
            case ArmCodeCondition.VC: return ArmCodeCondition.VS;
            case ArmCodeCondition.VS: return ArmCodeCondition.VC;
            }
            throw new NotImplementedException(string.Format("Don't know how to invert {0}.", cond));
        }

        private IEnumerator<Arm32Instruction> CreateInstructionStream(EndianImageReader rdr)
        {
            return new ThumbDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var bytes = rdr.Bytes;
            var offset = (int)rdr.Offset;
            var addr = rdr.Address.ToLinear();
            return new Enumerator(regs, this);
            while (instrs.MoveNext())
            {
                var rtlInstructions = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtlInstructions);
                if (!instrs.Current.TryGetInternal(out this.instr))
                {
                    Invalid();
                    yield return new RtlInstructionCluster(
                        instrs.Current.Address,
                        instrs.Current.Length,
                        rtlInstructions.ToArray())
                    {
                        Class = RtlClass.Invalid
                    };
                }
                this.ops = instr.ArchitectureDetail.Operands;
                this.rtlc = RtlClass.Linear;
                switch (instr.Id)
                {
                default:
                    EmitUnitTest();
                    Invalid();
                    break;
                case ArmInstruction.Invalid:
                    Invalid();
                    break;
                case ArmInstruction.ADD: RewriteBinop((a, b) => m.IAdd(a, b)); break;
                case ArmInstruction.ADDW: RewriteAddw(); break;
                case ArmInstruction.ADR: RewriteAdr(); break;
                case ArmInstruction.AND: RewriteAnd(); break;
                case ArmInstruction.ASR: RewriteShift(m.Sar); break;
                case ArmInstruction.B: RewriteB(); break;
                case ArmInstruction.BIC: RewriteBic(); break;
                case ArmInstruction.BL: RewriteBl(); break;
                case ArmInstruction.BLX: RewriteBlx(); break;
                case ArmInstruction.BX: RewriteBx(); break;
                case ArmInstruction.CBZ: RewriteCbnz(m.Eq0); break;
                case ArmInstruction.CBNZ: RewriteCbnz(m.Ne0); break;
                case ArmInstruction.CMP: RewriteCmp(); break;
                case ArmInstruction.DMB: RewriteDmb(); break;
                case ArmInstruction.EOR: RewriteEor(); break;
                case ArmInstruction.IT: RewriteIt(); break;
                case ArmInstruction.LDR: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case ArmInstruction.LDRB: RewriteLdr(PrimitiveType.UInt32, PrimitiveType.Byte); break;
                case ArmInstruction.LDRSB: RewriteLdr(PrimitiveType.Int32, PrimitiveType.SByte); break;
                case ArmInstruction.LDREX: RewriteLdrex(); break;
                case ArmInstruction.LDRH: RewriteLdr(PrimitiveType.UInt32, PrimitiveType.Word16); break;
                case ArmInstruction.LSL: RewriteShift(m.Shl); break;
                case ArmInstruction.LSR: RewriteShift(m.Shr); break;
                case ArmInstruction.MOV: RewriteMov(); break;
                case ArmInstruction.MOVT: RewriteMovt(); break;
                case ArmInstruction.MOVW: RewriteMovw(); break;
                case ArmInstruction.MRC: RewriteMrc(); break;
                case ArmInstruction.MVN: RewriteMvn(); break;
                case ArmInstruction.POP: RewritePop(); break;
                case ArmInstruction.PUSH: RewritePush(); break;
                case ArmInstruction.RSB: RewriteRsb(); break;
                case ArmInstruction.STM: RewriteStm(); break;
                case ArmInstruction.STR: RewriteStr(PrimitiveType.Word32); break;
                case ArmInstruction.STRH: RewriteStr(PrimitiveType.Word16); break;
                case ArmInstruction.STRB: RewriteStr(PrimitiveType.Byte); break;
                case ArmInstruction.STREX: RewriteStrex(); break;
                case ArmInstruction.SUB: RewriteBinop((a, b) => m.ISub(a, b)); break;
                case ArmInstruction.SUBW: RewriteSubw(); break;
                case ArmInstruction.TRAP: RewriteTrap(); break;
                case ArmInstruction.TST: RewriteTst(); break;
                case ArmInstruction.UDF: RewriteUdf(); break;
                case ArmInstruction.UXTH: RewriteUxth(); break;
                }
                yield return new RtlInstructionCluster(
                    instrs.Current.Address,
                    instr.Bytes.Length,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
                if (itState != null)
                {
                    if (++itPos >= itState.Length)
                    {
                        itState = null;
                        itPos = 0;
                        itStateCondition = ArmCodeCondition.AL;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(instr.Id))
                return;
            seen.Add(instr.Id);

            var bytes = instr.Bytes;
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void ThumbRw_" + instr.Id.ToString().ToLower()+ "()");
            Debug.WriteLine("        {");
            Debug.Write("            RewriteCode(\"");
            Debug.Write(string.Join(
                "",
                bytes.Select(b => string.Format("{0:X2}", (int)b))));
            Debug.WriteLine("\");\t// {0} {1}", instr.Mnemonic, instr.Operand);
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", bytes.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        private void Invalid()
        {
            m.Invalid();
            rtlc = RtlClass.Invalid;
        }


        private Expression GetReg(ArmRegister armRegister)
        {
            private INativeRewriter native;
            private byte[] bytes;
            private GCHandle hBytes;
            private RtlEmitter m;
            private NativeTypeFactory ntf;
            private NativeRtlEmitter rtlEmitter;
            private ArmNativeRewriterHost host;
            private IntPtr iRtlEmitter;
            private IntPtr iNtf;
            private IntPtr iHost;

            public Enumerator(Dictionary<int, RegisterStorage> regs, ThumbRewriterNew outer)
            {
                this.bytes = outer.rdr.Bytes;
                ulong addr = outer.rdr.Address.ToLinear();
                this.hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                this.m = new RtlEmitter(new List<RtlInstruction>());
                this.ntf = new NativeTypeFactory();
                this.rtlEmitter = new NativeRtlEmitter(m, ntf, outer.host);
                this.host = new ArmNativeRewriterHost(regs, outer.binder, outer.host, this.ntf, rtlEmitter);

                this.iRtlEmitter = GetCOMInterface(rtlEmitter, IID_IRtlEmitter);
                this.iNtf = GetCOMInterface(ntf, IID_INativeTypeFactory);
                this.iHost = GetCOMInterface(host, IID_INativeRewriterHost);

                this.native = outer.nArch.CreateRewriter(
                    hBytes.AddrOfPinnedObject(),
                    bytes.Length,
                    (int)outer.rdr.Offset,
                    addr,
                    rtlEmitter,
                    ntf,
                    host);
        }

            public RtlInstructionCluster Current { get; private set; }

            object IEnumerator.Current { get { return Current; } }

            private IntPtr GetCOMInterface(object o, Guid iid)
        {
                var iUnknown = Marshal.GetIUnknownForObject(o);
                IntPtr intf;
                var hr2 = Marshal.QueryInterface(iUnknown, ref iid, out intf);
                return intf;
            }

            public void Dispose()
        {
                if (this.native != null)
            {
                    int n = native.GetCount();
                    Marshal.ReleaseComObject(this.native);
                    this.native = null;
            }
                if (iHost != null)
            {
                    Marshal.Release(iHost);
            }
                if (iNtf != null)
            {
                    Marshal.Release(iNtf);
            }
                if (iRtlEmitter != null)
                {
                    Marshal.Release(iRtlEmitter);
        }
                if (this.hBytes != null && this.hBytes.IsAllocated)
        {
                    this.hBytes.Free();
            }
        }

            public bool MoveNext()
        {
                m.Instructions = new List<RtlInstruction>();
                int n = native.GetCount();
                if (native.Next() == 1)
                    return false;
                this.Current = this.rtlEmitter.ExtractCluster();
                return true;
        }

            public void Reset()
        {
            if (cond == ArmCodeCondition.AL)
            {
                m.Emit(instr);
            }
            else
            {
                m.BranchInMiddleOfInstruction(
                    TestCond(cond).Invert(),
                    Address.Ptr32((uint)(this.instr.Address + this.instr.Bytes.Length)),
                    RtlClass.ConditionalTransfer);
                m.Emit(instr);
            }
        }

        {
            RtlInstruction instr;
            Identifier id;
            if (dst.As(out id) && id.Storage == A32Registers.pc)
            {
                rtlc = RtlClass.Transfer;
                instr = new RtlGoto(src, RtlClass.Transfer);
            }
        static ThumbRewriterNew()
            {
            IID_INativeRewriter = typeof(INativeRewriter).GUID;
            IID_INativeRewriterHost = typeof(INativeRewriterHost).GUID;
            IID_INativeTypeFactory = typeof(INativeTypeFactory).GUID;
            IID_IRtlEmitter = typeof(INativeRtlEmitter).GUID;
            }

        private static Guid IID_INativeRewriter;
        private static Guid IID_INativeRewriterHost;
        private static Guid IID_IRtlEmitter;
        private static Guid IID_INativeTypeFactory;
        }
}
