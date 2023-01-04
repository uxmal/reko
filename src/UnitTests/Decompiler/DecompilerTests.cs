#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Decompiler
{
    using Decompiler = Reko.Decompiler;

    [TestFixture]
    public class DecompilerTests
    {
        Decompiler decompiler;
        private ServiceContainer sc;
        private Mock<IFileSystemService> fsSvc;

        [SetUp]
        public void Setup()
        {
            fsSvc = new Mock<IFileSystemService>();
            var cfgSvc = new Mock<IConfigurationService>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            var host = new FakeDecompiledFileService();
            sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            sc.AddService<IDecompiledFileService>(host);
            sc.AddService<IConfigurationService>((IConfigurationService)cfgSvc.Object);
            sc.AddService<ITypeLibraryLoaderService>(tlSvc.Object);
        }

        //$REVIEW: is this even used?
        [Test]
        public void Dec_LoadCallSignatures()
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            Program program = new Program { 
                SegmentMap = new SegmentMap(Address.SegPtr(0xC00, 0)),
                Architecture = arch,
                Platform = new MsdosPlatform(sc, arch)
            };

            decompiler = new Decompiler(new Project
                {
                    Programs = { program },
                },
                sc);

            SerializedSignature sig = new SerializedSignature();
            sig.Arguments = new Argument_v1[] {
			    new Argument_v1 {
			        Kind = new Register_v1("ds")
                },
                new Argument_v1 {
			        Kind = new Register_v1("bx"),
                }
            };
            var al = new List<SerializedCall_v1> {
                new SerializedCall_v1(Address.SegPtr(0x0C32, 0x3200), sig)
            };
            var sigs = decompiler.LoadCallSignatures(program, al);

            FunctionType ps = sigs[Address.SegPtr(0x0C32, 0x3200)];
            Assert.IsNotNull(ps, "Expected a call signature for address");
        }
    }
}
