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

using Microchip.Crownking;
using Reko.Core;
using Reko.Core.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// A factory class which constructs the PIC memory map using the individual PIC definition.
    /// </summary>
    public class PICMemoryMap : IPICMemoryMap
    {

        #region Locals

        private readonly MemTraits traits;
        private readonly ProgMemoryMap progmap;
        private DataMemoryMap datamap;

        /// <summary>
        /// Minimum size in bytes of the PIC data memory space.
        /// </summary>
        public const int MinDataSize = 12;

        #endregion

        #region Helper classes

        /// <summary>
        /// This class defines the current PIC memory traits.
        /// </summary>
        internal class MemTraits : IMemTraitsSymbolVisitor
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
                    if (ReferenceEquals(x, y)) return true;
                    if (x == null || y == null)
                        return false;
                    if (x.Domain == y.Domain)
                        return x.SubDomain == y.SubDomain;
                    return false;
                }

                int IEqualityComparer<MemoryDomainKey>.GetHashCode(MemoryDomainKey key)
                {
                    return ((int)key.Domain << (int)key.SubDomain).GetHashCode(); ;
                }
            }

            #endregion

            #region Locals

            private PIC _pic;
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
                _pic = thePIC ?? throw new ArgumentNullException(nameof(thePIC));
//                _pic.ArchDef.MemTraits.Traits.ForEach((e) => { if (e is IMemTraitsSymbolAcceptor ee) ee.Accept(this); });
                _pic.ArchDef.MemTraits.Traits.OfType<IMemTraitsSymbolAcceptor>().Select((e) => { e.Accept(this); return true; });
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

            #endregion

            #region IMemTraitsSymbolVisitor implementation

            void IMemTraitsSymbolVisitor.Visit(CalDataMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Calib), mTraits);

            void IMemTraitsSymbolVisitor.Visit(CodeMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Code), mTraits);

            void IMemTraitsSymbolVisitor.Visit(ConfigFuseMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Config), mTraits);

            void IMemTraitsSymbolVisitor.Visit(ExtCodeMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.ExtCode), mTraits);

            void IMemTraitsSymbolVisitor.Visit(EEDataMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.EEData), mTraits);

            void IMemTraitsSymbolVisitor.Visit(BackgroundDebugMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Debugger), mTraits);

            void IMemTraitsSymbolVisitor.Visit(ConfigWORMMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Other), mTraits);

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

            void IMemTraitsSymbolVisitor.Visit(TestMemTraits mTraits) { }

            void IMemTraitsSymbolVisitor.Visit(UserIDMemTraits mTraits)
                => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.UserID), mTraits);

            #endregion

        }

        /// <summary>
        /// A PIC memory region.
        /// </summary>
        internal abstract class MemoryRegionBase : IMemoryRegion
        {

            #region Locals

            private readonly MemTraits _traits;

            #endregion

            #region IMemoryRegion implementation

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
            public AddressRange LogicalByteAddress { get; }

            /// <summary>
            /// Gets or sets the physical byte addresses range.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 physical byte addresses of the memory region.
            /// </value>
            public AddressRange PhysicalByteAddress { get; internal set; }

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
            public uint Size => (PhysicalByteAddress == null ? 0 : (uint)(PhysicalByteAddress.End - PhysicalByteAddress.Begin));

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
                _traits = traits;
                RegionName = sRegion;
                if (regnAddr != null)
                {
                    LogicalByteAddress = regnAddr;
                    PhysicalByteAddress = regnAddr;
                }
                else
                {
                    LogicalByteAddress = PhysicalByteAddress = new AddressRange(Address.Ptr32(0), Address.Ptr32(0));
                }
                TypeOfMemory = memDomain;
                SubtypeOfMemory = memSubDomain;
                if (SubtypeOfMemory != MemorySubDomain.NNMR)  // Non-Memory-Mapped-Registers have no memory characteristics.
                {
                    if (!_traits.GetTrait(memDomain, memSubDomain, out MemTrait trait))
                        throw new InvalidOperationException($"Missing characteristics for [{memDomain}/{memSubDomain}] memory region '{RegionName}'");
                    Trait = trait;
                }

            }

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
                if (aFragAddr == null) return false;
                return ((aFragAddr >= LogicalByteAddress.Begin) && ((aFragAddr + Len) < LogicalByteAddress.End));
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
            {
                return Contains(cAddr.ToUInt32(), Len);
            }

            /// <summary>
            /// Checks whether the given memory fragment is contained in this memory region.
            /// </summary>
            /// <param name="uAddr">The starting memory address of the fragment.</param>
            /// <param name="Len">(Optional) The length in bytes of the fragment (default=0).</param>
            /// <returns>
            /// True if the fragment is contained in this memory region, false if not.
            /// </returns>
            public bool Contains(uint uAddr, uint Len = 0)
            {
                var bAddr = Address.Ptr32(uAddr);
                var eAddr = Address.Ptr32(uAddr + Len);
                return ((bAddr >= LogicalByteAddress.Begin) && (eAddr < LogicalByteAddress.End));
            }

            #endregion

        }

        /// <summary>
        /// A Program PIC memory region.
        /// </summary>
        internal class ProgMemRegion : MemoryRegionBase
        {
            public ProgMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, MemorySubDomain memSubDomain)
                : base(traits, sRegion, regnAddr, MemoryDomain.Prog, memSubDomain)
            {
            }

        }

        /// <summary>
        /// A Data PIC memory region.
        /// </summary>
        internal class DataMemRegion : MemoryRegionBase
        {
            public DataMemRegion(MemTraits traits, string sRegion, AddressRange regnAddr, MemorySubDomain memSubDomain)
                : base(traits, sRegion, regnAddr, MemoryDomain.Data, memSubDomain)
            {
            }

        }

        /// <summary>
        /// A PIC Linear Memory accessed region.
        /// </summary>
        internal class LinearRegion : ILinearRegion
        {

            #region Locals

            private readonly MemTraits _traits;

            #endregion

            #region ILinearRegion implementation

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
            /// Gets the size in bytes of the Linear Access data memory region.
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
                if (!Contains(aFSRAddr)) return null;
                if (!RemapFSRIndirect(aFSRAddr, out Tuple<byte, uint> add)) return null;
                return Address.Ptr16((ushort)(add.Item1 * BankSize + add.Item2));
            }

            /// <summary>
            /// Remap a FSR indirect address in linear data region address to the corresponding GPR bank number and
            /// offset.
            /// </summary>
            /// <param name="aFSRVirtAddr">The virtual data memory byte address.</param>
            /// <returns>
            /// A tuple containing the GPR Bank Number and GPR Offset or NOPHYSICAL_MEM(-1, -1) indicator.
            /// </returns>
            public bool RemapFSRIndirect(Address aFSRVirtAddr, out Tuple<byte, uint> gprBank)
            {
                gprBank = null;
                if (!Contains(aFSRVirtAddr)) return false;
                uint blocksize = (uint)(BlockByteRange.End - BlockByteRange.Begin);
                var fsraddr = aFSRVirtAddr.ToLinear() - FSRByteAddress.Begin.ToLinear();
                byte bankNo = (byte)(fsraddr / blocksize);
                uint bankOff = (uint)(fsraddr % blocksize);
                gprBank = new Tuple<byte, uint>(bankNo, (BlockByteRange.Begin + (int)bankOff).ToUInt32());
                return true;
            }

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
                _traits = traits;
                BankSize = bankSz;
                FSRByteAddress = regnAddr;
                BlockByteRange = blockRng;
                if (!_traits.GetTrait(TypeOfMemory, SubtypeOfMemory, out MemTrait trait))
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

        }

        internal abstract class MemoryMapBase
        {

            protected readonly PIC _pic;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="oPIC">The PIC definition.</param>
            protected MemoryMapBase(PIC oPIC)
            {
                _pic = oPIC;
            }

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
            /// Gets the memory regions contained in this memory map.
            /// </summary>
            /// <value>
            /// The list of memory regions.
            /// </value>
            public abstract IReadOnlyList<IMemoryRegion> Regions { get; }

            /// <summary>
            /// Gets a memory region given its name.
            /// </summary>
            /// <param name="sRegionName">Name of the region.</param>
            /// <returns>
            /// The memory region or null.
            /// </returns>
            public abstract IMemoryRegion GetRegion(string sRegionName);

            /// <summary>
            /// Gets a memory region given a virtual memory address.
            /// </summary>
            /// <param name="addr">The memory address.</param>
            /// <returns>
            /// The memory region or null.
            /// </returns>
            public abstract IMemoryRegion GetRegion(Address addr);

        }

        private class ProgMemoryMap : MemoryMapBase, IMemProgramRegionVisitor
        {

            #region Locals

            private readonly MemTraits traits;
            private List<ProgMemRegion> _memregions;

            #endregion

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="oPIC">The PIC definition.</param>
            /// <param name="traits">The PIC memory traits.</param>
            public ProgMemoryMap(PIC oPIC, MemTraits traits) : base(oPIC)
            {
                this.traits = traits;
                _memregions = new List<ProgMemRegion>();
                oPIC.ProgramSpace.Sectors?.ForEach((e) => { if (e is IMemProgramRegionAcceptor ee) ee.Accept(this); });
            }

            /// <summary>
            /// Gets the memory regions contained in this program memory map.
            /// </summary>
            /// <value>
            /// The list of program memory regions.
            /// </value>
            public override IReadOnlyList<IMemoryRegion> Regions => _memregions;

            /// <summary>
            /// Gets a program memory region given its name ID.
            /// </summary>
            /// <param name="sRegionName">Name ID of the memory region.</param>
            /// <returns>
            /// The program memory region.
            /// </returns>
            public override IMemoryRegion GetRegion(string sRegionName)
                => _memregions?.Find((r) => r.RegionName == sRegionName);

            /// <summary>
            /// Gets a program memory region given a memory virtual address.
            /// </summary>
            /// <param name="aVirtAddr">The memory address.</param>
            /// <returns>
            /// The program memory region.
            /// </returns>
            public override IMemoryRegion GetRegion(Address aVirtAddr)
                => _memregions?.Find((regn) => regn.Contains(aVirtAddr));

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Debugger program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Debugger program sectors, false if not.
            /// </value>
            public bool HasDebug { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has one or more Code program sectors.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has one or more Code program sectors, false if not.
            /// </value>
            public bool HasCode { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has one or more External Code program sectors.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has one or more External Code program sectors, false if not.
            /// </value>
            public bool HasExtCode { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Calibration program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Calibration program sector, false if not.
            /// </value>
            public bool HasCalibration { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Configuration Fuses program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Configuration Fuses program sector, false if not.
            /// </value>
            public bool HasConfigFuse { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Device ID program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Device ID program sector, false if not.
            /// </value>
            public bool HasDeviceID { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Data EEPROM program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Data EEPROM program sector, false if not.
            /// </value>
            public bool HasEEData { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a User ID program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a User ID program sector, false if not.
            /// </value>
            public bool HasUserID { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Revision ID program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Revision ID program sector, false if not.
            /// </value>
            public bool HasRevisionID { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Device Information Area program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Device Information Area program sector, false if not.
            /// </value>
            public bool HasDIA { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC memory map has a Device Configuration Information program sector.
            /// </summary>
            /// <value>
            /// True if this PIC memory map has a Device Configuration Information program sector, false if not.
            /// </value>
            public bool HasDCI { get; private set; } = false;

            #region IMemProgramRegionVisitor implementation

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
            {
                if (!_pic.IsPIC18)
                {
                    begAddr <<= 1;
                    endAddr <<= 1;
                }
                return new AddressRange(Address.Ptr32(begAddr), Address.Ptr32(endAddr));
            }

            void IMemProgramRegionVisitor.Visit(BACKBUGVectorSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasDebug = true;
            }

            void IMemProgramRegionVisitor.Visit(CalDataZone xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasCalibration = true;
            }

            void IMemProgramRegionVisitor.Visit(CodeSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasCode = true;
            }

            void IMemProgramRegionVisitor.Visit(ConfigFuseSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasConfigFuse = true;
            }

            void IMemProgramRegionVisitor.Visit(DeviceIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasDeviceID = true;
            }

            void IMemProgramRegionVisitor.Visit(EEDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasEEData = true;
            }

            void IMemProgramRegionVisitor.Visit(ExtCodeSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasExtCode = true;
            }

            void IMemProgramRegionVisitor.Visit(RevisionIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasRevisionID = true;
            }

            void IMemProgramRegionVisitor.Visit(TestZone xmlRegion) { }

            void IMemProgramRegionVisitor.Visit(UserIDSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasUserID = true;
            }

            void IMemProgramRegionVisitor.Visit(DIASector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasDIA = true;
            }

            void IMemProgramRegionVisitor.Visit(DCISector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                _memregions.Add(new ProgMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain));
                HasDCI = true;
            }

            #endregion

        }

        private class DataMemoryMap : MemoryMapBase, IMemDataRegionVisitor, IMemDataSymbolVisitor
        {

            #region Locals

            private readonly MemTraits traits;
            private List<DataMemRegion> _memregions = null;
            private Address[] _remaptable;
            internal IMemoryRegion Emulatorzone;
            internal ILinearRegion Linearsector;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool _isNMMR = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ushort _currLoadAddr;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ushort _currRelAddr;

            #endregion

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="oPIC">The PIC definition.</param>
            /// <param name="traits">The PIC memory traits.</param>
            /// <param name="mode">ThePIC execution  mode.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC data memory size is invalid.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC execution mode is invalid.</exception>
            public DataMemoryMap(PIC oPIC, MemTraits traits, PICExecMode mode) : base(oPIC)
            {
                this.traits = traits;
                _memregions = new List<DataMemRegion>();
                uint datasize = oPIC.DataSpace?.EndAddr ?? 0;
                if (datasize < 12) throw new ArgumentOutOfRangeException("Too low data memory size. Check PIC definition.");
                _remaptable = new Address[datasize];
                for (int i = 0; i < _remaptable.Length; i++)
                    _remaptable[i] = null;
                oPIC.DataSpace.RegardlessOfMode.Regions?.ForEach((e) => { if (e is IMemDataRegionAcceptor ee) ee.Accept(this); });
                switch (mode)
                {
                    case PICExecMode.Traditional:
                        oPIC.DataSpace.TraditionalModeOnly?.ForEach((e) => { if (e is IMemDataRegionAcceptor ee) ee.Accept(this); });
                        break;
                    case PICExecMode.Extended:
                        if (!oPIC.IsExtended) throw new InvalidOperationException("Extended execution mode is not supported by this PIC");
                        oPIC.DataSpace.ExtendedModeOnly?.ForEach((e) => { if (e is IMemDataRegionAcceptor ee) ee.Accept(this); });
                        break;
                }
                oPIC.IndirectSpace?.ForEach((e) => { if (e is IMemDataRegionAcceptor ee) ee.Accept(this); });

            }

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has one or more SFR (Special
            /// Functions Register) data sectors.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has one or more SFR data sectors, false if not.
            /// </value>
            public bool HasSFR { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has one or more GPR (General
            /// Purpose Register) data sectors.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has one or more GPR data sectors, false if not.
            /// </value>
            public bool HasGPR { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has one or more DPR (Dual-Port
            /// Register) data sectors.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has one or more DPR data sectors, false if not.
            /// </value>
            public bool HasDPR { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has one or more NMMR (Non-Memory-
            /// Mapped Register) definitions.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has one or more NMMR (Non-Memory-Mapped Register) definitions,
            /// false if not.
            /// </value>
            public bool HasNMMR { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has a zone reserved for Emulator.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has a zone reserved for Emulator, false if not.
            /// </value>
            public bool HasEmulatorZone { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating whether this PIC data memory map has a Linear Access data sector.
            /// </summary>
            /// <value>
            /// True if this PIC data memory map has a Linear Access data sector, false if not.
            /// </value>
            public bool HasLinear { get; private set; } = false;

            /// <summary>
            /// Gets the data memory regions contained in this data memory map.
            /// </summary>
            /// <value>
            /// The list of data memory regions.
            /// </value>
            public override IReadOnlyList<IMemoryRegion> Regions => _memregions;

            /// <summary>
            /// Gets a data memory region given its name ID.
            /// </summary>
            /// <param name="sRegionName">Name ID of the memory region.</param>
            /// <returns>
            /// The data memory region.
            /// </returns>
            public override IMemoryRegion GetRegion(string sRegionName)
                => _memregions.Find((r) => r.RegionName == sRegionName);

            /// <summary>
            /// Gets a data memory region given a memory virtual address.
            /// </summary>
            /// <param name="virtAddr">The memory address.</param>
            /// <returns>
            /// The data memory region.
            /// </returns>
            public override IMemoryRegion GetRegion(Address virtAddr)
                => _memregions.Find((regn) => regn.Contains(virtAddr));

            #region IMemDataRegionVisitor implementation

            #region Helpers

            private void _resetAddrs(ushort newAddr)
            {
                _currLoadAddr = newAddr;
                _currRelAddr = 0;
            }

            private void _updateAddr(int incr)
            {
                if (incr > 0)
                {
                    _currLoadAddr += (ushort)incr;
                    _currRelAddr += (ushort)incr;

                }
                else if (incr < 0)
                {
                    _currLoadAddr -= (ushort)(-incr);
                    _currRelAddr -= (ushort)(-incr);
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(incr));
            }

            #endregion

            protected override AddressRange CreateMemRange(uint begAddr, uint endAddr)
            {
                return new AddressRange(Address.Ptr16((ushort)begAddr), Address.Ptr16((ushort)endAddr));
            }

            void IMemDataRegionVisitor.Visit(SFRDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain);
                _memregions.Add(regn);
                HasSFR = true;
                _resetAddrs(regn.LogicalByteAddress.Begin.ToUInt16());
                _isNMMR = false;
                foreach (var ent in xmlRegion.SFRs)
                {
                    if (ent is IMemDataSymbolAcceptor ient) ient.Accept(this);
                }
            }

            void IMemDataRegionVisitor.Visit(GPRDataSector xmlRegion)
            {
                AddressRange memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr);
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain);
                if (!string.IsNullOrWhiteSpace(xmlRegion.ShadowIDRef))
                {
                    var remap = GetRegion(xmlRegion.ShadowIDRef);
                    if (remap == null) throw new ArgumentOutOfRangeException(nameof(xmlRegion.ShadowIDRef));
                    regn.PhysicalByteAddress = remap.LogicalByteAddress;
                }
                _memregions.Add(regn);
                HasGPR = true;
                for (int i = 0; i < regn.Size; i++)
                {
                    _remaptable[regn.LogicalByteAddress.Begin.ToUInt16() + i] = regn.PhysicalByteAddress.Begin + i;
                }
            }

            void IMemDataRegionVisitor.Visit(DPRDataSector xmlRegion)
            {
                AddressRange memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain);
                if (!string.IsNullOrWhiteSpace(xmlRegion.ShadowIDRef))
                {
                    var remap = GetRegion(xmlRegion.ShadowIDRef);
                    if (remap == null) throw new ArgumentOutOfRangeException(nameof(xmlRegion.ShadowIDRef));
                    regn.PhysicalByteAddress = remap.LogicalByteAddress;
                }
                _memregions.Add(regn);
                HasDPR = true;
                for (int i = 0; i < regn.LogicalByteAddress.End - regn.LogicalByteAddress.Begin; i++)
                {
                    _remaptable[regn.LogicalByteAddress.Begin.ToUInt16() + i] = regn.PhysicalByteAddress.Begin + i;
                }
            }

            void IMemDataRegionVisitor.Visit(EmulatorZone xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                Emulatorzone = new DataMemRegion(traits, xmlRegion.RegionID, memrng, xmlRegion.MemorySubDomain);
                HasEmulatorZone = true;
            }

            void IMemDataRegionVisitor.Visit(NMMRPlace xmlRegion)
            {
                var regn = new DataMemRegion(traits, xmlRegion.RegionID, null, MemorySubDomain.NNMR);
                _memregions.Add(regn);
                HasNMMR = true;
                _isNMMR = true;
                foreach (var ent in xmlRegion.SFRDefs)
                    ent.Accept(this);
                _isNMMR = false;
            }

            void IMemDataRegionVisitor.Visit(LinearDataSector xmlRegion)
            {
                var memrng = CreateMemRange(xmlRegion.BeginAddr, xmlRegion.EndAddr); ;
                var blkrng = CreateMemRange(xmlRegion.BlockBeginAddr, xmlRegion.BlockEndAddr); ;
                Linearsector = new LinearRegion(traits, xmlRegion.BankSize, memrng, blkrng);
                HasLinear = true;
            }

            #endregion

            #region IMemDataSymbolVisitor implementation

            void IMemDataSymbolVisitor.Visit(DataBitAdjustPoint xmlSymb)
            {
                // Do nothing. We are not interested here in SFR bits definition.
            }

            void IMemDataSymbolVisitor.Visit(DataByteAdjustPoint xmlSymb)
            {
                _updateAddr(xmlSymb.Offset);
            }

            void IMemDataSymbolVisitor.Visit(SFRDef xmlSymb)
            {
                if (_isNMMR) return;
                _remaptable[_currLoadAddr] = Address.Ptr16(_currLoadAddr);
                _updateAddr((int)xmlSymb.ByteWidth);
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
                        _remaptable[_currLoadAddr + i] = regn.PhysicalByteAddress.Begin + i;
                }
                _updateAddr(xmlSymb.NzSize);
            }

            void IMemDataSymbolVisitor.Visit(JoinedSFRDef xmlSymb)
            {
                xmlSymb.SFRs?.ForEach((e) => e.Accept(this));
            }

            void IMemDataSymbolVisitor.Visit(MuxedSFRDef xmlSymb)
            {
                for (int i = 0; i < ((xmlSymb.NzWidth + 7) >> 3); i++)
                {
                    _remaptable[_currLoadAddr + i] = Address.Ptr16((ushort)(_currLoadAddr + i));
                }
                _updateAddr(xmlSymb.ByteWidth);
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

        #region Constructors

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PICMemoryMap()
        {
        }

        /// <summary>
        /// Private constructor creating an instance of memory map for specified PIC.
        /// </summary>
        /// <param name="thePIC">the PIC descriptor.</param>
        private PICMemoryMap(PIC thePIC)
        {
            PIC = thePIC;
            traits = new MemTraits(thePIC);
            progmap = new ProgMemoryMap(thePIC, traits);
            datamap = new DataMemoryMap(thePIC, traits, PICExecMode.Traditional);
        }

        /// <summary>
        /// Creates a new <see cref="IPICMemoryMap"/> instance.
        /// </summary>
        /// <param name="thePIC">the PIC descriptor.</param>
        /// <returns>
        /// A <see cref="IPICMemoryMap"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thePIC"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid data memory size (less than 12 bytes).</exception>
        public static IPICMemoryMap Create(PIC thePIC)
        {
            if (thePIC == null) throw new ArgumentNullException(nameof(thePIC));
            uint datasize = thePIC.DataSpace?.EndAddr ?? 0;
            if (datasize < MinDataSize)
                throw new ArgumentOutOfRangeException($"Too low data memory size (less than {MinDataSize} bytes). Check PIC definition.");
            return new PICMemoryMap(thePIC);
        }

        #endregion

        #region IPICMemoryMap interface

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
        public PICExecMode ExecMode
        {
            get { return _execMode; }
            set
            {
                if (InstructionSetID == InstructionSetID.PIC18)
                    value = PICExecMode.Traditional;
                if (value != _execMode)
                {
                    _execMode = value;
                    datamap = new DataMemoryMap(PIC, traits, _execMode);
                }
            }
        }
        private PICExecMode _execMode = PICExecMode.Traditional;

        #region Data memory related

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sRegionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion GetDataRegion(string sRegionName)
            => datamap.GetRegion(sRegionName);

        /// <summary>
        /// Gets a data memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public IMemoryRegion GetDataRegion(Address aVirtAddr)
            => datamap.GetRegion(aVirtAddr);

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        /// <value>
        /// The data regions enumeration.
        /// </value>
        public IReadOnlyList<IMemoryRegion> DataRegions => datamap.Regions;

        /// <summary>
        /// Gets the data memory Emulator zone. Valid only if <seealso cref="HasEmulatorZone"/> is true.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        public IMemoryRegion EmulatorZone => datamap.Emulatorzone;

        /// <summary>
        /// Gets the Linear Data Memory definition. Valid only if <seealso cref="HasLinear"/> is true.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        public ILinearRegion LinearSector => datamap.Linearsector;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more SFR (Special
        /// Functions Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more SFR data sectors, false if not.
        /// </value>
        public bool HasSFR => datamap.HasSFR;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more GPR (General
        /// Purpose Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more GPR data sectors, false if not.
        /// </value>
        public bool HasGPR => datamap.HasGPR;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more DPR (Dual-Port
        /// Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more DPR data sectors, false if not.
        /// </value>
        public bool HasDPR => datamap.HasDPR;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more NMMR (Non-Memory-
        /// Mapped Register) definitions.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more NMMR (Non-Memory-Mapped Register) definitions,
        /// false if not.
        /// </value>
        public bool HasNMMR => datamap.HasNMMR;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a zone reserved for Emulator.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a zone reserved for Emulator, false if not.
        /// </value>
        public bool HasEmulatorZone => datamap.HasEmulatorZone;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Linear Access data sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Linear Access data sector, false if not.
        /// </value>
        public bool HasLinear => datamap.HasLinear;

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
            => progmap.GetRegion(sRegionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual address.
        /// </summary>
        /// <param name="aVirtAddr">The memory address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public IMemoryRegion GetProgramRegion(Address aVirtAddr)
            => progmap.GetRegion(aVirtAddr);

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        /// <value>
        /// The program regions enumeration.
        /// </value>
        public IReadOnlyList<IMemoryRegion> ProgramRegions => progmap.Regions;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Debugger program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Debugger program sectors, false if not.
        /// </value>
        public bool HasDebug => progmap.HasDebug;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more Code program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more Code program sectors, false if not.
        /// </value>
        public bool HasCode => progmap.HasCode;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more External Code
        /// program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more External Code program sectors, false if not.
        /// </value>
        public bool HasExtCode => progmap.HasExtCode;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Calibration program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Calibration program sector, false if not.
        /// </value>
        public bool HasCalibration => progmap.HasCalibration;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Configuration Fuses program
        /// sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Configuration Fuses program sector, false if not.
        /// </value>
        public bool HasConfigFuse => progmap.HasConfigFuse;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device ID program sector, false if not.
        /// </value>
        public bool HasDeviceID => progmap.HasDeviceID;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Data EEPROM program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Data EEPROM program sector, false if not.
        /// </value>
        public bool HasEEData => progmap.HasEEData;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a User ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a User ID program sector, false if not.
        /// </value>
        public bool HasUserID => progmap.HasUserID;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Revision ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Revision ID program sector, false if not.
        /// </value>
        public bool HasRevisionID => progmap.HasRevisionID;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Information Area
        /// program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Information Area program sector, false if not.
        /// </value>
        public bool HasDIA => progmap.HasDIA;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Configuration
        /// Information (DCI) program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Configuration Information program sector, false if
        /// not.
        /// </value>
        public bool HasDCI => progmap.HasDCI;

        #endregion

        #endregion

    }

}
