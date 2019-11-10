using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.Arch.Arc
{
    public class ARCompactRewriter : IEnumerable<RtlInstructionCluster>
    {
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
                var instrs = new List<RtlInstruction>();
                m = new RtlEmitter(instrs);
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(this.instr);
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = iclass,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static HashSet<Mnemonic> opcode_seen = new HashSet<Mnemonic>();

        void EmitUnitTest(ArcInstruction instr)
        {
            if (opcode_seen.Contains(instr.Mnemonic))
                return;
            opcode_seen.Add(instr.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void ARCompactRw_{0}()", instr.Mnemonic);
            Console.WriteLine("        {");

            if (instr.Length > 2)
            {
                var wInstr = r2.ReadUInt32();
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
        }
    }
}