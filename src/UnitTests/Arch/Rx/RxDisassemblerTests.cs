#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.Renesas;
using Reko.Arch.Renesas.Rx;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Rx
{
    [TestFixture]
    public class RxDisassemblerTests : DisassemblerTestBase<RxInstruction>
    {
        public RxDisassemblerTests()
        {
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;

            var options = new Dictionary<string, object>()
            {
                { ProcessorOption.Endianness, "big" }
            };
            this.Architecture = new RxArchitecture(CreateServiceContainer(), "rxv2", options);
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture { get; }
        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void RxDis_abs_dest()
        {
            AssertCode("abs\tr4", "7E24");
        }

        [Test]
        public void RxDis_abs_src_dest()
        {
            AssertCode("abs\tr3,r4", "FC0F34");
        }

        [Test]
        public void RxDis_adc_simm8_dest()
        {
            AssertCode("adc\t#0FFFFFFFFh,r4", "FD7424 FF");
        }

        [Test]
        public void RxDis_adc_simm24_dest()
        {
            AssertCode("adc\t#0FFA6789Ah,r4", "FD7C24 A6789A");
        }

        [Test]
        public void RxDis_adc_simm32_dest()
        {
            AssertCode("adc\t#56789ABCh,r4", "FD7024 56789ABC");
        }

        [Test]
        public void RxDis_adc_src_dest()
        {
            AssertCode("adc\tr3,sp", "FC0B30");
        }

        [Test]
        public void RxDis_adc_src_dest_mem()
        {
            AssertCode("adc.l\t[r3],sp", "06A00230");
            AssertCode("adc.l\t0x7F*4[r3],sp", "06A102307F");
            AssertCode("adc.l\t0x8764*4[r3],sp", "06A202308764");
        }

        [Test]
        public void RxDis_add_imm()
        {
            AssertCode("add\t#0Fh,r3", "62F3");
        }

    }
}
