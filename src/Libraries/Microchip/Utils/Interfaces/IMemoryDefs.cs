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

namespace Reko.Libraries.Microchip
{

    /// <summary>
    /// Values that represent PIC memory domains.
    /// </summary>
    public enum PICMemoryDomain : byte
    {
        /// <summary> Memory does not belong to any known PIC memory spaces. </summary>
        Unknown = 0,
        /// <summary> Memory belongs to the PIC Program memory space. </summary>
        Prog,
        /// <summary> Memory belongs to the PIC Data memory space. </summary>
        Data,
        /// <summary> Memory is an absolute memory space. </summary>
        Absolute,
        /// <summary> Memory belongs to some other PIC memory space. </summary>
        Other
    };

    /// <summary>
    /// Values that represent PIC memory sub-domains.
    /// </summary>
    public enum PICMemorySubDomain
    {
        /// <summary>Memory region is undefined (transient value).</summary>
        Undef = -1,
        /// <summary>Data region contains Special Function Registers (SFRDataSector).</summary>
        SFR,
        /// <summary>Data region contains General Purpose Registers (GPRDataSector).</summary>
        GPR,
        /// <summary>Data region contains Dual-Port Registers (DPRDataSector).</summary>
        DPR,
        /// <summary>Data region contains Non-Memory-Map Registers (NMMRPlace).</summary>
        NNMR,
        /// <summary>Data region is a zone reserved for Emulator (EmulatorZone).</summary>
        Emulator,
        /// <summary>Data region is a linear view of data memory (LinearDataSector).</summary>
        Linear,
        /// <summary>Data region is a Direct Access Memory space.</summary>
        DMA,
        /// <summary>Program region contains code (CodeSector).</summary>
        Code,
        /// <summary>Program region contains external code (ExtCodeSector).</summary>
        ExtCode,
        /// <summary>Program region contains Data EEPROM (EEDataSector).</summary>
        EEData,
        /// <summary>Program region contains Configuration Fuses Words (ConfigFuseSector).</summary>
        DeviceConfig,
        /// <summary>Program region contains Device Configuration Information (DCISector).</summary>
        DeviceConfigInfo,
        /// <summary>Program region contains Device Information Array (DIASector).</summary>
        DeviceInfoAry,
        /// <summary>Program region contains User IDs words (UserIDSector).</summary>
        UserID,
        /// <summary>Program region contains Device IDs words (DeviceIDsector).</summary>
        DeviceID,
        /// <summary>Program region contains Revision IDs words (RevisionIDSector).</summary>
        RevisionID,
        /// <summary>Program region is used for Debugger (BACKBUGVectorSector).</summary>
        Debugger,
        /// <summary>Program region contains Calibration words (CalDataZone).</summary>
        Calib,
        /// <summary>Program region is reserved for Factory Tests (TestZone).</summary>
        Test,
        /// <summary>Program region contains unspecified data.</summary>
        Other
    };

    /// <summary>
    /// This interface provides information on PIC memory address range [begin,end) , domain, sub-domain.
    /// </summary>
    public interface IPICMemoryAddrRange
    {

        /// <summary> Gets the beginning address of the memory range. </summary>
        uint BeginAddr { get; }

        /// <summary> Gets the ending (+1) address of the memory range. </summary>
        uint EndAddr { get; }

        /// <summary>
        /// Gets the memory domain of this memory range, a value from the <see cref="MemoryDomain"/> enumeration.
        /// </summary>
        PICMemoryDomain MemoryDomain { get; }

        /// <summary>
        /// Gets the memory sub-domain of this memory range, a value from the <see cref="MemorySubDomain"/> enumeration.
        /// </summary>
        PICMemorySubDomain MemorySubDomain { get; }

    }

    /// <summary>
    /// This interface provides access to a PIC data memory bank region.
    /// </summary>
    public interface IPICMemoryRegion : IPICMemoryAddrRange
    {
        /// <summary> Gets the name/identifier of the memory region, if any. </summary>
        string RegionID { get; }

        /// <summary> Gets the memory bank number, if any </summary>
        int Bank { get; }

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        string ShadowIDRef { get; }

        /// <summary> Gets the shadow memory address offset, if any. </summary>
        int ShadowOffset { get; }

        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        bool IsSection { get; }

        /// <summary> Gets the textual description of the section, if any. </summary>
        string SectionDesc { get; }

        /// <summary> Gets the name of the section, if any. </summary>
        string SectionName { get; }

    }

    /// <summary>
    /// This interface permits to retrieve a mirroring region.
    /// </summary>
    public interface IPICMirroringRegion
    {
        /// <summary> Gets the starting address of the mirroring region. </summary>
        uint Addr { get; }

        /// <summary> Gets the size in bytes of the mirroring region. </summary>
        uint Size { get; }

        /// <summary> Gets the identifier of the target mirrored region. </summary>
        string TargetRegionID { get; }

    }

}
