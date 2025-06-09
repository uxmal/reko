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

using Reko.Arch.Pdp;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Environments.RT11
{
    public class LdaFileLoader : ProgramImageLoader
    {
        public LdaFileLoader(IServiceProvider services, ImageLocation imageUri, byte[] imgRaw) : base(services, imageUri, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr16(0);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var arch = new Pdp11Architecture(Services, "pdp11", new Dictionary<string, object>());

            var rdr = new LeImageReader(RawImage);

            var (uAddrStart, segmentMap) = ReadDataBlocks(rdr);
            if (segmentMap is null)
                throw new BadImageFormatException("The file doesn't appear to be in LDA format.");

            var platform = new RT11Platform(Services, arch);
            var program = new Program
            {
                Architecture = arch,
                Platform = platform,
                Memory = new ByteProgramMemory(segmentMap),
                SegmentMap = segmentMap,
            };
            var addrEntry = Address.Ptr16(uAddrStart);
            program.EntryPoints[addrEntry] = ImageSymbol.Procedure(program.Architecture, addrEntry);
            return program;
        }

        public (ushort, SegmentMap?) ReadDataBlocks(LeImageReader rdr)
        {
            var segs = new List<ImageSegment>();
            var addrMin = (ushort)0xFFFFu;
            var addrMax = (ushort)0x0000u;
            ushort addrStart = 0;
            var blocks = new List<(ushort, byte[])>();
            for (;;)
            {
                var block = ReadDataBlock(rdr);
                if (block.Item2 is null)
                {
                    if (block.Item1 == 0)
                        return (0, null);
                    addrStart = block.Item1;
                    break;
                }
                blocks.Add(block!);
                addrMin = Math.Min(addrMin, block.Item1);
                addrMax = Math.Max(addrMax, (ushort)(block.Item1 + block.Item2.Length));
            }

            var bmem = new ByteMemoryArea(Address.Ptr16(addrMin), new byte[addrMax - addrMin]);
            foreach (var block in blocks)
            {
                Array.Copy(block.Item2, 0, bmem.Bytes, block.Item1 - addrMin, block.Item2.Length);
            }
            return (
                addrStart,
                new SegmentMap(
                    bmem.BaseAddress,
                    new ImageSegment("image", bmem, AccessMode.ReadWriteExecute)));
        }

        /// <summary>
        /// Reads LDA data blocks
        /// </summary>
        /// <remarks>
        /// The format of LDA blocks is:
        /// +------+
        /// | 0001 | - word16 - Magic
        /// |------|
        /// | BC   | - word16 - Count
        /// |------|
        /// | ADDR | - word16 - Absolute load address
        /// |------|
        /// | Data | - byte[] - Data (`Count` bytes, including the first 6 bytes)
        ///   ...  
        /// |------|
        /// | Chk  | - byte - Checksum
        /// +------+
        /// </remarks>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public (ushort, byte[]?) ReadDataBlock(LeImageReader rdr)
        {
            ushort count;
            ushort uAddr;
            byte b;

            // Eat bytes until 1 followed by 0.
            do
            {
                while (rdr.TryReadByte(out b) && b != 1)
                    ;
                if (b != 1)
                    return (0, null);    // invalid file
                if (!rdr.TryReadByte(out b))
                    return (0, null);
            } while (b != 0);

            if (!rdr.TryReadLeUInt16(out count))
                return (0, null);
            if (!rdr.TryReadLeUInt16(out uAddr))
                return (0, null);

            if (count == 6)
                return (uAddr, null);
            var data = rdr.ReadBytes(count - 6);
            if (data is null || data.Length < count - 6)
                return (0, null);
            if (!rdr.TryReadByte(out b))  // read (and ignore) checksum
                return (0, null);

            Debug.Print("Data block: {0:X4} {1:X4}", uAddr, count);
            return (uAddr, data);
        }
    }
}
