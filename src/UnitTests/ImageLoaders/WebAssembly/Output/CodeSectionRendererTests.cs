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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Output;
using Reko.ImageLoaders.WebAssembly;
using Reko.ImageLoaders.WebAssembly.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.WebAssembly.Output
{
    [TestFixture]
    public class CodeSectionRendererTests
    {
        private WasmArchitecture arch;
        private CodeSectionRenderer csr;
        private List<(LocalVariable[], int, int)> fns;
        private List<byte> bytes;
        private List<Section> sections;

        [SetUp]
        public void Setup()
        {
            this.arch = new WasmArchitecture(new ServiceContainer(), "wasm", new());
            this.bytes = new List<byte>();
            this.fns = new();
            this.bytes = new List<byte>();
            this.sections = new List<Section>();
        }

        private void Given_Function(int ifunc, params string[] hexbytes)
        {
            int iStart = this.bytes.Count;
            var byteCodes = BytePattern.FromHexBytes(string.Join("", hexbytes));
            this.bytes.AddRange(byteCodes);
            this.fns.Add((Array.Empty<LocalVariable>(), iStart, this.bytes.Count));
        }

        private void Given_CodeSectionRenderer()
        {
            var sectionBytes = bytes.ToArray();
            var functions = fns
                .Select(f => new FunctionDefinition(f.Item2, f.Item3, f.Item1, sectionBytes))
                .ToList();
            var section = new CodeSection(".text", sectionBytes, functions);
            sections.Add(section);
            var wasmFile = new WasmFile(sections);
            this.csr = new CodeSectionRenderer(arch, section, wasmFile);
        }

        private void AssertCode(string sExpected, StringWriter sw)
        {
            var sActual = sw.ToString();
            if (sActual != sExpected)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void WasmCsr_SimpleProc()
        {
            Given_Function(
                0,
                "41 2A",            // const 0x42
                "0F");              // return
            Given_CodeSectionRenderer();

            var sw = new StringWriter();
            var formatter = new TextFormatter(sw);
            formatter.UseTabs = false;
            csr.Render(null, null, formatter);

            var sExp =
            #region Expected
@"WASM functions
(func fn000000 (; 0 ;) retval??
    i32.const 0x2A
    return
)
";
            #endregion

            AssertCode(sExp, sw);
        }
    }
}
