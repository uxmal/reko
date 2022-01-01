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

using Reko.Core;
using Reko.Core.Memory;
using System;
using System.IO;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Update pointers within A5World that point to an address within A5World, to the real/absolute address. 
    /// </summary> 
    public class A5Relocator
    {
        private readonly MacOSClassic platform;
        private readonly BeImageReader a5dr;
        private readonly uint a5dbelow;

        public A5Relocator(MacOSClassic platform, BeImageReader a5dr, UInt32 a5dbelow)
        {
            this.platform = platform; 
            this.a5dr = a5dr;
            this.a5dbelow = a5dbelow;
        }

        public void Relocate()
        {
            var memA5 = (ByteMemoryArea) platform.A5World.MemoryArea;
            var a5belowWriter = new BeImageWriter(memA5, 0);
            var a5belowReader = new BeImageReader(memA5, 0);
            uint a5globalOffset = platform.A5Offset - a5dbelow;

            var a5WorldAddress = (UInt32) ((platform.A5World.Address.Offset + platform.A5Offset) & 0xFFFFFFFF);

            // set Repeat count = 1, reset after each completed copy cycle
            // byte token lower 4 bits number of words to copy from compressed data
            // byte token upper 4 bits number of words to skip in global application data space
            // if either value is 0 then get run length value which is in bytes.
            // if the new run length value for copy is 0 then it's the end of compression data.

            for (;;)
            {
                int a5repeat = 1;
                // Skip is number of 16-bit words to skip  
                uint a5globalSkip = a5dr.ReadByte();
                if (a5globalSkip == 0)
                {
                    a5globalSkip = a5dr.ReadByte();
                    if (a5globalSkip == 0)
                    {
                        break;
                    }
                    if (a5globalSkip > 0x7F)
                    {
                        a5globalSkip = ((a5globalSkip & 0x7F) << 8) + a5dr.ReadByte();
                        a5globalSkip = (a5globalSkip << 16) + a5dr.ReadBeUInt16();
                    }
                    else
                    {
                        a5repeat = ResourceFork.GetRunLengthValue(a5dr, ref a5repeat);
                        //$BUG: a5repeat could return the value 0. The do-while below will 
                        // decrement a5repeat before testing. This will lead to an effective
                        // repeat count of 2^32; likely not wanted.
                    }
                }
                else
                {
                    if ((a5globalSkip & 0x80) == 0x80)
                    {
                        a5globalSkip = ((a5globalSkip & 0x7F) << 8) + a5dr.ReadByte();
                    }
                }
                a5globalSkip = a5globalSkip * 2;
                do
                {
                    a5globalOffset += a5globalSkip;
                    a5belowReader.Seek(a5globalOffset, SeekOrigin.Begin);
                    uint a5ptrOffset = a5belowReader.ReadBeUInt32();
                    a5belowWriter.Position = (int) a5globalOffset;

                    // write relocated A5World pointers to absolute address in A5World segment
                    // Possible register/mark as Global pointer references to strings

                    a5belowWriter.WriteBeUInt32((a5WorldAddress + a5ptrOffset) & 0xFFFFFFFF);
                    --a5repeat;
                } while (a5repeat > 0);
            }
        }
    }
}
