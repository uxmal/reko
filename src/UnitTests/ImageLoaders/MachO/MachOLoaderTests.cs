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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.ImageLoaders.MachO;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.MachO
{
    [TestFixture]
    public class MachOLoaderTests
    {
        private ServiceContainer sc;
        private MockRepository mr;
        private ImageWriter writer;
        private MachOLoader ldr;
        private IConfigurationService cfgSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            cfgSvc = mr.Stub<IConfigurationService>();
            sc.AddService<IConfigurationService>(cfgSvc);
        }

        private void Given_Le64Header(uint cpu, uint loaders)
        {
            writer = new LeImageWriter(new byte[1000]);
            writer.WriteLeUInt32(0xFEEDFACF);
            WriteHeader(cpu, loaders);
            writer.WriteLeUInt32(0);    // reserved
        }

        private void WriteHeader(uint cputype, uint ncmds)
        {
            writer.WriteUInt32(cputype);
            writer.WriteUInt32(0);  // cpusubtype);
            writer.WriteUInt32(0); // filetype);
            writer.WriteUInt32(ncmds);
            writer.WriteUInt32(0); // sizeofcmds);
            writer.WriteUInt32(0);  //flags);
        }

        private void When_CreateLoader()
        {
            ldr = new MachOLoader(sc, "foo.o", writer.Bytes);
        }

        [Test]
        public void Moldr_LoadEmptyLe64()
        {
            Given_Le64Header(MachOLoader.CPU_TYPE_X86_64, 1);
            Given_x86Arch();

            mr.ReplayAll();

            When_CreateLoader();
            ldr.Load(Address.Ptr64(0x10000));
        }

        private void Given_x86Arch()
        {
            var arch = mr.Stub<IProcessorArchitecture>();
            cfgSvc.Stub(c => c.GetArchitecture("x86-protected-64")).Return(arch);
        }
    }
}
