using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Pdp7
{
    public class Pdp7Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Pdp7Architecture arch;
        private readonly Word18BeImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private readonly IEnumerator<Pdp7Instruction> dasm;
        private InstrClass iclass;
        private Pdp7Instruction instr;

        public Pdp7Rewriter(Pdp7Architecture arch, Word18BeImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rtls = [];
            this.m = new RtlEmitter(rtls);
            this.dasm = new Pdp7Disassembler(arch, rdr).GetEnumerator();
            this.instr = default!;
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
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.cal: RewriteCal(); break;
                case Mnemonic.cla: RewriteCla(); break;
                case Mnemonic.cll: RewriteCll(); break;
                case Mnemonic.cma: RewriteCma(); break;
                case Mnemonic.cml: RewriteCml(); break;
                case Mnemonic.dac: RewriteDac(); break;
                case Mnemonic.dzm: RewriteDzm(); break;
                case Mnemonic.hlt:
                    m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
                    break;
                case Mnemonic.isz: RewriteIsz(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jms: RewriteJms(); break;
                case Mnemonic.lac: RewriteLac(); break;
                case Mnemonic.oas: RewriteOas(); break;
                case Mnemonic.opr: RewriteOpr(); break;
                case Mnemonic.ral: RewriteRotate(CommonOps.RolC); break;
                case Mnemonic.rar: RewriteRotate(CommonOps.RorC); break;
                case Mnemonic.rtl: RewriteRotate(CommonOps.RolC); RewriteRotate(CommonOps.RolC); break;
                case Mnemonic.rtr: RewriteRotate(CommonOps.RorC); RewriteRotate(CommonOps.RorC); ; break;
                case Mnemonic.sad: RewriteSad(); break;
                case Mnemonic.sma: RewriteSma(); break;
                case Mnemonic.sna: RewriteSna(); break;
                case Mnemonic.snl: RewriteSnl(); break;
                case Mnemonic.spa: RewriteSpa(); break;
                case Mnemonic.sza: RewriteSza(); break;
                case Mnemonic.szl: RewriteSzl(); break;
                case Mnemonic.tad: RewriteTad(); break;
                case Mnemonic.xct: RewriteXct(); break;
                case Mnemonic.xor: RewriteXor(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Pdp7Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression Op(int iop)
        {
            var op = instr.Operands[iop];
            if (op is MemoryOperand mem)
            {
                Expression ea = Pdp7Architecture.Ptr18(mem.Address);
                var dt = mem.DataType;
                if (mem.Deferred)
                {
                    ea = m.Mem(PdpTypes.Ptr18, ea);
                    dt = PdpTypes.Ptr18;
                }
                return m.Mem(dt, ea);
            }
            throw new NotImplementedException($"Operand type {op.GetType().Name} not implemented yet.");
        }

        private void RewriteAdd()
        {
            // 1's complement add.
            var ac = binder.EnsureRegister(Registers.ac);
            var tmp1 = binder.CreateTemporary(PdpTypes.Word36);
            var tmp2 = binder.CreateTemporary(PdpTypes.Word36);
            var tmp3 = binder.CreateTemporary(PdpTypes.Word36);
            m.Assign(tmp1, m.ExtendZ(ac, tmp1.DataType));
            m.Assign(tmp2, m.ExtendZ(Op(0), tmp2.DataType));
            m.Assign(tmp3, m.IAdd(tmp1, tmp2));
            m.Assign(ac, m.Slice(tmp3, ac.DataType));
            m.Assign(ac, m.IAdd(ac, m.Slice(tmp3, PdpTypes.Word18, 18)));
            m.Assign(binder.EnsureFlagGroup(Registers.link), m.Cond(ac));
        }

        private void RewriteAnd()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            var tmp1 = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp1, Op(0));
            m.Assign(ac, m.And(ac, tmp1));
        }

        private void RewriteCal()
        {
            m.Call(Pdp7Architecture.Ptr18(0x10), 0);
        }

        private void RewriteCla()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Assign(ac, 0);
        }

        private void RewriteCll()
        {
            var link = binder.EnsureFlagGroup(Registers.link);
            m.Assign(link, 0);
        }

        private void RewriteCma()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Assign(ac, m.Comp(ac));
        }

        private void RewriteCml()
        {
            var link = binder.EnsureFlagGroup(Registers.link);
            m.Assign(link, m.Not(link));
        }

        private void RewriteDac()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            var dst = Op(0);
            m.Assign(dst, ac);
        }

        private void RewriteDzm()
        {
            var dst = Op(0);
            m.Assign(dst, Constant.Zero(PdpTypes.Word18));
        }

        private void RewriteIsz()
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, Op(0));
            m.Assign(tmp, m.IAdd(tmp, 1));
            m.Assign(Op(0), tmp);
            m.Branch(m.Eq0(tmp), instr.Address + 2);
        }

        private void RewriteJmp()
        {
            var mem = (MemoryAccess) Op(0);
            var target = mem.EffectiveAddress;
            m.Goto(target);
        }

        private void RewriteJms()
        {
            //$BUG: strictly speaking this code is incorrect, because
            // the PDP-7 does not have a return address stack.
            var mem = (MemoryAccess) Op(0);
            var target = mem.EffectiveAddress;
            m.Call(target, 0);
        }

        private void RewriteLac()
        {
            var src = Op(0);
            var ac = binder.EnsureRegister(Registers.ac);
            m.Assign(ac, src);
        }

        private void RewriteOas()
        {
            var tmp1 = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp1, m.Fn(oas_intrinsic));
            var ac = binder.EnsureRegister(Registers.ac);
            m.Assign(ac, m.Or(ac, tmp1));
        }


        private void RewriteOpr()
        {
            m.Nop();
        }


        private void RewriteRotate(IntrinsicProcedure operation)
        {
            Identifier? t;
            var link = binder.EnsureFlagGroup(Registers.link);
            t = binder.CreateTemporary(PrimitiveType.Bool);
            var ac = binder.EnsureRegister(Registers.ac);
            m.Assign(t, link);
            m.Assign(link, m.Ne0(m.And(ac, 1)));
            Expression p;
            p = m.Fn(
                operation.MakeInstance(ac.DataType, ac.DataType),
                ac, ac, t);
            m.Assign(ac, p);
        }

        private void RewriteSad()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            var src = Op(0);
            m.Branch(m.Ne(ac, src), instr.Address + 2);
        }

        private void RewriteSma()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Branch(m.Lt0(ac), instr.Address + 2);
        }

        private void RewriteSna()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Branch(m.Ne0(ac), instr.Address + 2);
        }

        private void RewriteSnl()
        {
            var link = binder.EnsureFlagGroup(Registers.link);
            m.Branch(m.Ne0(link), instr.Address + 2);
        }

        private void RewriteSpa()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Branch(m.Gt0(ac), instr.Address + 2);
        }

        private void RewriteSza()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            m.Branch(m.Eq0(ac), instr.Address + 2);
        }

        private void RewriteSzl()
        {
            var link = binder.EnsureFlagGroup(Registers.link);
            m.Branch(m.Eq0(link), instr.Address + 2);
        }

        private void RewriteTad()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            var src = Op(0);
            m.Assign(ac, m.IAdd(ac, src));
            m.Assign(binder.EnsureFlagGroup(Registers.link), m.Cond(ac));
        }

        private void RewriteXct()
        {
            var mem = (MemoryAccess) Op(0);
            var target = mem.EffectiveAddress;
            m.SideEffect(m.Fn(xct_intrinsic, target));
        }

        private void RewriteXor()
        {
            var ac = binder.EnsureRegister(Registers.ac);
            var src = Op(0);
            m.Assign(ac, m.Xor(ac, src));
        }

        private static readonly IntrinsicProcedure oas_intrinsic = IntrinsicBuilder.SideEffect("__read_ac_switches")
            .Returns(PdpTypes.Word18);
        private static readonly IntrinsicProcedure xct_intrinsic = IntrinsicBuilder.SideEffect("__execute")
            .Param(PdpTypes.Ptr18)
            .Void();
    }
}