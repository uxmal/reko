#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.ImageLoaders.Elf;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Elf
{
    public class ElfTests
    {
        protected MockRepository mr;

        protected IProcessorArchitecture arch;
        private MemoryStream segnametab;
        protected MemoryStream symbolStringtab;
        protected List<Elf32_Sym> symbols;
        protected ServiceContainer sc;
        protected List<ObjectSection> objectSections;
        protected IConfigurationService cfgSvc;
        private MemoryStream binaryContents;

        public class ObjectSection
        {
            public string Name;
            public byte[] Content;
            public uint Flags;
            public ushort Link;
            public SectionHeaderType Type;
            public uint Offset;
            public uint ElementSize;
        }

        public virtual void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();

            this.cfgSvc = mr.Stub<IConfigurationService>();
            this.sc.AddService(typeof(IConfigurationService), cfgSvc);
            this.segnametab = new MemoryStream();
            this.segnametab.WriteByte(0);

            this.symbolStringtab = new MemoryStream();
            this.symbolStringtab.WriteByte(0);
            this.symbols = new List<Elf32_Sym> { new Elf32_Sym() };

            this.binaryContents = new MemoryStream();


            this.objectSections = new List<ObjectSection>
            {
                new ObjectSection { Name = "" },        // dummy section always present
                new ObjectSection { Name = ".shstrtab", Type = SectionHeaderType.SHT_STRTAB, Flags = 0 },
            };
        }

        protected void Given_BeArchitecture()
        {
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.arch.Stub(a => a.CreateImageWriter()).Do(new Func<ImageWriter>(() => new BeImageWriter()));
            this.arch.Replay();
        }

        protected int Given_SegName(string segname)
        {
            int i = (int)segnametab.Position;
            var bytes = Encoding.ASCII.GetBytes(segname);
            segnametab.Write(bytes, 0, bytes.Length);
            segnametab.WriteByte(0);
            return i;
        }

        protected void Given_Symbol(
            string name,
            uint st_value,
            uint st_size,
            byte st_info,
            ushort st_shndx)
        {
            var iName = (uint)symbolStringtab.Position;
            var bytes = Encoding.ASCII.GetBytes(name);
            symbolStringtab.Write(bytes, 0, bytes.Length);
            symbolStringtab.WriteByte(0);
            symbols.Add(new Elf32_Sym
            {
                st_name = iName,
                st_value = st_value,
                st_size = st_size,
                st_info = st_info,
                st_shndx = st_shndx
            });
        }


        protected void Given_Section(string name, SectionHeaderType type, uint flags, byte[] blob)
        {
            var os = new ObjectSection
            {
                Name = name,
                Type = type,
                Flags = flags,
                Content = blob,
            };
            objectSections.Add(os);
        }

        protected static uint Align(uint n)
        {
            return 4 * (((n + 3) / 4));
        }

        protected static void Align(ImageWriter strtab)
        {
            while (strtab.Position % 4 != 0)
                strtab.WriteByte(0);
        }

        private static void Align(MemoryStream strtab)
        {
            // Align the string table.
            while (strtab.Position % 4 != 0)
                strtab.WriteByte(0);
        }
    }
}
