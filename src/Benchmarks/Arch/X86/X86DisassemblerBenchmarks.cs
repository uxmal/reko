using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Benchmarks.Arch.X86
{
    [Config(typeof(Config))]
    public class X86DisassemblerBenchmarks
    {
        private readonly ByteMemoryArea machineCode;
        private readonly X86ArchitectureFlat32 arch;

        public X86DisassemblerBenchmarks()
        {
            var mem = new byte[4096];
            var rnd = new Random(0x142621A2);
            rnd.NextBytes(mem);
            this.machineCode = new ByteMemoryArea(Address.Ptr32(0x10000), mem);
            this.arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new());
        }

        [Benchmark]
        public void DisassembleBytes()
        {
            var rdr = arch.CreateImageReader(machineCode, 0);
            var dasm = arch.CreateDisassembler(rdr);
            foreach (var instr in dasm)
                ;
        }

        public class Config : ManualConfig
        {
            public Config()
            {
                AddDiagnoser(MemoryDiagnoser.Default);
            }
        }
    }
}
