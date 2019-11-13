using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Arc
{
    public class ARCompactRewriter : IEnumerable<RtlInstructionCluster>
    {
        private const FlagM ZNCV = FlagM.ZF | FlagM.NF | FlagM.CF | FlagM.VF;
        private const FlagM ZN = FlagM.ZF | FlagM.NF;

        private readonly ARCompactArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<ArcInstruction> dasm;
        private ArcInstruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public ARCompactRewriter(ARCompactArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new ArcDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                m = new RtlEmitter(instrs);
                switch (instr.Mnemonic)
                {
                case Mnemonic.abs:
                case Mnemonic.abss:
                case Mnemonic.abssw:
                case Mnemonic.adc:
                case Mnemonic.add:
                case Mnemonic.add1:
                case Mnemonic.add2:
                case Mnemonic.add3:
                case Mnemonic.adds:
                case Mnemonic.addsdw:
                case Mnemonic.and:
                case Mnemonic.asl:
                case Mnemonic.asls:
                case Mnemonic.asr:
                case Mnemonic.asrs:
                case Mnemonic.bbit0:
                case Mnemonic.bbit1:
                case Mnemonic.bclr:
                case Mnemonic.blvc:
                case Mnemonic.blvs:
                case Mnemonic.bsc:
                case Mnemonic.bset:
                case Mnemonic.bss:
                case Mnemonic.btst:
                case Mnemonic.bxor:
                case Mnemonic.cmp:
                case Mnemonic.divaw:
                case Mnemonic.ex:
                case Mnemonic.extb:
                case Mnemonic.extw:
                case Mnemonic.flag:
                case Mnemonic.j:
                case Mnemonic.jcc:
                case Mnemonic.jcs:
                case Mnemonic.jeq:
                case Mnemonic.jge:
                case Mnemonic.jgt:
                case Mnemonic.jhi:
                case Mnemonic.jl:
                case Mnemonic.jle:
                case Mnemonic.jls:
                case Mnemonic.jlt:
                case Mnemonic.jmi:
                case Mnemonic.jne:
                case Mnemonic.jpl:
                case Mnemonic.jpnz:
                case Mnemonic.jvc:
                case Mnemonic.jvs:
                case Mnemonic.lp:
                case Mnemonic.lpcc:
                case Mnemonic.lpcs:
                case Mnemonic.lpeq:
                case Mnemonic.lpge:
                case Mnemonic.lpgt:
                case Mnemonic.lphi:
                case Mnemonic.lple:
                case Mnemonic.lpls:
                case Mnemonic.lplt:
                case Mnemonic.lpmi:
                case Mnemonic.lpne:
                case Mnemonic.lppl:
                case Mnemonic.lppnz:
                case Mnemonic.lpvc:
                case Mnemonic.lpvs:
                case Mnemonic.lsr:
                case Mnemonic.max:
                case Mnemonic.min:
                case Mnemonic.mul64:
                case Mnemonic.mulu64:
                case Mnemonic.negs:
                case Mnemonic.negsw:
                case Mnemonic.nop:
                case Mnemonic.norm:
                case Mnemonic.normw:
                case Mnemonic.rcmp:
                case Mnemonic.rlc:
                case Mnemonic.rnd16:
                case Mnemonic.ror:
                case Mnemonic.rrc:
                case Mnemonic.rsub:
                case Mnemonic.sat16:
                case Mnemonic.sbc:
                case Mnemonic.sexb:
                case Mnemonic.sexw:
                case Mnemonic.sr:
                case Mnemonic.sub:
                case Mnemonic.sub1:
                case Mnemonic.sub2:
                case Mnemonic.sub3:
                case Mnemonic.subs:
                case Mnemonic.subsdw:
                case Mnemonic.swap:
                case Mnemonic.trap0:
                case Mnemonic.tst:
                case Mnemonic.xor:

                case Mnemonic.abs_s:
                case Mnemonic.add_s:
                case Mnemonic.add1_s:
                case Mnemonic.add2_s:
                case Mnemonic.add3_s:
                case Mnemonic.and_s:
                case Mnemonic.asl_s:
                case Mnemonic.asr_s:
                case Mnemonic.bclr_s:
                case Mnemonic.bic_s:
                case Mnemonic.bl_s:
                case Mnemonic.breq_s:
                case Mnemonic.brk_s:
                case Mnemonic.brne_s:
                case Mnemonic.bset_s:
                case Mnemonic.btst_s:
                case Mnemonic.extb_s:
                case Mnemonic.extw_s:
                case Mnemonic.jl_s:
                case Mnemonic.ldb_s:
                case Mnemonic.lsr_s:
                case Mnemonic.mul64_s:
                case Mnemonic.neg_s:
                case Mnemonic.not_s:
                case Mnemonic.or_s:
                case Mnemonic.sexb_s:
                case Mnemonic.sexw_s:
                case Mnemonic.st_s:
                case Mnemonic.stb_s:
                case Mnemonic.trap_s:
                case Mnemonic.tst_s:
                case Mnemonic.unimp_s:
                case Mnemonic.xor_s:
                default:
                    EmitUnitTest(this.instr);
                    break;
                case Mnemonic.Invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.b:    case Mnemonic.b_s: RewriteB(ArcCondition.AL); break;
                case Mnemonic.bcc:  case Mnemonic.bhs_s: RewriteB(ArcCondition.CC); break;
                case Mnemonic.bcs:  case Mnemonic.blo_s: RewriteB(ArcCondition.CS); break;
                case Mnemonic.beq:  case Mnemonic.beq_s: RewriteB(ArcCondition.EQ); break;
                case Mnemonic.bge:  case Mnemonic.bge_s: RewriteB(ArcCondition.GE); break;
                case Mnemonic.bgt:  case Mnemonic.bgt_s: RewriteB(ArcCondition.GT); break;
                case Mnemonic.bhi:  case Mnemonic.bhi_s: RewriteB(ArcCondition.HI); break;
                case Mnemonic.ble:  case Mnemonic.ble_s: RewriteB(ArcCondition.LE); break;
                case Mnemonic.bls:  case Mnemonic.bls_s: RewriteB(ArcCondition.LS); break;
                case Mnemonic.blt:  case Mnemonic.blt_s: RewriteB(ArcCondition.LT); break;
                case Mnemonic.bmi:  RewriteB(ArcCondition.MI); break;
                case Mnemonic.bne:  case Mnemonic.bne_s: RewriteB(ArcCondition.NE); break;
                case Mnemonic.bpl:  RewriteB(ArcCondition.PL); break;
                case Mnemonic.bpnz: RewriteB(ArcCondition.PNZ); break;
                case Mnemonic.bvc: RewriteB(ArcCondition.VC); break;
                case Mnemonic.bvs: RewriteB(ArcCondition.VS); break;

                case Mnemonic.bl:    RewriteBl(ArcCondition.AL); break;
                case Mnemonic.blal:  RewriteBl(ArcCondition.AL); break;
                case Mnemonic.blcc:  RewriteBl(ArcCondition.CC); break;
                case Mnemonic.blcs:  RewriteBl(ArcCondition.CS); break;
                case Mnemonic.bleq:  RewriteBl(ArcCondition.EQ); break;
                case Mnemonic.blge:  RewriteBl(ArcCondition.GE); break;
                case Mnemonic.blgt:  RewriteBl(ArcCondition.GT); break;
                case Mnemonic.blhi:  RewriteBl(ArcCondition.HI); break;
                case Mnemonic.blle:  RewriteBl(ArcCondition.LE); break;
                case Mnemonic.blls:  RewriteBl(ArcCondition.LS); break;
                case Mnemonic.bllt:  RewriteBl(ArcCondition.LT); break;
                case Mnemonic.blmi:  RewriteBl(ArcCondition.MI); break;
                case Mnemonic.blne:  RewriteBl(ArcCondition.NE); break;
                case Mnemonic.blpl:  RewriteBl(ArcCondition.PL); break;
                case Mnemonic.blpnz: RewriteBl(ArcCondition.PNZ); break;

                case Mnemonic.breq: RewriteBr(m.Eq); break;
                case Mnemonic.brge: RewriteBr(m.Ge); break;
                case Mnemonic.brhs: RewriteBr(m.Uge); break;
                case Mnemonic.brlo: RewriteBr(m.Ult); break;
                case Mnemonic.brlt: RewriteBr(m.Lt); break;
                case Mnemonic.brne: RewriteBr(m.Ne); break;

                case Mnemonic.bic: RewriteAluOp(Bic, ZN); break;
                case Mnemonic.bmsk:
                case Mnemonic.bmsk_s:
                    RewriteAluOp(Bmsk, ZN); break;
                case Mnemonic.cmp_s: RewriteCmp(); break;

                case Mnemonic.j_s: RewriteJ(); break;

                case Mnemonic.ld:
                case Mnemonic.ld_s:
                    RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldb: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.ldw:
                case Mnemonic.ldw_s:
                    RewriteLoad(PrimitiveType.Word16); break;
                case Mnemonic.lr: RewriteLr(); break;
                case Mnemonic.or: RewriteAluOp(m.Or, ZN); break;
                case Mnemonic.mov:
                case Mnemonic.mov_s:
                    RewriteMov(); break;
                case Mnemonic.not: RewriteAluOp(m.Comp, ZN); break;
                case Mnemonic.pop_s: RewritePop(); break;
                case Mnemonic.push_s: RewritePush(); break;
                case Mnemonic.st: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.stw:
                case Mnemonic.stw_s:
                    RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.sub_s: RewriteAluOp(m.ISub, ZNCV); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = iclass
                };
            }
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Mnemonic> opcode_seen = new HashSet<Mnemonic>();

        void EmitUnitTest(ArcInstruction instr, string message = "")
        {
            if (opcode_seen.Contains(instr.Mnemonic))
                return;
            opcode_seen.Add(instr.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine("        // {0}", message);
            }
            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void ARCompactRw_{0}()", instr.Mnemonic);
            Console.WriteLine("        {");

            if (instr.Length > 2)
            {
                var wInstrHi = r2.ReadUInt16();
                var wInstrLo = r2.ReadUInt16();
                uint wInstr = (((uint) wInstrHi) << 16) | wInstrLo; 
                Console.WriteLine($"            RewriteCode(\"{wInstr:X8}\"); // {instr}");
            }
            else
            {
                var wInstr = r2.ReadUInt16();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X4}\"); // {instr}");
            }
            Console.WriteLine("            AssertCode(");
            Console.WriteLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            Console.WriteLine($"                \"1|L--|@@@\");");
            Console.WriteLine("        }");
            Console.WriteLine();

            m.Invalid();
            iclass = InstrClass.Invalid;
        }

        private Expression Bic(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private Expression Bmsk(Expression a, Expression b)
        {
            return host.PseudoProcedure("__bitmask", a.DataType, a, b);
        }

        private Expression Operand(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand iop:
                return iop.Value;
            case AddressOperand aop:
                return aop.Address;
            default:
                throw new NotImplementedException($"Unimplemented operand type {instr.Operands[iOp].GetType().Name}.");
            }
        }

        private void MaybeSkip(ArcCondition cond)
        {
            if (cond == ArcCondition.AL)
                return;
            var test = TestFlags(cond).Invert();
            m.BranchInMiddleOfInstruction(test, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
        }


        private Expression TestFlags(ArcCondition cond)
        {
            FlagM grf = 0;
            ConditionCode cc = ConditionCode.None;
            switch (cond)
            {
            case ArcCondition.EQ: cc = ConditionCode.EQ; grf = FlagM.ZF; break;
            case ArcCondition.NE: cc = ConditionCode.NE; grf = FlagM.ZF; break;
            case ArcCondition.PL: cc = ConditionCode.GE; grf = FlagM.NF; break;
            case ArcCondition.MI: cc = ConditionCode.LT; grf = FlagM.NF; break;
            case ArcCondition.CS: cc = ConditionCode.ULT; grf = FlagM.CF; break;
            case ArcCondition.CC: cc = ConditionCode.UGE; grf = FlagM.CF; break;
            case ArcCondition.VS: cc = ConditionCode.OV; grf = FlagM.VF; break;
            case ArcCondition.VC: cc = ConditionCode.NO; grf = FlagM.VF; break;
            case ArcCondition.GT: cc = ConditionCode.GT; grf = FlagM.ZF | FlagM.NF | FlagM.VF; break;
            case ArcCondition.GE: cc = ConditionCode.GE; grf = FlagM.ZF | FlagM.NF; break;
            case ArcCondition.LT: cc = ConditionCode.LT; grf = FlagM.ZF | FlagM.NF; break;
            case ArcCondition.LE: cc = ConditionCode.LE; grf = FlagM.ZF | FlagM.NF | FlagM.VF; break;
            case ArcCondition.HI: cc = ConditionCode.UGT; grf = FlagM.ZF | FlagM.CF; break;
            case ArcCondition.LS: cc = ConditionCode.ULE; grf = FlagM.ZF | FlagM.CF; break;
            case ArcCondition.PNZ: cc = ConditionCode.GT; grf = FlagM.ZF | FlagM.NF; break;
            default:
                EmitUnitTest(instr, $"Unknown ArcCondition {cond}");
                break;
            }
            var flags = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.Status32, (uint) grf));
            return m.Test(cc, flags);
        }

        private void MaybeCast(Expression dst, Expression src)
        {
            var bitDiff = dst.DataType.BitSize - src.DataType.BitSize;
            if (bitDiff > 0)
            {
                src = m.Cast(dst.DataType, src);
            }
            m.Assign(dst, src);
        }

        private void MaybeSlice(Expression dst, Expression src)
        {
            var bitDiff = src.DataType.BitSize - dst.DataType.BitSize;
            if (bitDiff > 0)
            {
                src = m.Slice(dst.DataType, src, 0);
            }
            m.Assign(dst, src);
        }

        private (RegisterStorage baseReg, Expression ea) RewriteEa(MemoryOperand mem)
        {
            Expression ea = null;
            if (mem.Base != null)
            {
                if (mem.Base == Registers.Pcl)
                {
                    var uAddr = (int) instr.Address.ToUInt32() & ~3;
                    ea = Address.Ptr32((uint) (uAddr + mem.Offset));
                }
                else
                {
                    ea = binder.EnsureRegister(mem.Base);
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
            }
            else
            {
                ea = m.Word32(mem.Offset);
            }
            return (mem.Base, ea);
        }

        private void RewriteAluOp(Func<Expression, Expression> fn, FlagM grf)
        {
            MaybeSkip(instr.Condition);
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, fn(src));
            if (instr.SetFlags)
            {
                var flagReg = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.Status32, (uint) grf));
                m.Assign(flagReg, m.Cond(dst));
            }
        }

        private void RewriteAluOp(Func<Expression, Expression, Expression> fn, FlagM grf)
        {
            MaybeSkip(instr.Condition);
            var src1 = Operand(1);
            var src2 = Operand(2);
            var dst = Operand(0);
            m.Assign(dst, fn(src1, src2));
            if (instr.SetFlags)
            {
                var flagReg = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.Status32, (uint) grf));
                m.Assign(flagReg, m.Cond(dst));
            }
        }

        private void RewriteB(ArcCondition cond)
        {
            var addr = ((AddressOperand) instr.Operands[0]).Address;
            if (cond == ArcCondition.AL)
            {
                m.Goto(addr, this.iclass);
            }
            else
            {
                m.Branch(this.TestFlags(cond), addr, this.iclass);
            }
        }

        private void RewriteBl(ArcCondition cond)
        {
            MaybeSkip(cond);
            m.Call(Operand(0), 0, instr.InstructionClass);
        }

        private void RewriteBr(Func<Expression, Expression, Expression> cmp)
        {
            var src1 = Operand(0);
            var src2 = Operand(1);
            m.Branch(cmp(src1, src2), ((AddressOperand) instr.Operands[2]).Address, instr.InstructionClass);
        }

        private void RewriteCmp()
        {
            var src1 = Operand(0);
            var src2 = Operand(1);
            var dst = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.Status32, (uint)ZNCV));
            m.Assign(dst, m.Cond(m.ISub(src1, src2)));
        }

        private void RewriteJ()
        {
            if (instr.Operands[0] is MemoryOperand mop)
            {
                if (mop.Base != null && mop.Offset == 0)
                {
                    if (mop.Base == Registers.Blink)
                    {
                        m.Return(0, 0, instr.InstructionClass);
                        return;
                    }
                }
            }
            EmitUnitTest(instr, "Unimplemented J instruction");
        }

        private void RewriteLoad(PrimitiveType dt)
        {
            var dst = Operand(0);
            var (baseReg, ea) = RewriteEa((MemoryOperand) instr.Operands[1]);
            switch (instr.Writeback)
            {
            case AddressWritebackMode.None:
                MaybeCast(dst, m.Mem(dt, ea));
                break;
            case AddressWritebackMode.ab:
                var reg = binder.EnsureRegister(baseReg);
                MaybeCast(dst, m.Mem(dt, reg));
                m.Assign(reg, ea);
                break;
            default:
                host.Warn(instr.Address, "Unimplemented writeback mode {0}", instr.Writeback);
                EmitUnitTest(instr);
                return;
            }
        }

        private void RewriteLr()
        {
            var dst = Operand(0);
            var src = (uint) ((MemoryOperand) instr.Operands[1]).Offset;
            m.Assign(dst, host.PseudoProcedure("__load_aux_reg", PrimitiveType.Word32, m.Word32(src)));
        }

        private void RewriteMov()
        {
            var src = Operand(1);
            var dst = Operand(0);
            m.Assign(dst, src);
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.Sp);
            var dst = Operand(0);
            m.Assign(dst, m.Mem32(sp));
            m.Assign(sp, m.IAddS(sp, 4));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.Sp);
            var src = Operand(0);
            m.Assign(sp, m.ISubS(sp, 4));
            m.Assign(m.Mem32(sp), src);
        }

        private void RewriteStore(PrimitiveType dt)
        {
            var src = Operand(0);
            var (baseReg, ea) = RewriteEa((MemoryOperand) instr.Operands[1]);
            switch (instr.Writeback)
            {
            case AddressWritebackMode.None:
                MaybeSlice(m.Mem(dt, ea), src);
                break;
            case AddressWritebackMode.aw:
                var reg = binder.EnsureRegister(baseReg);
                m.Assign(reg, ea);
                MaybeSlice(m.Mem(dt, reg), src);
                break;
            default:
                host.Warn(instr.Address, "Unimplemented writeback mode {0}", instr.Writeback);
                EmitUnitTest(instr);
                m.Invalid();
                iclass = InstrClass.Invalid;
                return;
            }
        }
    }
}