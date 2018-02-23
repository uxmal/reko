#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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

using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Reko.ImageLoaders.IntelHex32
{
    /// <summary>
    /// An Intel HEX32 image loader.
    /// </summary>
    public class IntelHEX32Loader : ImageLoader
    {

        //TODO: See how to adapt for Microchip PIC image loading (with memory mapping/checking) or other processors.

        //TODO: As Intel Hex specs do not specify any ordering of records, we should be able, getting a new record,
        // to add it at tail, at head, or merge with already loaded records.
        // For the time being we assume we are safe and Hex records are contiguous and sorted in increasing load address.

        #region Helper classes

        internal class MemChunk
        {
            public readonly Address BaseAddress;
            public List<byte> Datum;
            public Address EndAddress => BaseAddress + Datum.Count;

            public MemChunk(Address bAddr)
            {
                BaseAddress = bAddr;
                Datum = new List<byte>();
            }

            public MemChunk(uint address) : this(Address.Ptr32(address))
            {
            }

            public bool Contains(Address addr) => (BaseAddress <= addr && EndAddress > addr);

            public bool IsAtTail(Address addr) => addr == EndAddress;

            public bool IsAtHeadOf(Address addr) => EndAddress == addr;

        }

        /// <summary>
        /// List of memory chunks.
        /// </summary>
        internal class MemoryChunksList : IEnumerable<MemChunk>
        {
            private Address currAddr = null;
            private Address nextAddr = null;
            private SortedList<Address, MemChunk> memChunks;
            private MemChunk currMemChunk;

            public MemoryChunksList()
            {
                memChunks = new SortedList<Address, MemChunk>();
                currMemChunk = null;
            }

            private MemChunk _getPredChunk(Address addr)
            {
                foreach (var ch in memChunks.Values)
                {
                    if (ch.IsAtHeadOf(addr))
                        return ch;
                }
                return null;
            }

            /// <summary>
            /// Adds a series of data byte to the memory chunks list as decoded from an IHex32 data record.
            /// </summary>
            /// <remarks>
            /// The current implementation assumes that each IHex32 data record loads unique data block (no overlap).
            /// </remarks>
            /// <param name="address">The address from the decoded IHex32 record.</param>
            /// <param name="data">The binary bytes contained in the IHex32 record.</param>
            public void AddData(uint address, byte[] data)
            {
                if (data == null || data.Length < 1)
                    return;

                if (currAddr == null)
                {
                    currAddr = Address.Ptr32(address);
                    currMemChunk = new MemChunk(currAddr);
                    memChunks.Add(currAddr, currMemChunk);
                }
                else
                {
                    currAddr = Address.Ptr32(address);
                    if (nextAddr != currAddr)
                    {
                        currMemChunk = _getPredChunk(currAddr);
                        if (currMemChunk == null)
                        {
                            currMemChunk = new MemChunk(currAddr);
                            memChunks.Add(currAddr, currMemChunk);
                        }
                    }
                }

                currMemChunk.Datum.AddRange(data);
                nextAddr = currAddr + data.Length;
            }

            public IEnumerator<MemChunk> GetEnumerator() => memChunks.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion

        #region Locals

        private DecompilerEventListener listener;

        #endregion

        #region Constructors

        public IntelHEX32Loader(IServiceProvider services, string filename, byte[] imgRaw)
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
        /// </summary>
        /// <param name="addrLoad">Base address of program image. IGNORED.</param>
        /// <returns>
        /// A <see cref="Program"/> instance.
        /// </returns>
        public override Program Load(Address addrLoad)
        {
            listener = Services.RequireService<DecompilerEventListener>();
            MemoryChunksList memChunks = new MemoryChunksList();

            using (var rdr = new IntelHEX32Reader(new MemoryStream(RawImage)))
            {
                try
                {
                    for (; ; )
                    {
                        if (!rdr.Read(out uint address, out byte[] data))
                            break;
                        if (data != null)
                        {
                            memChunks.AddData(address, data);
                        }
                    }

                }
                catch (IntelHEX32Exception ex)
                {
                    listener.Error(new NullCodeLocation(""), ex.Message);
                }
            }

            var segs = new SegmentMap(PreferredBaseAddress);

            int i = 0;

            // Generate the image segments with fake names.
            foreach (var ch in memChunks)
            {
                var mem = new MemoryArea(ch.BaseAddress, ch.Datum.ToArray());
                var seg = new ImageSegment($"CODE_{i++:d2}", mem, AccessMode.ReadExecute);
                segs.AddSegment(seg);
            }

            return new Program(segs, null, null);
        }

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
            var prog = Load(addrLoad);
            return new Program() { SegmentMap = prog.SegmentMap, Architecture = arch, Platform = platform };
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
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }

        #endregion

    }

}
