#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Environments.Msdos;
using Reko.Loading;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    public class RewriterTestBase
    {
        private ServiceContainer sc;
        private string configFile;
        protected IAssembler asm;
        protected Program program;
        protected Scanner scanner;
        protected Address baseAddress;

        public RewriterTestBase()
        {
            baseAddress = Address.SegPtr(0x0C00, 0);
        }

        [SetUp]
        public void SetUp()
        {
            sc = new ServiceContainer();
            sc.AddService<ITestGenerationService>(new UnitTestGenerationService(sc));
            sc.AddService<IFileSystemService>(new FileSystemService());
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            program = new Program() { Architecture = arch };
            asm = new X86TextAssembler(arch);
            configFile = null;
        }

        public string ConfigFile
        {
            get { return configFile; }
            set { configFile = value; }
        }

        protected Procedure DoRewrite(string code)
        {
            program = asm.AssembleFragment(baseAddress, code);
            program.Platform = new MsdosPlatform(sc, program.Architecture);
            DoRewriteCore();
            return program.Procedures.Values[0];
        }

        private void DoRewriteCore()
        {
            var cfgSvc = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            var eventListener = new FakeDecompilerEventListener();
            cfgSvc.Setup(c => c.GetEnvironment("ms-dos")).Returns(env.Object);
            env.Setup(e => e.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(e => e.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            sc.AddService(cfgSvc.Object);
            sc.AddService(tlSvc.Object);

            Project project = LoadProject();
            project.Programs.Add(this.program);
            scanner = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, this.program, eventListener),
                sc);
            var ep = ImageSymbol.Procedure(this.program.Architecture, baseAddress);
            this.program.EntryPoints.Add(ep.Address, ep);
            var program = project.Programs[0];
            foreach (var sp in program.User.Procedures.Values)
            {
                scanner.EnqueueUserProcedure(program.Architecture, sp);
            }
            scanner.ScanImage();
        }

        private Project LoadProject()
        {
            Project project = null;
            if (configFile is not null)
            {
                var absFile = FileUnitTester.MapTestPath(configFile);
                var fsSvc = sc.RequireService<IFileSystemService>();
                using Stream stm = fsSvc.CreateFileStream(absFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                project = new ProjectLoader(
                    null,
                    new Loader(sc),
                    ImageLocation.FromUri(absFile),
                    new FakeDecompilerEventListener())
                .LoadProject(stm);
            }
            else
            {
                project = new Project();
            }
            return project;
        }

        protected void DoRewriteFile(string relativePath)
        {
            using (var stm = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                var lr = asm.Assemble(baseAddress, relativePath, stm);
                program.Memory = new ByteProgramMemory(lr.SegmentMap);
                program.SegmentMap = lr.SegmentMap;
                program.ImageMap = lr.ImageMap;
                program.Platform = lr.Platform ?? new MsdosPlatform(sc, lr.Architecture);
            }
            DoRewriteCore();
        }
    }

    /// <summary>
    /// Unit Tests for the Intel code rewriter.
    /// </summary>

    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        [Test]
        public void RwSimpleTest()
        {
            DoRewrite(
                @"	.i86
	mov	ax,0x0000
	mov	cx,0x10
	add	ax,cx
	ret
");

            Assert.AreEqual(1, program.Procedures.Count);
            Procedure proc = program.Procedures.Values[0];
            Assert.AreEqual(3, proc.ControlGraph.Blocks.Count);		// Entry, code, Exit

            Block block = new List<Block>(proc.ControlGraph.Successors(proc.EntryBlock))[0];
            Assert.AreEqual(5, block.Statements.Count);
            Assignment instr1 = (Assignment) block.Statements[0].Instruction;
            Assert.AreEqual("ax = 0<16>", block.Statements[0].Instruction.ToString());

            Assert.AreSame(new List<Block>(proc.ControlGraph.Successors(block))[0], proc.ExitBlock);
        }

        [Test]
        public void X86Rw_real_IfTest()
        {
            Procedure proc = DoRewrite(
                @"	.i86
	cmp	bx,ax
	jnz	not_eq

	mov	cx,3
	jmp	join
not_eq:
	mov	cx,2
join:
	ret
");
            Assert.AreEqual(6, proc.ControlGraph.Blocks.Count);
            StringWriter sb = new StringWriter();
            proc.Write(true, sb);
        }

        [Test]
        public void RwDeadConditionals()
        {
            DoRewriteFile("Fragments/small_loop.asm");
            Procedure proc = program.Procedures.Values[0];
            using (FileUnitTester fut = new FileUnitTester("Arch/X86/RwDeadConditionals.txt"))
            {
                proc.Write(true, fut.TextWriter);
                fut.AssertFilesEqual();
            }
            Assert.AreEqual(5, proc.ControlGraph.Blocks.Count);
        }

        [Test]
        public void RwPseudoProcs()
        {
            DoRewriteFile("Fragments/pseudoprocs.asm");
            Procedure proc = program.Procedures.Values[0];
            using (FileUnitTester fut = new FileUnitTester("Arch/X86/RwPseudoProcs.txt"))
            {
                proc.Write(true, fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void RwAddSubCarries()
        {
            RunTest("Fragments/addsubcarries.asm", "Arch/X86/RwAddSubCarries.txt");
        }

        [Test]
        public void RwLongAddSub()
        {
            RunTest("Fragments/longaddsub.asm", "Arch/X86/RwLongAddSub.txt");
        }

        [Test]
        public void RwEnterLeave()
        {
            RunTest("Fragments/enterleave.asm", "Arch/X86/RwEnterLeave.txt");
        }

        [Test]
        public void RwReg00003()
        {
            RunTest("Fragments/regressions/r00003.asm", "Arch/X86/RwReg00003.txt");
        }

        [Test]
        public void RwReg00005()
        {
            RunTest("Fragments/regressions/r00005.asm", "Arch/X86/RwReg00005.txt");
        }

        [Test]
        public void RwSequenceShifts()
        {
            RunTest("Fragments/sequenceshift.asm", "Arch/X86/RwSequenceShifts.txt");
        }

        [Test]
        public void RwLogical()
        {
            RunTest("Fragments/logical.asm", "Arch/X86/RwLogical.txt");
        }

        [Test]
        public void RwNegsNots()
        {
            RunTest("Fragments/negsnots.asm", "Arch/X86/RwNegsNots.txt");
        }

        [Test]
        public void RwFpuArgs()
        {
            RunTest("Fragments/multiple/fpuArgs.asm", "Arch/X86/RwFpuArgs.txt");
        }

        [Test]
        public void RwFpuOps()
        {
            RunTest("Fragments/fpuops.asm", "Arch/X86/RwFpuOps.txt");
        }

        [Test]
        public void RwFpuReversibles()
        {
            RunTest("Fragments/fpureversibles.asm", "Arch/X86/RwFpuReversibles.txt");
        }

        private void RunTest(string sourceFile, string outputFile)
        {
            DoRewriteFile(sourceFile);
            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                foreach (Procedure proc in program.Procedures.Values)
                {
                    proc.Write(true, fut.TextWriter);
                    fut.TextWriter.WriteLine();
                }
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void RwEvenOdd()
        {
            RunTest("Fragments/multiple/even_odd.asm", "Arch/X86/RwEvenOdd.txt");
        }

        [Test]
        public void RwPushPop()
        {
            RunTest("Fragments/pushpop.asm", "Arch/X86/RwPushPop.txt");
        }
    }
}
