using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class Scanner_x86Tests
    {
        private IntelArchitecture arch;

        private void BuildTest16(Action<IntelAssembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            var emitter = new IntelEmitter();
            var entryPoints = new List<EntryPoint>();
            var addrBase = new Address(0xC00, 0);
            var asm = new IntelAssembler(arch, addrBase, emitter, entryPoints);
            asmProg(asm);

            var scanner = new Scanner(
                arch,
                new ProgramImage(addrBase, emitter.Bytes),
                new MsdosPlatform(arch),
                new Dictionary<Address, ProcedureSignature>(),
                new FakeDecompilerEventListener());
            scanner.EnqueueEntryPoint(new EntryPoint(addrBase, arch.CreateProcessorState()));
            scanner.ProcessQueue();
            DumpProgram(scanner);
        }

        private void DumpProgram(Scanner scanner)
        {
            foreach (Procedure proc in scanner.Procedures.Values)
            {
                proc.Write(true, Console.Out);
                Console.Out.WriteLine();
            }
        }

        [Test]
        public void NestedProcedures()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Mov(m.ax, 3);
                m.Push(m.ax);
                m.Call("p2");
                m.Mov(m.MemW(Registers.ds, MachineRegister.None, 300), m.ax);
                m.Ret();

                m.Proc("p2");
                m.Add(m.ax, 2);
                m.Ret();
            });
        }
    }
}
