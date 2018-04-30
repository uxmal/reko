#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Libraries.Microchip;
using Reko.Core;
using Reko.Core.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// An abstract class which permits to construct the PIC memory map using the individual PIC definition.
    /// </summary>
    public abstract class MemoryMap : IMemoryMap
    {

        #region Inner classes

        /// <summary>
        /// This class defines the current PIC memory traits (characteristics).
        /// </summary>
        protected sealed class MemTraits : IMemTraitsSymbolVisitor
        {
            #region Class helpers

            /// <summary>
            /// Memory regions are keyed by domain/sub-domain.
            /// </summary>
            class MemoryDomainKey
            {

                public MemoryDomain Domain { get; }
                public MemorySubDomain SubDomain { get; }

                public MemoryDomainKey(MemoryDomain dom, MemorySubDomain subdom)
                {
                    Domain = dom;
                    SubDomain = subdom;
                }

            }

            /// <summary>
            /// Comparer of two memory regions' domain/sub-domain.
            /// </summary>
            class MemoryDomainKeyEqualityComparer : IEqualityComparer<MemoryDomainKey>
            {
                bool IEqualityComparer<MemoryDomainKey>.Equals(MemoryDomainKey x, MemoryDomainKey y)
                {
                    if (ReferenceEquals(x, y))
                        return true;
                    if (x is null || y is null)
                        return false;
                    if (x.Domain == y.Domain)
                        return x.SubDomain == y.SubDomain;
                    return false;
                }

                int IEqualityComparer<MemoryDomainKey>.GetHashCode(MemoryDomainKey key)
                {
                    return ((int)key.Domain << (int)key.SubDomain).GetHashCode();
                    ;
                }
            }

            #endregion

            #region Members fields

            private PIC pic;
            private readonly Dictionary<MemoryDomainKey, MemTrait> maptraits = new Dictionary<MemoryDomainKey, MemTrait>(new MemoryDomainKeyEqualityComparer());
            private static readonly MemTrait memtraitdefault = new DefaultMemTrait();

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="thePIC">the PIC definition.</param>
            public MemTraits(PIC thePIC)
            {
                pic = thePIC ?? throw new ArgumentNullException(nameof(thePIC));
                foreach (var mt in pic.ArchDef.MemTraits.Traits.OfType<IMemTraitsSymbolAcceptor>())
                    mt.Accept(this);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Gets the memory trait corresponding to the specified memory domain and sub-domain.
            /// </summary>
            /// <param name="dom">A memory domain value from the <see cref="MemoryDomain"/> enumeration.</param>
            /// <param name="subdom">A sub-domain value from the <see cref="MemorySubDomain"/> enumeration.</param>
            /// <param name="trait">[out] The memory trait.</param>
            /// <returns>
            /// True if it succeeds, false if it fails.
            /// </returns>
            public bool GetTrait(MemoryDomain dom, MemorySubDomain subdom, out MemTrait trait)
            {
                if (!maptraits.TryGetValue(new MemoryDomainKey(dom, subdom), out trait))
                    trait = memtraitdefault;
                return true;
            }

            /// <summary>
            /// Gets the memory trait corresponding to the specified memory sub-domain.
            /// </summary>
            /// <param name="subdom">A sub-domain value from the <see cref="MemorySubDomain"/> enumeration.</param>
            /// <param name="trait">[out] The memory trait.</param>
            /// <returns>
            /// True if it succeeds, false if it fails.
            /// </returns>
            public bool GetTrait(MemorySubDomain subdom, out MemTrait trait)
            {
                return GetTrait(PICArch.GetDomain(subdom), subdom, out trait);
            }

            #endregion

            #region IMemTraitsSymbolVisitor interface implementation

            void IMemTraitsSymbolVisitor.Visit(CalDataMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Calib), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(CodeMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Code), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(ConfigFuseMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.DeviceConfig), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(ExtCodeMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.ExtCode), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(EEDataMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.EEData), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(BackgroundDebugMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Debugger), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(ConfigWORMMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Other), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(DataMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.DPR), mTraits);
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.GPR), mTraits);
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.SFR), mTraits);
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.Emulator), mTraits);
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.Linear), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(DeviceIDMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.DeviceID), mTraits);
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.RevisionID), mTraits);
            }

            void IMemTraitsSymbolVisitor.Visit(TestMemTraits mTraits)
            {
                // Not interested in this.
            }

            void IMemTraitsSymbolVisitor.Visit(UserIDMemTraits mTraits)
            {
                maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.UserID), mTraits);
            }

            #endregion

        }

        /// <summary>
        /// A PIC memory region.
        /// </summary>
        protected abstract class MemoryRegionBase : IMemoryRegion
        {

            #region Members fields

            private readonly MemTraits traits;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="traits">The memory traits.</param>
            /// <param name="sRegion">The unique name of the memory region.</param>
            /// <param name="regnAddr">The region's (start,end) addresses.</param>
            /// <param name="memDomain">The memory domain type.</param>
            /// <param name="memSubDomain">The memory sub-domain type.</param>
            /// <exception cref="InvalidOperationException">Thrown if the map can't provide the region's
            ///                                             memory traits.</exception>
            public MemoryRegionBase(MemTraits traits, string sRegion, AddressRange regnAddr, MemoryDomain memDomain, MemorySubDomain memSubDomain)
            {
                this.traits = traits;
                RegionName = sRegion;
                if (regnAddr != null)
                {
                    LogicalByteAddrRange = regnAddr;
                    PhysicalByteAddrRange = regnAddr;
                }
                else
                {
                    LogicalByteAddrRange = PhysicalByteAddrRange = new AddressRange(Address.Ptr32(0), Address.Ptr32(0));
                }
                TypeOfMemory = memDomain;
                SubtypeOfMemory = memSubDomain;
                if (SubtypeOfMemory != MemorySubDomain.NNMR)  // Non-Memory-Mapped-Registers have no memory characteristics.
                {
                    if (!this.traits.GetTrait(memDomain, memSubDomain, out MemTrait trait))
                        throw new InvalidOperationException($"Missing characteristics for [{memDomain}/{memSubDomain}] memory region '{RegionName}'");
                    Trait = trait;
                }

            }

            #endregion

            #region IMemoryRegion interface implementation

            /// <summary>
            /// Gets the name of the memory region.
            /// </summary>
            /// <value>
            /// The ID of the memory region as a string.
            /// </value>
            public string RegionName { get; }

            /// <summary>
            /// Gets the virtual byte addresses range.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 virtual byte addresses of the memory region.
            /// </value>
            public AddressRange LogicalByteAddrRange { get; }

            /// <summary>
            /// Gets or sets the physical byte addresses range.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 physical byte addresses of the memory region.
            /// </value>
            public AddressRange PhysicalByteAddrRange { get; internal set; }

            /// <summary>
            /// Gets the type of the memory region.
            /// </summary>
            /// <value>
            /// A value from <see cref="MemoryDomain"/> enumeration.
            /// </value>
            public MemoryDomain TypeOfMemory { get; }

            /// <summary>
            /// Gets the subtype of the memory region.
            /// </summary>
            /// <value>
            /// A value from <see cref="MemorySubDomain"/> enumeration.
            /// </value>
            public MemorySubDomain SubtypeOfMemory { get; }

            /// <summary>
            /// Gets the memory region traits.
            /// </summary>
            /// <value>
            /// The characteristics of the memory region.
            /// </value>
            public MemTrait Trait { get; }

            /// <summary>
            /// Gets the memory region total size in bytes.
            /// </summary>
            /// <value>
            /// The size in number of bytes.
            /// </value>
            public uint Size => (PhysicalByteAddrRange is null ? 0 : (uint)(PhysicalByteAddrRange.End - PhysicalByteAddrRange.Begin));

            #endregion

            #region Methods

            /// <summary>
            /// Checks whether the given memory fragment is contained in this memory region.
            /// </summary>
            /// <param name="aFragAddr">The starting memory address of the fragment.</param>
            /// <param name="Len">(Optional) The length in bytes of the fragment (default=0).</param>
            /// <returns>
            /// True if the fragment is contained in this memory region, false if not.
            /// </returns>
            public bool Contains(Address aFragAddr, uint Len = 0)
            {
                if (aFragAddr is null)
                    return false;
                return ((aFragAddr >= LogicalByteAddrRange.Begin) && ((aFragAddr + Len) < LogicalByteAddrRange.End));
            }

            /// <summary>
            /// Checks whether the given memory fragment is contained in this memory region.
            /// </summary>
            /// <param name="cAddr">The starting memory address of the fragment.</param>
            /// <param name="Len">(Optional) The length in bytes of the fragment (default=0).</param>
            /// <returns>
            /// True if the fragment is contained in this memory region, false if not.
            /// </returns>
            public bool Contains(Constant cAddr, uint Len = 0)
                => Contains(cAddr.ToUInt32(), Len);

            /// <summary>
            /// Checks whether the given memory fragment is contained in this memory region.
            /// </summary>
            /// <param name="uAddr">The starting memory address of the fragment.</param>
            /// <param name="Len">(Optional) The length in bytes of the fragment (default=0).</param>
            /// <returns>
            /// True if the fragment is contained in this memory region, false if not.
            /// </returns>
            public bool Contains(uint uAddr, uint Len = 0)
                => Contains(Address.Ptr32(uAddr), Len);

            public override string ToString()
                => $"'{RegionName}': {SubtypeOfMemory}[0x{LogicalByteAddrRange.Begin:X}-0x{LogicalByteAddrRange.End:X}]";

            #endregion

        }

        /// <summary>
        /// A Program PIC memory region.
        /// </summary>
        protected class ProgMemRegion : MemoryRegionBase
        {
            /// <summary>
            /// Instantiates a new program memory region.
            /// </summary>
            /// <param name="traits">The memory traits (characteristics).</param>
            /// <param name="sRegion">The region's name.</param>
            /// <param name="regnAddr">The region memory address range.</param>
            /// <param name="memSubDomain">The memory sub-domain code.</param>
            public ProgMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, MemorySubDomain memSubDomain)
                : base(traits, sRegion, regnAddr, MemoryDomain.Prog, memSubDomain)
            {
            }

        }

        /// <summary>
        /// A Data PIC memory region.
        /// </summary>
        protected class DataMemRegion : MemoryRegionBase
        {
            /// <summary>
            /// Instantiates a new data memory region.
            /// </summary>
            /// <param name="traits">The memory traits (characteristics).</param>
            /// <param name="sRegion">The region's name.</param>
            /// <param name="regnAddr">The region memory address range.</param>
            /// <param name="memSubDomain">The memory sub-domain code.</param>
            public DataMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, MemorySubDomain memSubDomain)
                : base(traits, sRegion, regnAddr, MemoryDomain.Data, memSubDomain)
            {
            }

        }

        /// <summary>
        /// A PIC Linear Memory accessed region.
        /// </summary>
        protected class LinearRegion : ILinearRegion
        {

            #region Member fields

            private readonly MemTraits traits;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="traits">The memory regions traits.</param>
            /// <param name="bankSz">Size of the memory bank in number of bytes.</param>
            /// <param name="regnAddr">The region's (start,end) addresses.</param>
            /// <param name="blockRng">The block memory range addresses.</param>
            /// <exception cref="InvalidOperationException">Thrown if the map can't provide the region's
            ///                                             memory traits.</exception>
            public LinearRegion(MemTraits traits, int bankSz, AddressRange regnAddr, AddressRange blockRng)
            {
                this.traits = traits;
                BankSize = bankSz;
                FSRByteAddress = regnAddr;
                BlockByteRange = blockRng;
                if (!this.traits.GetTrait(TypeOfMemory, SubtypeOfMemory, out MemTrait trait))
                    throw new InvalidOperationException($"Missing characteristics for [{TypeOfMemory}/{SubtypeOfMemory}] linear region");
                Trait = trait;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Query if this linear memory region contains the given  memory subrange.
            /// </summary>
            /// <param name="aVirtByteAddr">Zero-based index of the virtual byte address.</param>
            /// <param name="Len">(Optional) The length in bytes of the memory subrange.</param>
            public bool Contains(Address aVirtByteAddr, int Len = 0)
                => ((aVirtByteAddr >= FSRByteAddress.Begin) && ((aVirtByteAddr + Len) < FSRByteAddress.End));

            #endregion

            #region ILinearRegion interface implementation

            /// <summary>
            /// Gets the byte size of GPR memory banks.
            /// </summary>
            /// <value>
            /// The size of the memory banks in number of bytes.
            /// </value>
            public int BankSize { get; }

            /// <summary>
            /// Gets the FSR byte address indirect range of the Linear Access data memory region.
            /// </summary>
            public AddressRange FSRByteAddress { get; }

            /// <summary>
            /// Gets the block byte range visible in the Linear Access data region.
            /// </summary>
            public AddressRange BlockByteRange { get; }

            /// <summary>
            /// Gets the type of the Linear Access memory region.
            /// </summary>
            public MemoryDomain TypeOfMemory => MemoryDomain.Data;

            /// <summary>
            /// Gets the subtype of the Linear Access memory region.
            /// </summary>
            public MemorySubDomain SubtypeOfMemory => MemorySubDomain.Linear;

            /// <summary>
            /// Gets the memory characteristics of the Linear Access data memory region.
            /// </summary>
            public MemTrait Trait { get; }

            /// <summary>
            /// Gets the size, in bytes, of the Linear Access data memory region.
            /// </summary>
            /// <value>
            /// The size in number of bytes.
            /// </value>
            public int Size => (int)(FSRByteAddress.End - FSRByteAddress.Begin);

            /// <summary>
            /// Remap a FSR indirect address from the Linear Access data region address to the corresponding GPR
            /// full memory address.
            /// </summary>
            /// <param name="aFSRAddr">The Linear Access data memory byte address.</param>
            /// <returns>
            /// The data memory address or null.
            /// </returns>
            public Address RemapAddress(Address aFSRAddr)
            {
                if (!Contains(aFSRAddr))
                    return null;
                if (!RemapFSRIndirect(aFSRAddr, out (byte BankNum, uint BankOffset) add))
                    return null;
                return Address.Ptr16((ushort)(add.BankNum * BankSize + add.BankOffset));
            }

            /// <summary>
            /// Remap a FSR indirect address in linear data region address to the corresponding GPR bank number and
            /// offset.
            /// </summary>
            /// <param name="aFSRVirtAddr">The virtual data memory byte address.</param>
            /// <returns>
            /// A tuple containing the GPR Bank Number and GPR Offset or NOPHYSICAL_MEM(-1, -1) indicator.
            /// </returns>
            public bool RemapFSRIndirect(Address aFSRVirtAddr, out (byte BankNum, uint BankOffset) gprBank)
            {
                gprBank = (0, 0);
                if (!Contains(aFSRVirtAddr))
                    return false;
                uint blocksize = (uint)(BlockByteRange.End - BlockByteRange.Begin);
                var fsraddr = aFSRVirtAddr.ToLinear() - FSRByteAddress.Begin.ToLinear();
                byte bankNo = (byte)(fsraddr / blocksize);
                uint bankOff = (uint)(fsraddr % blocksize);
                gprBank = (bankNo, (BlockByteRange.Begin + (int)bankOff).ToUInt32());
                return true;
            }

            #endregion

        }

        protected abstract class MemoryMapBase<T> where T : MemoryRegionBase
        {
            protected readonly PIC pic;
            protected readonly MemoryMap map;
            protected readonly MemTraits traits;
            protected List<T> memRegions;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pic">The PIC definition.</param>
            /// <param name="map">The memory map.</param>
            /// <param name="traits">The PIC memory traits.</param>
            protected MemoryMapBase(PIC pic, MemoryMap map, MemTraits traits)
            {
                this.pic = pic;
                this.map = map;
                this.traits = traits;
                memRegions = new List<T>();
            }


            /// <summary>
            /// Gets the memory regions contained in this memory map.
            /// </summary>
            /// <value>
            /// The list of memory regions.
            /// </value>
            public IReadOnlyList<IMemoryRegion> RegionsList => memRegions;

            /// <summary>
            /// Enumerates the memory regions contained in this memory map.
            /// </summary>
            public IEnumerable<IMemoryRegion> Regions => memRegions.Select(p => p);


            /// <summary>
            /// Creates memory range given begin/end addresses.
            /// </summary>
            /// <param name="begAddr">The begin address as an unsigned integer.</param>
            /// <param name="endAddr">The end address as an unsigned integer.</param>
            /// <returns>
            /// The new memory range.
            /// </returns>
            protected abstract AddressRange CreateMemRange(uint begAddr, uint endAddr);

            /// <summary>
            /// Gets a data memory region given its name ID.
            /// </summary>
            /// <param name="sRegionName">Name ID of the memory region.</param>
            /// <returns>
            /// The data memory region.
            /// </returns>
            public IMemoryRegion GetRegion(string sRegionName)
                => memRegions.Find((r) => r.RegionName == sRegionName);

            /// <summary>
            /// Gets a data memory region given a memory virtual address.
            /// </summary>
            /// <param name="virtAddr">The memory address.</param>
            /// <returns>
            /// The data memory region.
            /// </returns>
            public IMemoryRegion GetRegion(Address virtAddr)
                => memRegions.Find((regn) => regn.Contains(virtAddr));

            public virtual void AddRegion(T regn)
            {
                memRegions.Add(regn);
                map.AddSubDomain(regn.SubtypeOfMemory);
            }


        }

        /// <summary>
        /// This class defines the program memory map of current PIC.
        /// </summary>
        protected sealed class ProgMemoryMap : MemoryMapBase<ProgMemRegion>, IMemProgramRegionVisitor
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="thePIC">The PIC definition.</param>
            /// <param name="map">The memory map.</param>
            /// <param name="traits">The PIC memory traits.</param>
            public ProgMemoryMap(PIC thePIC, MemoryMap map, MemTraits traits) : base(thePIC, map, traits)
            {
                foreach (var pmr in thePIC.ProgramSpace.Sectors?.OfType<IMemProgramRegionAcceptor>())
                {
                    pmr.Accept(this);
                }
            }


            #region IMemProgramRegionVisitor interface implementation

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
            {
                if (!pic.IsPIC18)
                {
                    begAddr <<= 1;
                    endAddr <<= 1;
                }
                return new AddressRange(Address.Ptr32(begAddr), Address.Ptr32(endAddr));
            }

            void IMemProgramRegionVisitor.Visit(BACKBUGVectorSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.Debugger));
            }

            void IMemProgramRegionVisitor.Visit(CalDataZone xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.Calib));
            }

            void IMemProgramRegionVisitor.Visit(CodeSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.Code));
            }

            void IMemProgramRegionVisitor.Visit(ConfigFuseSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.DeviceConfig));
            }

            void IMemProgramRegionVisitor.Visit(DeviceIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.DeviceID));
            }

            void IMemProgramRegionVisitor.Visit(EEDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.EEData));
            }

            void IMemProgramRegionVisitor.Visit(ExtCodeSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.ExtCode));
            }

            void IMemProgramRegionVisitor.Visit(RevisionIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.RevisionID));
            }

            void IMemProgramRegionVisitor.Visit(TestZone xmlRegion)
            {
                // No interest.
            }

            void IMemProgramRegionVisitor.Visit(UserIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.UserID));
            }

            void IMemProgramRegionVisitor.Visit(DIASector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.DeviceInfoAry));
            }

            void IMemProgramRegionVisitor.Visit(DCISector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                AddRegion(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.DeviceConfigInfo));
            }

            #endregion

        }

        /// <summary>
        /// This class defines the data memory map of current PIC.
        /// </summary>
        protected sealed class DataMemoryMap : MemoryMapBase<DataMemRegion>, IMemDataRegionVisitor, IMemDataSymbolVisitor
        {

            #region Member fields

            public Address[] remapTable;
            internal IMemoryRegion Emulatorzone;
            internal ILinearRegion Linearsector;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool isNMMR = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ushort currLoadAddr;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ushort currRelAddr;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="thePIC">The PIC definition.</param>
            /// <param name="traits">The PIC memory traits.</param>
            /// <param name="mode">The PIC execution  mode.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC data memory size is invalid.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC execution mode is invalid.</exception>
            public DataMemoryMap(PIC thePIC, MemoryMap map, MemTraits traits, PICExecMode mode) : base(thePIC, map, traits)
            {
                uint datasize = thePIC.DataSpace?.EndAddr ?? 0;
                if (datasize < MinDataMemorySize)
                    throw new ArgumentOutOfRangeException("Too low data memory size. Check PIC definition.");
                remapTable = new Address[datasize];
                for (int i = 0; i < remapTable.Length; i++)
                    remapTable[i] = null;
                foreach (var dmr in thePIC.DataSpace.RegardlessOfMode.Regions?.OfType<IMemDataRegionAcceptor>())
                    dmr.Accept(this);
                switch (mode)
                {
                    case PICExecMode.Traditional:
                        foreach (var dmr in thePIC.DataSpace.TraditionalModeOnly?.OfType<IMemDataRegionAcceptor>())
                            dmr.Accept(this);
                        break;
                    case PICExecMode.Extended:
                        if (!thePIC.IsExtended)
                            throw new InvalidOperationException("Extended execution mode is not supported by this PIC");
                        foreach (var dmr in thePIC.DataSpace.ExtendedModeOnly?.OfType<IMemDataRegionAcceptor>())
                            dmr.Accept(this);
                        break;
                }
                foreach (var dmr in thePIC.IndirectSpace?.OfType<IMemDataRegionAcceptor>())
                    dmr.Accept(this);

            }

            #endregion

            #region IMemDataRegionVisitor interface implementation

            #region Helpers

            private void ResetAddrs(ushort newAddr)
            {
                currLoadAddr = newAddr;
                currRelAddr = 0;
            }

            private void UpdateAddrs(int incr)
            {
                if (incr > 0)
                {
                    currLoadAddr += (ushort)incr;
                    currRelAddr += (ushort)incr;

                }
                else if (incr < 0)
                {
                    currLoadAddr -= (ushort)(-incr);
                    currRelAddr -= (ushort)(-incr);
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(incr));
            }

            #endregion

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
                => new AddressRange(Address.Ptr16((ushort)begAddr), Address.Ptr16((ushort)endAddr));

            void IMemDataRegionVisitor.Visit(SFRDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.SFR);
                AddRegion(regn);
                ResetAddrs(regn.LogicalByteAddrRange.Begin.ToUInt16());
                isNMMR = false;
                foreach (var sds in xmlRegion.SFRs.OfType<IMemDataSymbolAcceptor>())
                    sds.Accept(this);
            }

            void IMemDataRegionVisitor.Visit(GPRDataSector xmlRegion)
            {
                AddressRange memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr);
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.GPR);
                var idRef = xmlRegion.ShadowIDRef;
                if (!string.IsNullOrWhiteSpace(idRef))
                {
                    var remap = GetRegion(idRef) ?? throw new ArgumentOutOfRangeException(nameof(idRef));
                    regn.PhysicalByteAddrRange = remap.PhysicalByteAddrRange;
                }
                AddRegion(regn);
                for (int i = 0; i < regn.Size; i++)
                {
                    remapTable[regn.LogicalByteAddrRange.Begin.ToUInt16() + i] = regn.PhysicalByteAddrRange.Begin + i;
                }
            }

            void IMemDataRegionVisitor.Visit(DPRDataSector xmlRegion)
            {
                AddressRange memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.DPR);
                var idRef = xmlRegion.ShadowIDRef;
                if (!string.IsNullOrWhiteSpace(idRef))
                {
                    var remap = GetRegion(idRef) ?? throw new ArgumentOutOfRangeException(nameof(idRef));
                    regn.PhysicalByteAddrRange = remap.PhysicalByteAddrRange;
                }
                AddRegion(regn);
                for (int i = 0; i < regn.Size; i++)
                {
                    remapTable[regn.LogicalByteAddrRange.Begin.ToUInt16() + i] = regn.PhysicalByteAddrRange.Begin + i;
                }
            }

            void IMemDataRegionVisitor.Visit(EmulatorZone xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                Emulatorzone = new DataMemRegion(traits, xmlRegion.RegionID, memrng, MemorySubDomain.Emulator);
                map.AddSubDomain(MemorySubDomain.Emulator);
            }

            void IMemDataRegionVisitor.Visit(NMMRPlace xmlRegion)
            {
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, null, MemorySubDomain.NNMR);
                AddRegion(regn);
                isNMMR = true;
                foreach (var nmmr in xmlRegion.SFRDefs.OfType<IMemDataSymbolAcceptor>()) nmmr.Accept(this);
                isNMMR = false;
            }

            void IMemDataRegionVisitor.Visit(LinearDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var blkrng = CreateMemRange(xmlRegion.BlockBeginAddr, xmlRegion.BlockEndAddr); ;
                Linearsector = new LinearRegion(traits, xmlRegion.BankSize, memrng, blkrng);
                map.AddSubDomain(MemorySubDomain.Linear);
            }

            #endregion

            #region IMemDataSymbolVisitor<bool> implementation

            void IMemDataSymbolVisitor.Visit(DataBitAdjustPoint xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(DataByteAdjustPoint xmlSymb)
            {
                UpdateAddrs(xmlSymb.Offset);
            }

            void IMemDataSymbolVisitor.Visit(SFRDef xmlSymb)
            {
                if (isNMMR)
                    return;
                remapTable[currLoadAddr] = Address.Ptr16(currLoadAddr);
                UpdateAddrs((int)xmlSymb.ByteWidth);
            }

            void IMemDataSymbolVisitor.Visit(SFRFieldDef xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(SFRFieldSemantic xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(SFRModeList xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(SFRMode xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(Mirror xmlSymb)
            {
                var regn = GetRegion(xmlSymb.RegionIDRef);
                if (regn != null)
                {
                    for (int i = 0; i < xmlSymb.NzSize; i++)
                        remapTable[currLoadAddr + i] = regn.PhysicalByteAddrRange.Begin + i;
                }
                UpdateAddrs(xmlSymb.NzSize);
            }

            void IMemDataSymbolVisitor.Visit(JoinedSFRDef xmlSymb)
            {
                foreach (var sfr in xmlSymb.SFRs?.OfType<IMemDataSymbolAcceptor>())
                    sfr.Accept(this);
            }

            void IMemDataSymbolVisitor.Visit(MuxedSFRDef xmlSymb)
            {
                for (int i = 0; i < ((xmlSymb.NzWidth + 7) >> 3); i++)
                {
                    remapTable[currLoadAddr + i] = Address.Ptr16((ushort)(currLoadAddr + i));
                }
                UpdateAddrs(xmlSymb.ByteWidth);
            }

            void IMemDataSymbolVisitor.Visit(SelectSFR xmlSymb)
            {
                // Do nothing. Address increment is already handled by parent MuxedSFRDef.
            }

            void IMemDataSymbolVisitor.Visit(DMARegisterMirror xmlSymb)
            {
                // Do nothing for now.
            }

            #endregion

        }

        #endregion

        /// <summary>
        /// Minimum size in bytes of the PIC data memory space.
        /// </summary>
        public const int MinDataMemorySize = 12;

        protected readonly MemTraits traits;
        protected readonly ProgMemoryMap progMap;
        protected DataMemoryMap dataMap;
        private HashSet<MemorySubDomain> subdomains = new HashSet<MemorySubDomain>();


        protected MemoryMap()
        {
        }

        protected MemoryMap(PIC thePIC)
        {
            PIC = thePIC;
            traits = new MemTraits(thePIC);
            progMap = new ProgMemoryMap(thePIC, this, traits);
            dataMap = new DataMemoryMap(thePIC, this, traits, PICExecMode.Traditional);
        }

        #region Methods

        protected void AddSubDomain(MemorySubDomain subdom) => subdomains.Add(subdom);

        public bool HasSubDomain(MemorySubDomain subdom) => subdomains.Contains(subdom);

        protected static bool IsValidMap(MemoryMap map)
        {
            if (map.traits == null)
                return false;
            if (map.progMap == null)
                return false;
            if (!map.HasSubDomain(MemorySubDomain.DeviceConfig))
                return false;
            if (!map.HasSubDomain(MemorySubDomain.Code) && !map.HasSubDomain(MemorySubDomain.ExtCode))
                return false;
            if (map.dataMap == null)
                return false;
            if (!map.HasSubDomain(MemorySubDomain.GPR))
                return false;
            if (!map.HasSubDomain(MemorySubDomain.SFR))
                return false;
            return true;
        }

        public virtual bool IsValid => IsValidMap(this);

        #endregion

        #region IMemoryMap interface implementation

        /// <summary>
        /// Gets the target PIC for this memory map.
        /// </summary>
        /// <value>
        /// The target PIC.
        /// </value>
        public PIC PIC { get; }

        /// <summary>
        /// Gets the instruction set identifier of the target PIC.
        /// </summary>
        /// <value>
        /// A value from the <see cref="InstructionSetID"/> enumeration.
        /// </value>
        public InstructionSetID InstructionSetID => PIC.GetInstructionSetID;

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public abstract PICExecMode ExecMode { get; set; }

        #region Data memory related

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sRegionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion GetDataRegion(string sRegionName) => dataMap.GetRegion(sRegionName);

        /// <summary>
        /// Gets a data memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion GetDataRegion(Address aVirtAddr) => dataMap.GetRegion(aVirtAddr);

        /// <summary>
        /// Gets a list of data regions.
        /// </summary>
        public IReadOnlyList<IMemoryRegion> DataRegionsList => dataMap.RegionsList;

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        public IEnumerable<IMemoryRegion> DataRegions => dataMap.Regions;

        /// <summary>
        /// Gets the data memory Emulator zone. Valid only if <seealso cref="HasEmulatorZone"/> is true.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        public IMemoryRegion EmulatorZone => dataMap.Emulatorzone;

        /// <summary>
        /// Gets the Linear Data Memory definition. Valid only if <seealso cref="HasLinear"/> is true.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        public ILinearRegion LinearSector => dataMap.Linearsector;

        /// <summary>
        /// Remap a data memory address.
        /// </summary>
        /// <param name="lAddr">The logical memory address.</param>
        /// <returns>
        /// The physical memory address.
        /// </returns>
        public abstract PICDataAddress RemapDataAddress(PICDataAddress lAddr);

        #endregion

        #region Program memory related

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sRegionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region or null.
        /// </returns>
        public IMemoryRegion GetProgramRegion(string sRegionName)
            => progMap.GetRegion(sRegionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public IMemoryRegion GetProgramRegion(Address aVirtAddr)
            => progMap.GetRegion(aVirtAddr);

        /// <summary>
        /// Gets a list of program regions.
        /// </summary>
        public IReadOnlyList<IMemoryRegion> ProgramRegionsList => progMap.RegionsList;

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        public IEnumerable<IMemoryRegion> ProgramRegions => progMap.Regions;

        public virtual PICProgAddress RemapProgramAddress(PICProgAddress lAddr)
        {
            return lAddr;
        }

        #endregion

        /// <summary>
        /// Provides a memory sub-domain's location and word sizes.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="MemorySubDomain"/>
        ///                      enumeration.</param>
        /// <returns>
        /// A Tuple containing the location size and wordsize. Returns (0,0) if the subdomain does not
        /// exist.
        /// </returns>
        public (uint LocSize, uint WordSize) SubDomainSizes(MemorySubDomain subdom)
        {
            if (traits.GetTrait(subdom, out MemTrait t))
                return (t.LocSize, t.WordSize);
            return (0, 0);
        }

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public abstract bool IsAccessRAMLow(PICDataAddress cAddr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public abstract bool IsAccessRAMHigh(PICDataAddress uAddr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> can be a FSR2 index
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        public abstract bool CanBeFSR2IndexAddress(ushort uAddr);

        #endregion

    }

}
