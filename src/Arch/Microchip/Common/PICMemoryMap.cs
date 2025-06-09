#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// An abstract class which permits to construct the PIC memory map modelization based on the individual PIC definition/descriptor.
    /// </summary>
    public abstract class PICMemoryMap : IPICMemoryMap
    {

        #region Inner classes

        /// <summary>
        /// This class defines the current PIC memory traits (characteristics).
        /// </summary>
        protected sealed class MemTraits
        {
            #region Class helpers

            /// <summary>
            /// Memory regions are keyed by domain/sub-domain.
            /// </summary>
            class MemoryDomainKey
            {

                public PICMemoryDomain Domain { get; }
                public PICMemorySubDomain SubDomain { get; }

                public MemoryDomainKey(PICMemoryDomain dom, PICMemorySubDomain subdom)
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
                public bool Equals(MemoryDomainKey? x, MemoryDomainKey? y)
                {
                    if (ReferenceEquals(x, y))
                        return true;
                    if (x is null || y is null)
                        return false;
                    if (x.Domain == y.Domain)
                        return x.SubDomain == y.SubDomain;
                    return false;
                }

                public int GetHashCode(MemoryDomainKey key)
                {
                    return ((int)key.Domain << (int)key.SubDomain).GetHashCode();
                }

            }

            #endregion

            private static readonly Dictionary<MemoryDomainKey, IPICMemTrait> maptraits = new Dictionary<MemoryDomainKey, IPICMemTrait>(new MemoryDomainKeyEqualityComparer());
            private static readonly IPICMemTrait memtraitdefault = new DefaultMemTrait();

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pic">The PIC architecture main definitions.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
            public MemTraits(IPICDescriptor pic)
            {
                if (pic is null)
                    throw new ArgumentNullException(nameof(pic));
                maptraits.Clear();
                foreach (var trait in pic.PICMemoryTraits)
                {
                    if (trait.Domain != PICMemoryDomain.Data)
                    {
                        var key = new MemoryDomainKey(trait.Domain, trait.SubDomain);
                        if (!maptraits.ContainsKey(key))
                        {
                            maptraits.Add(key, trait);
                        }
                    }
                    else
                    {
                        maptraits.Add(new MemoryDomainKey(PICMemoryDomain.Data, PICMemorySubDomain.DPR), trait);
                        maptraits.Add(new MemoryDomainKey(PICMemoryDomain.Data, PICMemorySubDomain.GPR), trait);
                        maptraits.Add(new MemoryDomainKey(PICMemoryDomain.Data, PICMemorySubDomain.SFR), trait);
                        maptraits.Add(new MemoryDomainKey(PICMemoryDomain.Data, PICMemorySubDomain.Emulator), trait);
                        maptraits.Add(new MemoryDomainKey(PICMemoryDomain.Data, PICMemorySubDomain.Linear), trait);
                    }
                }
            }


            /// <summary>
            /// Gets the memory trait corresponding to the specified memory domain and sub-domain.
            /// </summary>
            /// <param name="dom">A memory domain value from the <see cref="PICMemoryDomain"/> enumeration.</param>
            /// <param name="subdom">A sub-domain value from the <see cref="PICMemorySubDomain"/> enumeration.</param>
            /// <param name="trait">[out] The memory trait.</param>
            /// <returns>
            /// True if it succeeds, false if it fails.
            /// </returns>
            public bool GetTrait(PICMemoryDomain dom, PICMemorySubDomain subdom, [MaybeNullWhen(false)] out IPICMemTrait trait)
            {
                if (!maptraits.TryGetValue(new MemoryDomainKey(dom, subdom), out trait))
                    trait = memtraitdefault;
                return true;
            }

            /// <summary>
            /// Gets the memory trait corresponding to the specified memory sub-domain.
            /// </summary>
            /// <param name="subdom">A sub-domain value from the <see cref="PICMemorySubDomain"/> enumeration.</param>
            /// <param name="trait">[out] The memory trait.</param>
            /// <returns>
            /// True if it succeeds, false if it fails.
            /// </returns>
            public bool GetTrait(PICMemorySubDomain subdom, [MaybeNullWhen(false)] out IPICMemTrait trait)
                => GetTrait(subdom.GetDomain(), subdom, out trait);

        }

        /// <summary>
        /// A PIC memory region.
        /// </summary>
        protected abstract class MemoryRegionBase : IMemoryRegion
        {

            private readonly MemTraits traits;


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
            public MemoryRegionBase(MemTraits traits, string sRegion, AddressRange regnAddr, PICMemoryDomain memDomain, PICMemorySubDomain memSubDomain)
            {
                this.Trait = null!;
                this.traits = traits;
                RegionName = sRegion;
                if (regnAddr is not null)
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
                if (SubtypeOfMemory != PICMemorySubDomain.NNMR)  // Non-Memory-Mapped-Registers have no memory characteristics.
                {
                    if (!this.traits.GetTrait(memDomain, memSubDomain, out IPICMemTrait? trait))
                        throw new InvalidOperationException($"Missing characteristics for [{memDomain}/{memSubDomain}] memory region '{RegionName}'");
                    Trait = trait;
                }

            }


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
            /// A value from <see cref="PICMemoryDomain"/> enumeration.
            /// </value>
            public PICMemoryDomain TypeOfMemory { get; }

            /// <summary>
            /// Gets the subtype of the memory region.
            /// </summary>
            /// <value>
            /// A value from <see cref="PICMemorySubDomain"/> enumeration.
            /// </value>
            public PICMemorySubDomain SubtypeOfMemory { get; }

            /// <summary>
            /// Gets the memory region traits.
            /// </summary>
            /// <value>
            /// The characteristics of the memory region.
            /// </value>
            public IPICMemTrait Trait { get; }

            /// <summary>
            /// Gets the memory region total size in bytes.
            /// </summary>
            /// <value>
            /// The size in number of bytes.
            /// </value>
            public uint Size => (PhysicalByteAddrRange is null ? 0 : (uint)(PhysicalByteAddrRange.End - PhysicalByteAddrRange.Begin));

            #endregion

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
            public ProgMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, PICMemorySubDomain memSubDomain)
                : base(traits, sRegion, regnAddr, PICMemoryDomain.Prog, memSubDomain)
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
            /// <param name="bankSel">The memory bank selector.</param>
            public DataMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, PICMemorySubDomain memSubDomain, Constant bankSel)
                : base(traits, sRegion, regnAddr, PICMemoryDomain.Data, memSubDomain)
            {
                BankSelector = bankSel;
            }

            /// <summary>
            /// Gets the data memory bank selector.
            /// </summary>
            public Constant BankSelector { get; }

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
                if (!this.traits.GetTrait(TypeOfMemory, SubtypeOfMemory, out IPICMemTrait? trait))
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
            public PICMemoryDomain TypeOfMemory => PICMemoryDomain.Data;

            /// <summary>
            /// Gets the subtype of the Linear Access memory region.
            /// </summary>
            public PICMemorySubDomain SubtypeOfMemory => PICMemorySubDomain.Linear;

            /// <summary>
            /// Gets the memory characteristics of the Linear Access data memory region.
            /// </summary>
            public IPICMemTrait Trait { get; }

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
            public Address? RemapAddress(Address aFSRAddr)
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
            protected readonly PICMemoryMap map;
            protected readonly MemTraits traits;
            protected List<T> memRegions;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pic">The PIC definition.</param>
            /// <param name="map">The memory map.</param>
            /// <param name="traits">The PIC memory traits.</param>
            protected MemoryMapBase(PICMemoryMap map, MemTraits traits)
            {
                this.map = map ?? throw new ArgumentNullException(nameof(map));
                this.traits = traits ?? throw new ArgumentNullException(nameof(traits));
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
            public IMemoryRegion? GetRegionByName(string sRegionName)
                => memRegions.Find(r => r.RegionName == sRegionName);

            /// <summary>
            /// Gets a data memory region given a memory virtual address.
            /// </summary>
            /// <param name="virtAddr">The memory address.</param>
            /// <returns>
            /// The data memory region.
            /// </returns>
            public IMemoryRegion? GetRegionByAddress(Address virtAddr)
                => memRegions.Find(r => r.Contains(virtAddr));

            public virtual void AddRegion(T regn)
            {
                memRegions.Add(regn);
                map.AddSubDomain(regn.SubtypeOfMemory);
            }


        }

        /// <summary>
        /// This class defines the program memory map of current PIC.
        /// </summary>
        protected sealed class ProgMemoryMap : MemoryMapBase<ProgMemRegion>
        {
            private readonly bool isPIC18 = false;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="map">The PIC memory map.</param>
            /// <param name="traits">The PIC memory traits.</param>
            public ProgMemoryMap(PICMemoryMap map, MemTraits traits) : base(map, traits)
            {
                IPICDescriptor pic = map.PIC;

                isPIC18 = pic.Family == PICFamily.PIC18;                

                foreach (var reg in pic.ProgMemoryRegions)
                {
                    if (reg.MemorySubDomain == PICMemorySubDomain.Test)
                        continue;
                    var memrng = CreateMemRange(reg.BeginAddr, reg.EndAddr);
                    AddRegion(new ProgMemRegion(traits, reg.RegionID, memrng, reg.MemorySubDomain));
                }
            }

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
            {
                if (!isPIC18)
                {
                    begAddr <<= 1;
                    endAddr <<= 1;
                }
                return new AddressRange(Address.Ptr32(begAddr), Address.Ptr32(endAddr));
            }

        }

        /// <summary>
        /// This class defines the data memory map of current PIC.
        /// </summary>
        protected sealed class DataMemoryMap : MemoryMapBase<DataMemRegion>
        {

            public Address?[] remapTable;


            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pic">The PIC definition.</param>
            /// <param name="traits">The PIC memory traits.</param>
            /// <param name="mode">The PIC execution  mode.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC data memory size is invalid.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC execution mode is invalid.</exception>
            public DataMemoryMap(PICMemoryMap map, MemTraits traits, PICExecMode mode) : base(map, traits)
            {
                var pic = map?.PIC ?? throw new ArgumentNullException(nameof(map));
                if (mode == PICExecMode.Extended && !pic.HasExtendedMode)
                    throw new InvalidOperationException("Extended execution mode is not supported by this PIC");
                if (traits is null)
                    throw new ArgumentNullException(nameof(traits));

                if (pic.DataSpaceSize < MinDataMemorySize)
                    throw new ArgumentOutOfRangeException("Too low data memory size. Check PIC definition.");
                remapTable = new Address?[pic.DataSpaceSize];
                for (int i = 0; i < remapTable.Length; i++)
                    remapTable[i] = null!;

                foreach (var regn in pic.DataMemoryRegions(mode))
                {
                    var memrng = CreateMemRange(regn.BeginAddr, regn.EndAddr);
                    map.AddSubDomain(regn.MemorySubDomain);
                    var dregn = new DataMemRegion(traits, regn.RegionID, memrng, regn.MemorySubDomain, Constant.UInt16((ushort)regn.Bank));
                    var idRef = regn.ShadowIDRef;
                    if (!string.IsNullOrWhiteSpace(idRef))
                    {
                        var remap = GetRegionByName(idRef) ?? throw new ArgumentOutOfRangeException(nameof(idRef));
                        dregn.PhysicalByteAddrRange = remap.PhysicalByteAddrRange;
                    }
                    AddRegion(dregn);
                    switch (regn.MemorySubDomain)
                    {
                        case PICMemorySubDomain.GPR:
                        case PICMemorySubDomain.DPR:
                            for (int i = 0; i < dregn.Size; i++)
                            {
                                remapTable[dregn.LogicalByteAddrRange.Begin.ToUInt16() + i] = dregn.PhysicalByteAddrRange.Begin + i;
                            }
                            break;
                        case PICMemorySubDomain.SFR:
                            break;
                    }

                }

                foreach (var mir in pic.MirroringRegions)
                {
                    for (int i = 0; i < mir.ByteSize; i++)
                    {
                        remapTable[mir.Addr + i] = Address.Ptr16((ushort)(mir.Addr + i));
                    }
                }

                foreach (var sfr in pic.SFRs)
                {
                    for (int i = 0; i < sfr.ByteWidth; i++)
                    {
                        remapTable[sfr.Addr + i] = Address.Ptr16((ushort)(sfr.Addr + i));
                    }
                }


            }

            /// <summary>
            /// Gets a data memory region by its bank selector.
            /// </summary>
            /// <param name="bankSel">The memory bank selector.</param>
            /// <returns>
            /// The region by selector.
            /// </returns>
            public IMemoryRegion? GetRegionBySelector(Constant bankSel)
                => memRegions.Find(r => r.BankSelector == bankSel);

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
                => new AddressRange(Address.Ptr16((ushort)begAddr), Address.Ptr16((ushort)endAddr));

        }

        #endregion

        /// <summary>
        /// Minimum size in bytes of the PIC data memory space.
        /// </summary>
        public const int MinDataMemorySize = 12;

        protected readonly MemTraits traits;
        protected readonly ProgMemoryMap progMap;
        protected DataMemoryMap dataMap;
        private HashSet<PICMemorySubDomain> subdomains = new HashSet<PICMemorySubDomain>();


        protected PICMemoryMap()
        {
            this.PIC = null!;
            this.dataMap = null!;
            this.progMap = null!;
            this.traits = null!;
        }

        protected PICMemoryMap(IPICDescriptor thePIC)
        {
            PIC = thePIC;
            traits = new MemTraits(thePIC);
            progMap = new ProgMemoryMap(this, traits);
            dataMap = new DataMemoryMap(this, traits, PICExecMode.Traditional);
        }

        #region Methods

        protected void AddSubDomain(PICMemorySubDomain subdom) => subdomains.Add(subdom);

        public bool HasSubDomain(PICMemorySubDomain subdom) => subdomains.Contains(subdom);

        protected static bool IsValidMap(PICMemoryMap map)
        {
            if (map.traits is null)
                return false;
            if (map.progMap is null)
                return false;
            if (!map.HasSubDomain(PICMemorySubDomain.DeviceConfig))
                return false;
            if (!map.HasSubDomain(PICMemorySubDomain.Code) && !map.HasSubDomain(PICMemorySubDomain.ExtCode))
                return false;
            if (map.dataMap is null)
                return false;
            if (!map.HasSubDomain(PICMemorySubDomain.GPR))
                return false;
            if (!map.HasSubDomain(PICMemorySubDomain.SFR))
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
        public IPICDescriptor PIC { get; }

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
        public IMemoryRegion? GetDataRegionByName(string sRegionName) => dataMap.GetRegionByName(sRegionName);

        /// <summary>
        /// Gets a data memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion? GetDataRegionByAddress(Address aVirtAddr) => dataMap.GetRegionByAddress(aVirtAddr);

        /// <summary>
        /// Gets a data memory region given a bank selector.
        /// </summary>
        /// <param name="bankSel">The data memory bank selector.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion? GetDataRegionBySelector(Constant bankSel) => dataMap.GetRegionBySelector(bankSel);

        /// <summary>
        /// Gets a list of data regions.
        /// </summary>
        public IReadOnlyList<IMemoryRegion> DataRegionsList => dataMap.RegionsList;

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        public IEnumerable<IMemoryRegion> DataRegions => dataMap.Regions;

        public ILinearRegion LinearSector => throw new NotImplementedException(nameof(ILinearRegion));

        /// <summary>
        /// Remap a data memory address.
        /// </summary>
        /// <param name="lAddr">The logical memory address.</param>
        /// <returns>
        /// The physical memory address.
        /// </returns>
        public abstract PICDataAddress RemapDataAddress(PICDataAddress lAddr);

        /// <summary>
        /// Try to remap a data memory banked address.
        /// </summary>
        /// <param name="bAddr">The memory banked address to check.</param>
        /// <param name="absAddr">[out] The absolute data memory address.</param>
        /// <returns>
        /// True if successfully translated banked address to absolute address.
        /// </returns>
        public abstract bool TryGetAbsDataAddress(PICBankedAddress bAddr, out PICDataAddress absAddr);

        #endregion

        #region Program memory related

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sRegionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region or null.
        /// </returns>
        public IMemoryRegion? GetProgramRegionByName(string sRegionName)
            => progMap.GetRegionByName(sRegionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public IMemoryRegion? GetProgramRegionByAddress(Address aVirtAddr)
            => progMap.GetRegionByAddress(aVirtAddr);

        /// <summary>
        /// Gets a list of program regions.
        /// </summary>
        public IReadOnlyList<IMemoryRegion> ProgramRegionsList => progMap.RegionsList;

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        public IEnumerable<IMemoryRegion> ProgramRegions => progMap.Regions;

        #endregion

        /// <summary>
        /// Provides a memory sub-domain's location and word sizes.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="PICMemorySubDomain"/>
        ///                      enumeration.</param>
        /// <returns>
        /// A Tuple containing the location size and wordsize. Returns (0,0) if the subdomain does not
        /// exist.
        /// </returns>
        public (uint LocSize, uint WordSize) SubDomainSizes(PICMemorySubDomain subdom)
        {
            if (traits.GetTrait(subdom, out IPICMemTrait? t))
                return (t.LocSize, t.WordSize);
            return (0, 0);
        }

        /// <summary>
        /// Query if memory address <paramref name="bAddr"/> can be a FSR2 index.
        /// </summary>
        /// <param name="bAddr">The memory address to check.</param>
        public abstract bool CanBeFSR2IndexAddress(PICBankedAddress bAddr);

        /// <summary>
        /// Creates a data memory banked address.
        /// </summary>
        /// <param name="bankSel">The data memory bank selector.</param>
        /// <param name="offset">The offset in the data memory bank.</param>
        /// <param name="access">True if Access addressing mode.</param>
        public abstract PICBankedAddress CreateBankedAddr(Constant bankSel, Constant offset, bool access);

        #endregion

    }

}
