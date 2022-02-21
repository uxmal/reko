#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmLoaderTests
    {
        private WasmLoader ldr;

        void Create_Loader(params byte[] bytes)
        {
            ldr = new WasmLoader(null, ImageLocation.FromUri("file:foo.wasm"), bytes);
        }

        void Create_Loader(string hexBytes)
        {
            Create_Loader(BytePattern.FromHexBytes(hexBytes));
        }

        [Test]
        public void WasmLdr_Header()
        {
            Create_Loader(
                0x00, 0x61, 0x73, 0x6D, // Magic cookie
                0x0B, 0x00, 0x00, 0x00 // version number.
            );

            var rdr = ldr.LoadHeader();
            Assert.AreEqual(8, rdr.Offset);
        }

        [Test]
        public void WasmLdr_LoadTypeSection()
        {
            var bytes = new byte[]
            {
                0x01,   // type  section
                0x05,   // payload length
                0x01,   // One type
                    0x60, 0x01, 0x7F, 0x00      // fn taking one int arg
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var ts = (TypeSection)section;
            Assert.AreEqual(1, ts.Types.Count);
            Assert.AreEqual("(fn void (word32))", ts.Types[0].ToString());
        }

        [Test]
        public void WasmLdr_LoadImportSection()
        {
            var bytes = new byte[]
            {
                0x02, 0x0C,     // import section
                0x01,           // one import
                    0x03, 0x65, 0x6E, 0x76,         // env
                    0x04, 0x70, 0x75, 0x74, 0x73,   // puts
                    0x00, 0x42,     // kind of external
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var imps = (ImportSection)section;
            Assert.AreEqual(1, imps.Imports.Count);
            Assert.AreEqual("env", imps.Imports[0].Module);
            Assert.AreEqual("puts", imps.Imports[0].Field);
            Assert.AreEqual(0x42, imps.Imports[0].Index);
        }

        [Test]
        public void WasmLdr_LoadFunctionSection()
        {
            var bytes = new byte[]
            {
                0x03, 0x02,     // Function section
                0x01,           // one function declaration
                    0x42
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var funcs = (FunctionSection)section;
            Assert.AreEqual(1, funcs.Declarations.Count);
            Assert.AreEqual(0x42, funcs.Declarations[0]);
        }

        [Test]
        public void WasmLdr_LoadTableSection()
        {
            var bytes = new byte[]
            {
                0x04, 0x04,     // Table section
                0x01,           // one table declaration
                    0x70, 0x00, 0x00
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var tables = (TableSection)section;
            Assert.AreEqual(1, tables.Tables.Count);
            Assert.AreEqual("(table 0 0 anyfunc)", tables.Tables[0].ToString());
        }

        [Test]
        public void WasmLdr_LoadMemorySection()
        {
            var bytes = new byte[]
            {
                0x05, 0x04,     // Memory section
                0x01,           // one table declaration
                    0x01, 0x00, 0x10
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var mems = (MemorySection)section;
            Assert.AreEqual(1, mems.Memories.Count);
            Assert.AreEqual("(memory 0 16)", mems.Memories[0].ToString());
        }

/*

07 12		;; exports
02 06 6D 65 6D 6F 72 79 
   02 00
   05 68 65 6C 6C 6F
   00 01

0A 08 */

        [Test]
        public void WasmLdr_LoadExportSection()
        {
            var bytes = new byte[]
            {
0x07, 0x12,                 //       ; ; exports
0x02, 0x06, 0x6D, 0x65, 0x6D, 0x6F, 0x72, 0x79,
      0x02, 0x00,
      0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F,
      0x00, 0x01, 

//0A 08
            };

            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var mems = (ExportSection)section;
            Assert.AreEqual(2, mems.ExportEntries.Count);
            Assert.AreEqual("(export \"memory\" (memory 0))", mems.ExportEntries[0].ToString());
            Assert.AreEqual("(export \"hello\" (func 1))", mems.ExportEntries[1].ToString());
        }

        [Test]
        public void WasmLdr_LoadCodeSection()
        {
            var bytes = new byte[]
            {
                0x0A, 0x08,
                0x01,           // 1 function body
                    0x06, 0x00, 0x41, 0x10, 0x10, 0x00, 0x0B
            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var codes = (CodeSection)section;
            Assert.AreEqual(1, codes.Functions.Count);
            Assert.AreEqual(new byte[] {  0x41, 0x10, 0x10, 0x00, 0x0B }, codes.Functions[0].ByteCode);
        }

        [Test]
        public void WasmLdr_LoadCodeSection_2_fns()
        {
            var bytes = new byte[]
            {
                0x0A, 0x0C,
                0x02,           // 1 function body
                    0x06,
                        0x00, 0x41, 0x10, 0x10, 0x00, 0x0B,
                    0x03,
                        0x00, 0x02, 0x0B,

            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var codes = (CodeSection)section;
            Assert.AreEqual(2, codes.Functions.Count);
            Assert.AreEqual(new byte[] { 0x41, 0x10, 0x10, 0x00, 0x0B }, codes.Functions[0].ByteCode);
        }

        [Test]
        public void WasmLdr_LoadDataSection()
        {
            var bytes = new byte[]
            {
                0x0B, 0x1C,
                0x02,
                      0x00, 0x41, 0x04, 0x0B, 0x04,
                            0x60, 0x42, 0x0F, 0x00,
                      0x00, 0x41, 0x10, 0x0B, 0x0D,
                            0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64, 0x21, 0x00
            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var codes = (DataSection)section;
            Assert.AreEqual(2, codes.Segments.Count);
            Assert.AreEqual(0, codes.Segments[0].MemoryIndex);
            Assert.AreEqual(4, codes.Segments[0].Offset);
            Assert.AreEqual(new byte[] { 0x60, 0x42, 0x0F, 0x00 } , codes.Segments[0].Bytes);
        }

        [Test]
        public void WasmLdr_LoadGlobalSection()
        {
            var bytes = new byte[]
            {
                0x06, 0x06,
                0x01,
                    0x7F, 0x01, 
                          0x23, 0x02, 0x0B,
                    // global_type
                    // init_expr
            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var codes = (GlobalSection)section;
            Assert.AreEqual(1, codes.Globals.Count);
        }

        [Test]
        public void WasmLdr_LoadCustomSection()
        {
            var bytes = new byte[]
            {
                0x00, 0x05,
                      0x02, 0x43, 0x75, 
                      0xAA, 0xBB
            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var custom = (CustomSection)section;
            Assert.AreEqual("Cu", custom.Name);
            Assert.AreEqual(2, custom.Bytes.Length);
            Assert.AreEqual(0xBB, custom.Bytes[1]);
        }

        [Test]
        public void WasmLdr_Load_function_locals()
        {
            var bytes = new byte[]
            {
                0x0A, 0x08,
                0x01, 0x06, // function body
                      0x02, // local entries
                            0x01, 0x7F,     // one i32
                            0x02, 0x7E,     // two i64's
                      0x0B
            };
            Create_Loader();
            var rdr = new WasmImageReader(bytes);
            var section = ldr.LoadSection(rdr);
            var codes = (CodeSection)section;
            Assert.AreEqual(1, codes.Functions.Count);
            var func = codes.Functions[0];
            Assert.AreEqual(new byte[] { 0x0B }, func.ByteCode);
        }
    }
}
