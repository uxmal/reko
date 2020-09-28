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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.ImageLoaders.MzExe;
using Reko.ImageLoaders.MzExe.Borland;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.MzExe.Borland
{
    [TestFixture]
    public class SymbolLoaderTests
    {
        private LeImageWriter writer;
        private ExeImageLoader exeLoader;
        private int offNames;
        private X86ArchitectureReal arch;

        [SetUp]
        public void Setup()
        {
            this.writer = null;
            this.exeLoader = null;
            this.offNames = -1;
            this.arch = new X86ArchitectureReal("x86-real-16");
        }

        private void Given_MzExeProgram(uint size)
        {
            this.writer = new LeImageWriter();
            writer.WriteBytes(0, size);
            var pos = writer.Position;
            writer.Position = 0;
            writer.WriteString("MZ", Encoding.ASCII);
            uint cPages = size / ExeImageLoader.CbPageSize;
            uint cbLastPage = size % ExeImageLoader.CbPageSize;
            if (cbLastPage > 0)
                ++cPages;
            writer.WriteLeUInt16((ushort)cbLastPage);
            writer.WriteLeUInt16((ushort)cPages);
            writer.Position = pos;
        }

        public void Given_DebugHeader(short nSymbols, short nTypes)
        {
            writer.WriteLeUInt16(SymbolLoader.MagicNumber);  // magic_number
            writer.WriteLeUInt16(0);  // version_id
            // Remember this position so we can backpatch later.
            this.offNames = writer.Position;
            writer.WriteLeUInt32(0);  // names
            writer.WriteLeUInt16(0);  // names_count
            writer.WriteLeInt16(nTypes);  // types_count
            writer.WriteLeUInt16(0);  // members_count
            writer.WriteLeInt16(nSymbols);  // symbols_count
            writer.WriteLeUInt16(0);  // globals_count
            writer.WriteLeUInt16(0);  // modules_count
            writer.WriteLeUInt16(0);  // locals_count
            writer.WriteLeUInt16(0);  // scopes_count
            writer.WriteLeUInt16(0);  // lines_count
            writer.WriteLeUInt16(0);  // source_count
            writer.WriteLeUInt16(0);  // segment_count
            writer.WriteLeUInt16(0);  // correlation_count
            writer.WriteUInt32(0);  // image_size
            writer.WriteUInt32(0);  // /*void far * */debugger_hook
            writer.WriteByte(0);  // program_flags
            writer.WriteLeUInt16(0);  // stringsegoffset
            writer.WriteLeUInt16(0);  // data_count
            writer.WriteByte(0);  // filler
            writer.WriteLeUInt16(0x00);  // extension_size
        }

        private void Given_GenericSymbol(short iName, ushort seg, ushort offset)
        {
            writer.WriteLeInt16(iName);
            writer.WriteLeInt16(0x00);  // Untyped.
            writer.WriteLeUInt16(offset);
            writer.WriteLeUInt16(seg);
            writer.WriteByte(0);        // static symbol
        }

        private void Given_FnSymbol(short iName, ushort seg, ushort offset, ushort iType)
        {
            writer.WriteLeInt16(iName);
            writer.WriteLeUInt16(iType);  // Typed.
            writer.WriteLeUInt16(offset);
            writer.WriteLeUInt16(seg);
            writer.WriteByte(0);        // static symbol
        }

        private void Given_FnType(ushort iName, ushort iTypeReturn)
        {
            writer.WriteByte(0x23); // TID_FUNCTION
            writer.WriteBeUInt16(iName);
            writer.WriteBeUInt16(0);
            writer.WriteByte(0);    // C near fn.
            writer.WriteBeUInt16(iTypeReturn);
        }

        private void Given_Names(params string[] names)
        {
            var iNamesBegin = writer.Position;
            foreach (var name in names)
            {
                writer.WriteString(name, Encoding.ASCII);
                writer.WriteByte(0);
            }
            var iNamesEnd = writer.Position;
            writer.WriteLeUInt32((uint)this.offNames, (uint)(iNamesEnd - iNamesBegin));
        }

        private void Given_ExeLoader()
        {
            this.exeLoader = new ExeImageLoader(null, "FOO.EXE", writer.ToArray());
            this.exeLoader.ReadCommonExeFields();
        }

        [Test(Description = "This binary has no Borland symbols.")]
        public void BorSymLdr_Fail()
        {
            Given_MzExeProgram(0x0430);

            Given_ExeLoader();

            var borsymldr = new SymbolLoader(arch, exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsFalse(borsymldr.LoadDebugHeader());
        }

        [Test]
        public void BorSymLdr_HasHeader()
        {
            Given_MzExeProgram(0x0430);
            Given_DebugHeader(0, 0);
            Given_ExeLoader();

            var borsymldr = new SymbolLoader(arch, exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsTrue(borsymldr.LoadDebugHeader());
        }

        [Test]
        public void BorSymLdr_GenericSymbols()
        {
            Given_MzExeProgram(0x0430);
            Given_DebugHeader(2, 0);
            Given_GenericSymbol(1, 0, 0x0101);
            Given_GenericSymbol(2, 0, 0x014F);
            Given_Names("fn1", "Function2");
            Given_ExeLoader();

            var borsymldr = new SymbolLoader(arch, exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsTrue(borsymldr.LoadDebugHeader());
            var syms = borsymldr.LoadSymbols().Values.OrderBy(s => s.Address).ToArray();
            Assert.AreEqual(2, syms.Length);
            Assert.AreEqual("fn1", syms[0].Name);
            Assert.AreEqual("Function2", syms[1].Name);
        }

        [Test]
        public void BorSymLdr_FunctionSymbols()
        {
            Given_MzExeProgram(0x0430);
            Given_DebugHeader(2, 2);
            Given_FnSymbol(1, 0x0000, 0x0101, 1);
            Given_FnSymbol(2, 0x0000, 0x014F, 2);
            Given_FnType(0x0000, 0x0000);
            Given_FnType(0x0000, 0x0000);
            Given_Names("fn1", "Function2");
            Given_ExeLoader();

            var borsymldr = new SymbolLoader(arch, exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsTrue(borsymldr.LoadDebugHeader());
            var syms = borsymldr.LoadSymbols().Values.OrderBy(s => s.Address).ToArray();
            Assert.AreEqual(2, syms.Length);
            Assert.AreEqual("fn1", syms[0].Name);
            Assert.AreEqual("Function2", syms[1].Name);
        }
    }
}
