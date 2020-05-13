using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Avr.Avr32
{
    internal class Avr32Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Avr32Instruction> dasm;
        private Avr32Instruction instr;
        private InstrClass iclass;
        private RtlEmitter m;

        public Avr32Rewriter(Avr32Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Avr32Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = this.instr.InstructionClass;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                switch (this.instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.invalid:
                    m.Invalid();
                    break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

#if DEBUG
        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var hexInstr = string.Join("", r2.ReadBytes(dasm.Current.Length)
                .Select(b => $"{b:X2}"));
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void Avr32Rw_" + dasm.Current.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            Given_Instruction(");
            Debug.Write("\"{0}\"}", hexInstr);
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine($"                \"0|L--|{dasm.Current.Address}({dasm.Current.Length}): 1 instructions\",");
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        [Conditional("DEBUG")]
        public void EmitUnitTest()
        {
        }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}