#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface provides access to the PIC definitions (architecture, memory regions, instruction set, etc...).
    /// It permits to dissociate the physical implementation of the Microchip XML definition file from
    /// the access by consumers of the PIC descriptor.
    /// </summary>
    public interface IPICDescriptor
    {
        /// <summary>
        /// Gets the version number of the interface
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx)
        /// </summary>
        string ArchName { get; }

        /// <summary>
        /// Gets the PIC description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the unique processor identifier. Used by development tools.
        /// </summary>
        int ProcID { get; }

        /// <summary>
        /// Gets a value from <seealso cref="PICFamily"/> enumeration indicating the PIC family, this PIC belongs to.
        /// </summary>
        PICFamily Family { get; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// </summary>
        bool HasExtendedMode { get; }

        /// <summary>
        /// Gets the instruction set identifier of this PIC as a value from the <see cref="InstructionSetID"/> enumeration.
        /// </summary>
        InstructionSetID GetInstructionSetID { get; }

        /// <summary>
        /// Gets the instruction set family name.
        /// </summary>
        string InstructionSetFamily { get; }

        /// <summary>
        /// Gets the PIC architecture main characteristics thru the <see cref="IArchDef"/> interface.
        /// </summary>
        IArchDef ArchDefinitions { get; }

        /// <summary>
        /// Gets the PIC program memory space definitions.
        /// </summary>
        IProgramSpace ProgramMemorySpace { get; }

        /// <summary>
        /// Gets the PIC data memory space definitions.
        /// </summary>
        IDataSpace DataMemorySpace { get; }

        /// <summary>
        /// Gets the PIC registers definitions.
        /// </summary>
        IRegistersDefinitions PICRegisters { get; }

    }

    /// <summary>
    /// This interface provides access to the generic characteristics of the PIC architecture.
    /// </summary>
    public interface IArchDef
    {
        /// <summary> Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture. </summary>
        string Name { get; }

        /// <summary> Gets the description of the PIC architecture. </summary>
        string Description { get; }

        /// <summary> Gets address magic offset in the binary image for EEPROM content. </summary>
        uint MagicOffset { get; }

        /// <summary> Gets the depth of the hardware stack. </summary>
        int HWStackDepth { get; }

        /// <summary> Gets the number of memory banks. </summary>
        int BankCount { get; }

        /// <summary> Gets the memory traits. </summary>
        IEnumerable<ITrait> MemoryTraits { get; }

    }

    /// <summary>
    /// This interface provides traits (characteristics) for a given memory domain/sub-domain.
    /// </summary>
    public interface ITrait : IMemTrait
    {
        /// <summary>
        /// Gets the memory domain.
        /// </summary>
        /// <value>
        /// A value from the enumeration <seealso cref="PICMemoryDomain"/> enumeration.
        /// </value>
        PICMemoryDomain Domain { get; }

        /// <summary>
        /// Gets the memory sub-domain.
        /// </summary>
        /// <value>
        /// A value from the enumeration <seealso cref="PICMemorySubDomain"/> enumeration.
        /// </value>
        PICMemorySubDomain SubDomain { get; }

    }

    /// <summary>
    /// This interface provides information on a memory trait (characteristics).
    /// </summary>
    public interface IMemTrait
    {
        /// <summary>
        /// Gets the size of the memory word (in bytes).
        /// </summary>
        uint WordSize { get; }

        /// <summary>
        /// Gets the memory location access size (in bytes).
        /// </summary>
        uint LocSize { get; }

        /// <summary>
        /// Gets the memory word implementation (bit mask).
        /// </summary>
        uint WordImpl { get; }

        /// <summary>
        /// Gets the initial (erased) memory word value.
        /// </summary>
        uint WordInit { get; }

        /// <summary>
        /// Gets the memory word 'safe' value. (Probably unused)
        /// </summary>
        uint WordSafe { get; }

    }

    /// <summary>
    /// A default memory trait.
    /// </summary>
    public sealed class DefaultMemTrait : ITrait
    {

        /// <summary> Gets the default size of the memory word (in bytes). </summary>
        public uint WordSize => 1;

        /// <summary> Gets the default memory location access size (in bytes). </summary>
        public uint LocSize => 1;

        /// <summary> Gets the default memory word implementation (bit mask). </summary>
        public uint WordImpl => 0xFF;

        /// <summary> Gets the default initial (erased) memory word value. </summary>
        public uint WordInit => 0xFF;

        /// <summary> Gets the default memory word 'safe' value. </summary>
        public uint WordSafe => 0x00;

        public PICMemoryDomain Domain => PICMemoryDomain.Unknown;

        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Undef;

    }

    /// <summary>
    /// This interface permits to access the description of the PIC program memory space.
    /// </summary>
    public interface IProgramSpace
    {
        /// <summary>
        /// Enumerates the program memory regions with address range and attributes.
        /// </summary>
        IEnumerable<IPICMemoryRegion> MemoryRegions { get; }

        /// <summary>
        /// Enumerates the definition of the configuration fuses.
        /// </summary>
        IEnumerable<IDeviceFuse> ConfigurationFuses { get; }

        /// <summary>
        /// Enumerates the device hard-coded infos (device config, device information).
        /// </summary>
        IEnumerable<IDeviceInfoRegister> DeviceHWInfos { get; }

    }

    /// <summary>
    /// This interface permits to access the description of the PIC data memory space.
    /// </summary>
    public interface IDataSpace
    {
        /// <summary>
        /// Gets the size of the data space in bytes.
        /// </summary>
        uint DataSpaceSize { get; }

        /// <summary>
        /// Enumerates all the data memory regions with address range and attributes.
        /// </summary>
        IEnumerable<IPICMemoryRegion> AllMemoryRegions { get; }

        /// <summary>
        /// Enumerates the memory regions when running in the specified PIC execution mode.
        /// </summary>
        /// <param name="mode">The PIC execution mode.</param>
        IEnumerable<IPICMemoryRegion> MemoryRegions(PICExecMode mode);

        /// <summary>
        /// Enumerates the mirroring regions.
        /// </summary>
        IEnumerable<IPICMirroringRegion> Mirrors { get; }

    }

    /// <summary>
    /// This interface permits to retrieve the description of a mirroring memory region.
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

    /// <summary>
    /// This interface provides access to the description of a PIC data memory region.
    /// </summary>
    public interface IPICMemoryRegion : IPICMemoryAddrRange
    {
        /// <summary> Gets the name/identifier of the memory region, if any. </summary>
        string RegionID { get; }

        /// <summary> Gets a value indicating whether this memory region is banked. </summary>
        bool IsBank { get; }

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
    /// This interface permits to get an enumeration of the PIC configuration fuses.
    /// </summary>
    public interface IConfigFuses
    {
        IEnumerable<IDeviceFuse> Fuses { get; }
    }

    /// <summary>
    /// This interface provides access to a PIC device configuration register (configuration fuses).
    /// </summary>
    public interface IDeviceFuse
    {
        /// <summary> Gets the memory address of the device configuration register. </summary>
        int Addr { get; }

        /// <summary> Gets the name of the device configuration register. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the device configuration register. </summary>
        string Description { get; }

        /// <summary> Gets the bit width of the device configuration register. </summary>
        int BitWidth { get; }

        /// <summary> Gets the implemented bit mask of the device configuration register. </summary>
        int ImplMask { get; }

        /// <summary> Gets the access modes of the device configuration register's bits. </summary>
        string AccessBits { get; }

        /// <summary> Gets the default value of the device configuration register. </summary>
        int DefaultValue { get; }

        /// <summary> Gets a value indicating whether this register is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Enumerates the illegal settings for this configuration register. </summary>
        IEnumerable<IDeviceFusesIllegal> IllegalSettings { get; }

        /// <summary> Enumerates the bit fields of the configuration register. </summary>
        IEnumerable<IDeviceFusesField> ConfigFields { get; }

    }

    /// <summary>
    /// This interface provides conditions for illegal device configuration settings.
    /// </summary>
    public interface IDeviceFusesIllegal
    {
        /// <summary> Gets the "when" pattern of the illegal condition. </summary>
        string When { get; }

        /// <summary> Gets the textual description of the illegal condition. </summary>
        string Description { get; }

    }

    /// <summary>
    /// This interface provides access to a PIC device configuration fuses field.
    /// </summary>
    public interface IDeviceFusesField : IRegisterBitField
    {
        /// <summary> Enumerates the semantics of the settings for this configuration field. </summary>
        IEnumerable<IDeviceFusesSemantic> Semantics { get; }

    }

    /// <summary>
    /// This interface provides access to a semantic of a PIC device configuration field (fuses).
    /// </summary>
    public interface IDeviceFusesSemantic
    {
        /// <summary> Gets the name of the fuses field. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the field configuration pattern. </summary>
        string Description { get; }

        /// <summary> Gets the 'when' condition for the field value (configuration pattern). </summary>
        string When { get; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden to language tools. </summary>
        bool IsLangHidden { get; }

    }

    /// <summary>
    /// This interface provides access to a device information sector (either DIA or DCI sector)
    /// </summary>
    public interface IDeviceInfoSector : IPICMemoryRegion
    {
        /// <summary> Enumerates the device information registers. </summary>
        IEnumerable<IDeviceInfoRegister> Registers { get; }
    }

    /// <summary>
    /// This interface provides access to a device information register.
    /// </summary>
    public interface IDeviceInfoRegister
    {
        /// <summary> Gets the address of the device information register. </summary>
        int Addr { get; }

        /// <summary> Gets the name of the device information register. </summary>
        string Name { get; }

        /// <summary> Gets the bit width of the device information register. </summary>
        int BitWidth { get; }
    }

    /// <summary>
    /// This interface provides access to the PIC registers definitions.
    /// </summary>
    public interface IRegistersDefinitions
    {
        /// <summary>
        /// Enumerates the Special Function Registers.
        /// </summary>
        IEnumerable<ISFRRegister> SFRs { get; }

        /// <summary>
        /// Enumerates the Joined Special Function Registers.
        /// </summary>
        IEnumerable<IJoinedRegister> JoinedRegisters { get; }

    }

    public interface IRegisterBasicInfo
    {
        /// <summary> Gets the data memory address of this register. </summary>
        uint Addr { get; }

        /// <summary> Gets the name of this register. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of this register. </summary>
        string Description { get; }

        /// <summary> Gets the bit width of this register. </summary>
        byte BitWidth { get; }

    }

    /// <summary>
    /// This interface provides access to the definition of a Special Function Register (SFR).
    /// </summary>
    public interface ISFRRegister : IRegisterBasicInfo
    {

        /// <summary> Gets the byte width of this SFR. </summary>
        int ByteWidth { get; }

        /// <summary> Gets the implemented bits mask of this SFR. </summary>
        uint ImplMask { get; }

        /// <summary> Gets the access mode bits descriptor for this SFR. </summary>
        string Access { get; }

        /// <summary> Gets the Master Clear (MCLR) bits values (string) of this SFR. </summary>
        string MCLR { get; }

        /// <summary> Gets the Power-ON Reset bits values (string) of this SFR. </summary>
        string POR { get; }

        /// <summary> Gets a value indicating whether this SFR is indirect. </summary>
        bool IsIndirect { get; }

        /// <summary> Gets a value indicating whether this SFR is volatile. </summary>
        bool IsVolatile { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden to MPLAB IDE. </summary>
        bool IsIDEHidden { get; }

        /// <summary> Gets the Non-Memory-Mapped-Register identifier of the SFR. </summary>
        string NMMRID { get; }

        /// <summary> Gets a value indicating whether this SFR is Non-Memory-Mapped. </summary>
        bool IsNMMR { get; }

        /// <summary> Enumerates the definition of the bit fields contained in this SFR. </summary>
        IEnumerable<ISFRBitField> BitFields { get; }

    }

    /// <summary>
    /// This interface provides access to a Special Function Register (SFR) bit field.
    /// </summary>
    public interface ISFRBitField : IRegisterBitField
    {
        /// <summary> Enumerates the list of semantics of this SFR field. </summary>
        IEnumerable<ISFRFieldSemantic> FieldSemantics { get; }
    }

    /// <summary>
    /// This interface provides access to a SFR field semantic (condition of activation in the PIC).
    /// </summary>
    public interface ISFRFieldSemantic
    {
        /// <summary> Gets the textual description of the semantic.</summary>
        string Description { get; }

        /// <summary> Gets the "when" condition of the semantic. </summary>
        string When { get; }
    }

    /// <summary>
    /// This interface provides access to a Joined SFR register.
    /// </summary>
    public interface IJoinedRegister : IRegisterBasicInfo
    {

        /// <summary>
        /// Enumerates the child SFRs of this joined SFR register.
        /// </summary>
        IEnumerable<ISFRRegister> ChildSFRs { get; }

    }

    /// <summary>
    /// This interface provides access to a register bit field definition.
    /// </summary>
    public interface IRegisterBitField
    {
        /// <summary> Gets the name of the register's field. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the field. </summary>
        string Description { get; }

        /// <summary> Gets the bit position of the field. </summary>
        byte BitPos { get; }

        /// <summary> Gets the bit width of the field. </summary>
        byte BitWidth { get; }

        /// <summary> Gets the bit mask of the field in the register. </summary>
        int BitMask { get; }

        /// <summary> Gets a value indicating whether this bit field is globally hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this bit field is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Gets a value indicating whether this bit field is hidden to the MPLAB IDE. </summary>
        bool IsIDEHidden { get; }

    }

    /// <summary>
    /// This interface provides IRQ description for a PIC interrupt vector entry.
    /// </summary>
    public interface IInterrupt
    {
        /// <summary> Gets the IRQ number. </summary>
        uint IRQ { get; }

        /// <summary> Gets the name of the interrupt request.</summary>
        string Name { get; }

        /// <summary> Gets the description of the interrupt request. </summary>
        string Description { get; }
    }

}
