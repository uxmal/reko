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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Output;
using System.Text;

namespace Reko.UnitTests.Arch.Pdp.Memory
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class Word36MemoryAreaTests
    {
        private class TestOutputDevice : IMemoryFormatterOutput
        {
            private readonly StringBuilder sb;

            public TestOutputDevice()
            {
                sb = new StringBuilder();
            }

            public string Actual => sb.ToString();

            public void BeginLine()
            {
            }

            public void EndLine(Constant[] bytes)
            {
                sb.AppendLine();
            }

            public void RenderAddress(Address addr)
            {
                sb.Append(addr);
            }

            public void RenderFillerSpan(int nChunks, int nCharsPerChunk)
            {
                sb.Append(' ', (1 + nCharsPerChunk) * nChunks);
            }

            public void RenderUnit(Address addr, string sUnit)
            {
                sb.Append(' ');
                sb.Append(sUnit);
            }

            public void RenderUnitsAsText(int prePadding, string sBytes, int postPadding)
            {
                sb.Append(' ', prePadding + 1);
                sb.Append(sBytes);
                sb.Append(' ', postPadding);
            }
        }

        [Test]
        public void W36Mem_Dump()
        {
            var mem = new Word36MemoryArea(new Address18(0x1234), new ulong[]
            {
                Pdp10Architecture.Ascii7("Hello"),
                Pdp10Architecture.Ascii7(" worl"),
                Pdp10Architecture.Ascii7("d! Th"),
                Pdp10Architecture.Ascii7("is is"),

                Pdp10Architecture.Ascii7(" 7-bi"),
                Pdp10Architecture.Ascii7("t ASC"),
                Pdp10Architecture.Ascii7("II!"),
                0
            });

            var rdr = mem.CreateBeReader(0);
            var output = new TestOutputDevice();
            mem.Formatter.RenderMemory(rdr, Encoding.ASCII, output);

            var sExp =
            #region Expected
@"011064 221456633157 101676774554 310412025150 323632032363 Hello world! This is
011070 100672670551 350404064703 223112040000 000000000000  7-bit ASCII!.......
";
            #endregion

            Assert.AreEqual(sExp, output.Actual);
        }

        [Test]
        public void W36Mem_Dump_offset_by_word()
        {
            var mem = new Word36MemoryArea(new Address18(0x1234), new ulong[]
            {
                Pdp10Architecture.Ascii7("Hello"),
                Pdp10Architecture.Ascii7(" worl"),
                Pdp10Architecture.Ascii7("d! Th"),
                Pdp10Architecture.Ascii7("is is"),

                Pdp10Architecture.Ascii7(" 7-bi"),
                Pdp10Architecture.Ascii7("t ASC"),
                Pdp10Architecture.Ascii7("II!"),
                0
            });

            // start at one word past the beginning of the memory area.
            var rdr = mem.CreateBeReader(1);
            var output = new TestOutputDevice();
            mem.Formatter.RenderMemory(rdr, Encoding.ASCII, output);

            var sExp =
            #region Expected
@"011065              101676774554 310412025150 323632032363       world! This is
011070 100672670551 350404064703 223112040000 000000000000  7-bit ASCII!.......
";
            #endregion

            Assert.AreEqual(sExp, output.Actual);
        }
    }
}
