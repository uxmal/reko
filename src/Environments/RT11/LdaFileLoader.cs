#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        public LdaFileLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var arch = new Pdp11Architecture();
            arch.Name = "pdp11";

            var rdr = new LeImageReader(RawImage);
            byte b;
            while (rdr.TryPeekByte(0, out b) && b == 0)
            {
                rdr.Offset += 1;
            }

            var segMap = ReadDataBlocks(rdr);

            var platform = new RT11Platform(Services, arch);
            var program = new Program
            {
                Architecture = arch,
                Platform = platform,
                SegmentMap = segMap
            };
           
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                new SortedList<Address, ImageSymbol>())
            {

            };
        }

        public SegmentMap ReadDataBlocks(LeImageReader rdr)
        {
            var segs = new List<ImageSegment>();
            var addrMin = Address.Ptr16(0);
            var addrMax = Address.Ptr16(0);
            for (;;)
            {
                var seg = ReadDataBlock(rdr);
                if (seg == null)
                    break;
                segs.Add(seg);
                addrMax = Address.Max(addrMax, seg.MemoryArea.EndAddress);
            }

            var image = new MemoryArea(addrMin, new byte[addrMax - addrMin]);
            foreach (var seg in segs)
            {
                var bytes = seg.MemoryArea.Bytes;
                Array.Copy(
                    bytes, 0,
                    image.Bytes, seg.Address.ToUInt16(),
                    bytes.Length);
            }
            return new SegmentMap(
                addrMin,
                new ImageSegment("image", image, AccessMode.ReadWriteExecute)); 
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
        /// | Data | - byte[] - Data (`Count` bytes)
        ///   ...  
        /// |------|
        /// | Chk  | - byte - Checksum
        /// +------+
        /// </remarks>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public ImageSegment ReadDataBlock(LeImageReader rdr)
        {
            ushort w;
            ushort count;
            ushort uAddr;
            byte b;
            if (!rdr.TryReadLeUInt16(out w) || w != 0x0001)
                return null;
            if (!rdr.TryReadLeUInt16(out count))
                return null;
            if (!rdr.TryReadLeUInt16(out uAddr))
                return null;
            var data = rdr.ReadBytes(count);
            if (data == null || data.Length < count)
                return null;
            if (!rdr.TryReadByte(out b))
                return null;

            Debug.Print("Data block: {0:X4} {1:X4}", uAddr, count);
            return new ImageSegment(
                string.Format("seg{0:X4}", uAddr),
                new MemoryArea(Address.Ptr16(uAddr), data),
                AccessMode.ReadWriteExecute);
        }
    }
}
