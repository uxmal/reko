#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Loading;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class SymbolLoadingServiceTests
    {
        private Mock<IConfigurationService> cfgSvc;
        private Mock<DecompilerEventListener> eventListener;
        private Mock<IFileSystemService> fsSvc;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            this.eventListener = new Mock<DecompilerEventListener>();
            cfgSvc = new Mock<IConfigurationService>();
            fsSvc = new Mock<IFileSystemService>();
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<IFileSystemService>(fsSvc.Object);
            sc.AddService<DecompilerEventListener>(eventListener.Object);

        }

        [Test]
        public void SymLoaderSvc_Load()
        {
            cfgSvc.Setup(c => c.GetSymbolSources()).Returns(new List<SymbolSourceDefinition>
            {
                new SymbolSourceDefinition
                {
                     Name = "Bob",
                     Description = "Bob symbol loader",
                     TypeName = typeof(BobLoader).AssemblyQualifiedName,
                }
            });
            fsSvc.Setup(f => f.ReadAllBytes("foo.pdb")).Returns(new byte[] { 1, 2, 3, 4 });

            var symLdrSvc = new SymbolLoadingService(sc);
            var symSrc = symLdrSvc.GetSymbolSource("foo.pdb");
            var syms = symSrc.GetAllSymbols();

            Assert.AreEqual(1, syms.Count);

        }

        public class BobLoader : ISymbolSource
        {
            private Mock<IProcessorArchitecture> arch;

            public BobLoader()
            {
                this.arch = new Mock<IProcessorArchitecture>();
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
                    ImageSymbol.Procedure(
                        arch.Object, 
                        Address.Ptr64(0x12340000),
                        "MyFunction",
                        signature: new SerializedSignature
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
                        })
                };
            }
        }
    }
}
