#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Environments.Msdos;
using Reko.Loading;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Structure
{
    public class StructureTestBase
	{
		protected Program program;
        private ServiceContainer sc;

        protected Program RewriteProgramMsdos(string sourceFilename, Address addrBase)
		{
            var cfgSvc = MockRepository.GenerateStub<IConfigurationService>();
            var env = MockRepository.GenerateStub<OperatingEnvironment>();
            var tlSvc = MockRepository.GenerateStub<ITypeLibraryLoaderService>();
            cfgSvc.Stub(c => c.GetEnvironment("ms-dos")).Return(env);
            cfgSvc.Stub(c => c.GetSignatureFiles()).Return(new List<SignatureFile>());
            env.Stub(e => e.TypeLibraries).Return(new List<ITypeLibraryElement>());
            env.Stub(e => e.CharacteristicsLibraries).Return(new List<ITypeLibraryElement>());
            sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(cfgSvc);
            sc.AddService<DecompilerHost>(new FakeDecompilerHost());
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            sc.AddService<ITypeLibraryLoaderService>(tlSvc);
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureReal();

            program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(sourceFilename),
                new X86TextAssembler(sc, arch) { Platform = new MsdosPlatform(sc, arch) },
                addrBase);
            return RewriteProgram();
		}

        protected Program RewriteProgram32(string sourceFilename, Address addrBase)
        {
            sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(new FakeDecompilerConfiguration());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureFlat32();
            program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(sourceFilename),
                new X86TextAssembler(sc, arch) { Platform = new DefaultPlatform(sc, arch) },
                addrBase);
            return RewriteProgram();
        }

        protected Program RewriteX86RealFragment(string asmFragment, Address addrBase)
        {
            sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            var asm = new X86TextAssembler(sc, new X86ArchitectureReal());
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            program.EntryPoints.Add(
                addrBase,
                new ImageSymbol(addrBase));
            return RewriteProgram();
        }

        protected Program RewriteX86_32Fragment(string asmFragment, Address addrBase)
        {
            sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            var asm = new X86TextAssembler(sc, new X86ArchitectureFlat32());
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            program.EntryPoints.Add(
                addrBase,
                new ImageSymbol(addrBase));
            return RewriteProgram();
        }

        private Program RewriteProgram()
        {
            var eventListener = new FakeDecompilerEventListener();
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            var scan = new Scanner(
                program,
                importResolver,
                sc);
            foreach (ImageSymbol ep in program.EntryPoints.Values)
            {
                scan.EnqueueImageSymbol(ep, true);
            }
            scan.ScanImage();

            var dfa = new DataFlowAnalysis(program, importResolver, eventListener);
            dfa.AnalyzeProgram();

            return program;
        }
	}
}
