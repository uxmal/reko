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

using NUnit.Framework;
using Reko.Core;
using Reko.ImageLoaders.WebAssembly;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmFileTests
    {
        private readonly WasmArchitecture arch;
        private WasmFile file;

        public WasmFileTests()
        {
            this.arch = new WasmArchitecture(new ServiceContainer(), "wasm", new());
        }

        [SetUp]
        public void Setup()
        {
            this.file = null;
        }

        private void When_ConstructFile(List<Section> sections)
        {
            this.file = new WasmFile(sections);
        }

        [Test]
        public void Wasf_BuildFunctionIndex()
        {
            var sections = new List<Section>
            {
                new ImportSection(".imports", new byte[0], new List<Import>
                {
                    new Import { Module="env", Name="malloc", Index=0, Type = SymbolType.ExternalProcedure }
                }),
                new ExportSection(".exports", new byte[0], new List<ExportEntry>
                {
                    new ExportEntry { Name = "ble", Type = SymbolType.Procedure, Index = 1 },
                }),
                new CodeSection(".text", new byte[0], new List<FunctionDefinition>
                {
                    new FunctionDefinition(0, 42, Array.Empty<LocalVariable>(), Array.Empty<byte>())
                }),
                new FunctionSection(".funcs", new byte[0],new List<uint>
                {
                    0
                })
            };
            When_ConstructFile(sections);

            Assert.AreEqual(2, file.FunctionIndex.Count);
            Assert.AreEqual("malloc", file.FunctionIndex[0].Name);
            Assert.AreEqual("ble", file.FunctionIndex[1].Name);
        }
    }
}
