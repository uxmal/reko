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

using Reko.Core.Memory;
using Reko.Core;
using System.Text;
using System;
using Moq;
using Reko.Core.Types;

namespace Reko.UnitTests.Decompiler.Scanning
{
    public class AbstractBaseFinderTests
    {
        protected ByteMemoryArea mem;
        protected Mock<IProcessorArchitecture> arch;

        public virtual void Setup()
        {
            this.mem = new ByteMemoryArea(Address.Ptr32(0), new byte[0x1000]);

            this.arch = new Mock<IProcessorArchitecture>(MockBehavior.Strict);
            this.arch.Setup(a => a.MemoryGranularity).Returns(8);
            this.arch.Setup(a => a.InstructionBitSize).Returns(16);
            this.arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            this.arch.Setup(a => a.Endianness).Returns(EndianServices.Big);
        }

        protected void Given_Bytes(int offset, string hexString)
        {
            var bytes = BytePattern.FromHexBytes(hexString);
            Array.Copy(bytes, 0, mem.Bytes, offset, bytes.Length);
        }

        protected void Given_String(int offset, string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            Array.Copy(bytes, 0, mem.Bytes, offset, bytes.Length);
        }

        protected void Given_Pointer(int offset, uint uPtr)
        {
            var w = arch.Object.Endianness.CreateImageWriter(mem, offset);
            w.WriteUInt32(uPtr);
        }
    }
}