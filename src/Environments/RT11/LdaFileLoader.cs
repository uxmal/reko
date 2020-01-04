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

using System;
using Reko.Core;
using Reko.Arch.Pdp11;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Environments.RT11
{
    public class LdaFileLoader : ImageLoader
    {
        private Address addrEntry;

        public LdaFileLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var arch = new Pdp11Architecture("pdp11");

            var rdr = new LeImageReader(RawImage);

            var tuple = ReadDataBlocks(rdr);
            if (tuple == null)
                throw new BadImageFormatException("The file doesn't appear to be in LDA format.");

            var platform = new RT11Platform(Services, arch);
            var program = new Program
            {
                Architecture = arch,
                Platform = platform,
                SegmentMap = tuple.Item2,
            };
            this.addrEntry = Address.Ptr16(tuple.Item1);
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>
                {
                    ImageSymbol.Procedure(program.Architecture, addrEntry),
                },
                new SortedList<Address, ImageSymbol>());
        }

        public Tuple<ushort, SegmentMap> ReadDataBlocks(LeImageReader rdr)
        {
            var segs = new List<ImageSegment>();
            var addrMin = (ushort)0xFFFFu;
            var addrMax = (ushort)0x0000u;
            ushort addrStart = 0;
            var blocks = new List<Tuple<ushort, byte[]>>();
            for (;;)
            {
                var block = ReadDataBlock(rdr);
                if (block == null)
                    return null;
                if (block.Item2 == null)
                {
                    addrStart = block.Item1;
                    break;
                }
                blocks.Add(block);
                addrMin = Math.Min(addrMin, block.Item1);
                addrMax = Math.Max(addrMax, (ushort)(block.Item1 + block.Item2.Length));
            }

            var image = new MemoryArea(Address.Ptr16(addrMin), new byte[addrMax - addrMin]);
            foreach (var block in blocks)
            {
                Array.Copy(block.Item2, 0, image.Bytes, block.Item1 - addrMin, block.Item2.Length);
            }
            return Tuple.Create(
                addrStart,
                new SegmentMap(
                    image.BaseAddress,
                    new ImageSegment("image", image, AccessMode.ReadWriteExecute)));
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
        public Tuple<ushort, byte[]> ReadDataBlock(LeImageReader rdr)
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
                    return null;    // invalid file
                if (!rdr.TryReadByte(out b))
                    return null;
            } while (b != 0);

            if (!rdr.TryReadLeUInt16(out count))
                return null;
            if (!rdr.TryReadLeUInt16(out uAddr))
                return null;

            if (count == 6)
                return new Tuple<ushort, byte[]>(uAddr, null);
            var data = rdr.ReadBytes(count - 6);
            if (data == null || data.Length < count - 6)
                return null;
            if (!rdr.TryReadByte(out b))  // read (and ignore) checksum
                return null;

            Debug.Print("Data block: {0:X4} {1:X4}", uAddr, count);
            return Tuple.Create(uAddr, data);
        }
    }
}
