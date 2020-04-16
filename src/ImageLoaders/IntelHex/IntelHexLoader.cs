#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work of:
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

using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.ImageLoaders.IntelHex
{
    /// <summary>
    /// An Intel Hexadecimal 32-bit object format image (a.k.a. IHEX32) loader.
    /// </summary>
    public class HexLoader : ImageLoader
    {
        //$TODO: As Intel Hex specs do not specify any ordering of records, we should be able, getting a new record,
        // to add it at tail, at head, or merge with already loaded records - or to create a new memory chunk.
        // For the time being we assume we are safe and Hex records are contiguous (not overlapping) and somehow sorted in increasing load address order.

        #region Helper classes

        /// <summary>
        /// A memory chunk. Consecutive bytes loaded at a given start memory address.
        /// </summary>
        internal class MemoryChunk
        {
            public readonly Address BaseAddress;
            public List<byte> Datum;
            public Address EndAddress => BaseAddress + Datum.Count;

            public MemoryChunk(Address bAddr)
            {
                BaseAddress = bAddr;
                Datum = new List<byte>();
            }

            public MemoryChunk(uint address) : this(Address.Ptr32(address))
            {
            }

            public bool Contains(Address addr) => (BaseAddress <= addr && EndAddress > addr);

            public bool IsAtTail(Address addr) => addr == EndAddress;

            public bool IsAtHeadOf(Address addr) => EndAddress == addr;

        }

        /// <summary>
        /// List of memory chunks.
        /// </summary>
        internal class MemoryChunksList : IEnumerable<MemoryChunk>
        {
            private Address currAddr = null;
            private Address nextAddr = null;
            private SortedList<Address, MemoryChunk> memChunks;
            private MemoryChunk currMemChunk;

            public MemoryChunksList()
            {
                memChunks = new SortedList<Address, MemoryChunk>();
                currMemChunk = null;
            }

            private MemoryChunk GetPredChunk(Address addr)
                => memChunks.Values.FirstOrDefault(mchk => mchk.IsAtHeadOf(addr));

            /// <summary>
            /// Adds a series of data byte to the memory chunks list as decoded from an IHex32 data record.
            /// </summary>
            /// <remarks>
            /// The current implementation assumes that each IHex32 data record loads unique data block (no overlap).
            /// </remarks>
            /// <param name="address">The address from the decoded IHex32 record.</param>
            /// <param name="data">The binary bytes contained in the IHex32 record.</param>
            public void AddData(Address address, byte[] data)
            {
                if (data == null || data.Length < 1)
                    return;

                if (currAddr == null)
                {
                    currAddr = address;
                    currMemChunk = new MemoryChunk(currAddr);
                    memChunks.Add(currAddr, currMemChunk);
                }
                else
                {
                    currAddr = address;
                    if (nextAddr != currAddr)
                    {
                        currMemChunk = GetPredChunk(currAddr);
                        if (currMemChunk == null)
                        {
                            currMemChunk = new MemoryChunk(currAddr);
                            memChunks.Add(currAddr, currMemChunk);
                        }
                    }
                }

                currMemChunk.Datum.AddRange(data);
                nextAddr = currAddr + data.Length;
            }

            public IEnumerator<MemoryChunk> GetEnumerator() => memChunks.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion

        #region Locals

        private DecompilerEventListener listener;

        #endregion

        #region Constructors

        public HexLoader(IServiceProvider services, string filename, byte[] imgRaw)
            : base(services, filename, imgRaw)
        {
        }

        #endregion

        #region ImageLoader interface implementation

        /// <summary>
        /// If nothing else is specified, this is the address at which the image will be loaded.
        /// </summary>
        /// <value>
        /// The preferred base address.
        /// </value>
        public override Address PreferredBaseAddress { get; set; } = Address32.NULL;

        /// <summary>
        /// Loads the image into memory starting at the specified address.
        /// Not used for Intel Hex as this format contains no meta-data.
        /// </summary>
        /// <param name="addrLoad">Base address of program image. IGNORED.</param>
        /// <returns>
        /// A <see cref="Program"/> instance.
        /// </returns>
        public override Program Load(Address addrLoad) => throw new NotImplementedException();

        /// <summary>
        /// Loads the image into memory at the specified address, using the provided
        /// <seealso cref="IProcessorArchitecture"/> and <seealso cref="IPlatform"/>.
        /// Used when loading raw files; not all image loaders can support this.
        /// </summary>
        /// <param name="addrLoad">Loading address *IGNORED*.</param>
        /// <param name="arch">Processor architecture.</param>
        /// <param name="platform">Platform/operating environment.</param>
        /// <returns>
        /// A <see cref="Program"/> instance.
        /// </returns>
        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            listener = Services.RequireService<DecompilerEventListener>();
            var memChunks = new MemoryChunksList();
            Address addrEp = null;
            Address addrBase = MakeZeroAddress(arch);
            using (var rdr = new IntelHexReader(new MemoryStream(RawImage), addrBase))
            {
                try
                {
                    for (; ; )
                    {
                        if (!rdr.TryReadRecord(out Address address, out byte[] data))
                            break;
                        if (data != null)
                        {
                            memChunks.AddData(address, data);
                            continue;
                        }
                    }
                    addrEp = rdr.StartAddress;

                }
                catch (IntelHexException ex)
                {
                    listener.Error(new NullCodeLocation(""), ex.Message);
                    return null;
                }
            }

            var segs = new SegmentMap(PreferredBaseAddress);

            // Generate the image segments with fake names.
            int i = 0;
            foreach (var mchk in memChunks)
            {
                var mem = new MemoryArea(mchk.BaseAddress, mchk.Datum.ToArray());
                var seg = new ImageSegment($"CODE_{i++:d2}", mem, AccessMode.ReadExecute);
                segs.AddSegment(seg);
            }

            var program = new Program(segs, arch, platform);
            if (addrEp != null)
            {
                program.EntryPoints.Add(addrEp, ImageSymbol.Procedure(arch, addrEp));
            }
            return program;

        }

        /// <summary>
        /// Generate a zero address for the given architecture.
        /// </summary>
        /// <param name="arch">Processor architecture to use</param>
        /// <returns>An address with linear offset 0.
        /// </returns>
        private Address MakeZeroAddress(IProcessorArchitecture arch)
        {
            if (!arch.TryParseAddress("0", out Address addr) &&
                !arch.TryParseAddress("0:0", out addr))
            {
                // Something's wrong with your architecture's TryParseAddress
                // method.
                throw new InvalidOperationException("Unable to create start address.");
            }
            return addr;
        }

        /// <summary>
        /// Performs fix-ups of the loaded image, adding findings to the supplied collections.
        /// Nothing actual can be done with Intel HEX binary files.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="addrLoad">The address at which the program image is loaded.</param>
        /// <returns>
        /// The <see cref="RelocationResults"/>.
        /// </returns>
        public override RelocationResults Relocate(Program program, Address addrLoad)
            => new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());

        #endregion

    }

}
