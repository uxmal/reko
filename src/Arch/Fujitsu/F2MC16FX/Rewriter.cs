using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly F2MC16FXArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private Instruction instr;
        private InstrClass iclass;

        public Rewriter(F2MC16FXArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Disassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid(); iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.movx: RewriteMovx(); break;
                case Mnemonic.mulu: RewriteMulu(); break;
                case Mnemonic.nop: m.Nop(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("F2MC16FXRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression Op(int iop, PrimitiveType? dt = null)
        {
            var op = instr.Operands[iop];
            Expression e;
            switch (op)
            {
            case RegisterStorage reg:
                e = binder.EnsureRegister(reg);
                break;
            case AddressOperand addr:
                e = addr.Address;
                break;
            case MemoryOperand mem:
                if (mem.Base is { })
                {
                    e = binder.EnsureRegister(mem.Base);
                    if (mem.Index is { })
                    {
                        e = m.IAdd(e, binder.EnsureRegister(mem.Index));
                    }
                    else if (mem.Displacement is { })
                    {
                        e = m.AddSubSignedInt(e, mem.Displacement.ToInt32());
                    }
                }
                else
                {
                    e = mem.Displacement!;
                }
                if (mem.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(dt ?? mem.Width);
                    m.Assign(tmp, m.Mem(tmp.DataType, e));
                    m.Assign(e, m.IAdd(e, tmp.DataType.Size));
                    return tmp;
                }
                else
                {
                    return m.Mem(dt ?? mem.Width, e);
                }
            default:
                throw new NotImplementedException($"Rewrite of {op.GetType().Name}.");
            }
            if (dt is { } && e.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(e, dt));
                return tmp;
            }
            else
            {
                return e;
            }
        }

        private void RewriteCall()
        {
            m.Call(Op(0), 2);
        }

        private void RewriteMovx()
        {
            var al = binder.EnsureRegister(Registers.al);
            if (instr.Operands[1] is not MemoryOperand mem ||
                mem.Base != Registers.a)
            {
                m.Assign(binder.EnsureRegister(Registers.ah), al);
            }
            var src = Op(1, PrimitiveType.Byte);
            m.Assign(al, m.Convert(src, src.DataType, PrimitiveType.Int16));
        }

        private void RewriteMulu()
        {
            if (instr.Operands.Length != 2 || instr.Operands[0] != Registers.a)
            {
                EmitUnitTest();
                iclass = InstrClass.Invalid;
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.UInt8);
            var a = binder.EnsureRegister(Registers.a);
            var al = binder.EnsureRegister(Registers.al);
            m.Assign(tmp, m.Slice(a, tmp.DataType));
            var src = Op(1);
            m.Assign(al, m.UMul(PrimitiveType.UInt16, tmp, src));
        }
    }
}