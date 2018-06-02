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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Loading;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class SymbolLoadingServiceTests
    {
        private IConfigurationService cfgSvc;
        private DecompilerEventListener eventListener;
        private IFileSystemService fsSvc;
        private MockRepository mr;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            this.eventListener = mr.StrictMock<DecompilerEventListener>();
            cfgSvc = mr.StrictMock<IConfigurationService>();
            fsSvc = mr.StrictMock<IFileSystemService>();
            sc.AddService<IConfigurationService>(cfgSvc);
            sc.AddService<IFileSystemService>(fsSvc);
            sc.AddService<DecompilerEventListener>(eventListener);

        }

        [Test]
        public void SymLoaderSvc_Load()
        {
            cfgSvc.Expect(c => c.GetSymbolSources()).Return(new List<SymbolSource>
            {
                new SymbolSourceDefinition
                {
                     Name = "Bob",
                     Description = "Bob symbol loader",
                     TypeName = typeof(BobLoader).AssemblyQualifiedName,
                }
            });
            fsSvc.Expect(f => f.ReadAllBytes("foo.pdb")).Return(new byte[] { 1, 2, 3, 4 });
            mr.ReplayAll();

            var symLdrSvc = new SymbolLoadingService(sc);
            var symSrc = symLdrSvc.GetSymbolSource("foo.pdb");
            var syms = symSrc.GetAllSymbols();

            Assert.AreEqual(1, syms.Count);

        }

        public class BobLoader : ISymbolSource
        {
            private IProcessorArchitecture arch;

            public BobLoader()
            {
                this.arch = MockRepository.GenerateStub<IProcessorArchitecture>();
                this.arch.Replay();
            }

            public bool CanLoad(string filename, byte[] fileContents)
            {
                return true;
            }

            public void Dispose()
            {
            }

            public List<ImageSymbol> GetAllSymbols()
            {
                return new List<ImageSymbol>
                {
                    new ImageSymbol(arch, Address.Ptr64(0x12340000))
                    {
                        Name = "MyFunction",
                        Type = SymbolType.Procedure,
                        Signature = new SerializedSignature
                        {
                            Arguments = new Argument_v1[]
                            {
                                new Argument_v1("arg1", PrimitiveType_v1.Int32(), new StackVariable_v1(), false),
                                new Argument_v1("arg2", new PointerType_v1(PrimitiveType_v1.Char8()) { PointerSize = 4 }, new StackVariable_v1(), false),
                            },
                            ReturnValue = new Argument_v1
                            {
                                Type = PrimitiveType_v1.Int32()
                            }
                        }
                    }
                };
            }
        }
    }
}
