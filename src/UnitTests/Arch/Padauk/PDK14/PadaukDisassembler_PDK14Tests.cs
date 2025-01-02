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
using Reko.Arch.Padauk;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Padauk.PDK14
{
    [TestFixture]
    public class PadaukDisassembler_PDK14Tests : DisassemblerTestBase<PadaukInstruction>
    {
        private readonly PadaukArchitecture arch;
        private readonly Address addrLoad;

        public PadaukDisassembler_PDK14Tests()
        {
            arch = new PadaukArchitecture(
                CreateServiceContainer(),
                "padauk",
                new() {
                    {   ProcessorOption.InstructionSet, "PDK14" }
                });
            addrLoad = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override PadaukInstruction DisassembleHexBytes(string hexBytes)
        {
            var mem = HexStringToMemoryArea(hexBytes);
            return Disassemble(mem);
        }

        private MemoryArea HexStringToMemoryArea(string sBytes)
        {
            int shift = 12;
            int bb = 0;
            var words = new List<ushort>();
            for (int i = 0; i < sBytes.Length; ++i)
            {
                char c = sBytes[i];
                if (BytePattern.TryParseHexDigit(c, out byte b))
                {
                    bb |= b << shift;
                    shift -= 4;
                    if (shift < 0)
                    {
                        words.Add((ushort) bb);
                        shift = 12;
                        bb = 0;
                    }
                }
            }
            return new Word16MemoryArea(LoadAddress, words.ToArray());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Pdk14Dis_and_m_a()
        {
            AssertCode("and\t[0x1],a", "4A01");
        }

        [Test]
        public void Pdk14Dis_set0()
        {
            AssertCode("set0\tIO(0x2).1", "1C42");
        }
    }
}
