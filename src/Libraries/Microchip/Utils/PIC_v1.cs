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

// summary:	Implements the Microchip PIC XML definition serialization per Microchip Crownking Database.
// This is version 1.
//
namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using System.Linq;

    #region Base common definitions

    /// <summary>
    /// Values that represent PIC memory domains.
    /// </summary>
    public enum MemoryDomain : byte
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
    public enum MemorySubDomain
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
    /// Values that represent PIC Execution modes.
    /// </summary>
    public enum PICExecMode
    {

        /// <summary>
        /// PIC is executing the traditional/legacy instruction set.
        /// </summary>
        Traditional,

        /// <summary>
        /// PIC is executing the extended instruction set with Indexed Literal Offset Addressing mode.
        /// </summary>
        Extended
    };

    /// <summary>
    /// Values that represent PIC Instruction-Set identifiers.
    /// </summary>
    public enum InstructionSetID
    {
        /// <summary> PIC Instruction Set is undefined. </summary>
        UNDEFINED,
        /// <summary> PIC Instruction Set is PIC16 like pic16f77 - basic mid-range. Identified as InstructionSet="pic16f77" in XML definition.</summary>
        PIC16,
        /// <summary> PIC Instruction Set is PIC16 like pic16f1946 - mid-range enhanced 5-bit BSR. Identified as InstructionSet="cpu_mid_v10" in XML definition.</summary>
        PIC16_ENHANCED,
        /// <summary> PIC Instruction Set is PIC16 like pic16f15313 - mid-range enhanced 6-bit BSR. Identified as InstructionSet="cpu_p16f1_v1" in XML definition.</summary>
        PIC16_FULLFEATURED,
        /// <summary> PIC Instruction Set is traditional PIC18 like pic18f1220 - without any extended mode. Identified as InstructionSet="pic18" in XML definition.</summary>
        PIC18,
        /// <summary> PIC Instruction Set is PIC18 like pic18f1230 - with extended execution mode capabilities. Identified as InstructionSet="egg" in XML definition.</summary>
        PIC18_EXTENDED,
        /// <summary> PIC Instruction Set is PIC18 enhanced like pic18f25k42 - same as PIC18_EXTD + some instructions for bigger RAM size. Identified as InstructionSet="cpu_pic18f_v6" in XML definition.</summary>
        PIC18_ENHANCED,
    }

    /// <summary>
    /// SFR bits access modes.
    /// </summary>
    public enum SFRBitAccess : byte
    {
        /// <summary>Bit is read-write ('n') </summary>
        RW = 0,
        /// <summary>Bit is read-write persistent ('N') (no longer used?) </summary>
        RW_Persistant = 1,
        /// <summary>Bit is read-only ('r') </summary>
        ROnly = 2,
        /// <summary>Bit is write-only ('w') </summary>
        WOnly = 3,
        /// <summary>Bit is settable only ('s') </summary>
        Set = 4,
        /// <summary>Bit is clear-able only ('c') </summary>
        Clr = 5,
        /// <summary>Bit is always 0 ('0') </summary>
        Zero = 6,
        /// <summary>Bit is always 1 ('1') </summary>
        One = 7,
        /// <summary>Bit is unimplemented ('-') </summary>
        UnImpl = 8,
        /// <summary>Bit is undetermined ('x') </summary>
        UnDef = 9,
        /// <summary>Bit access mode is unknown ('?') </summary>
        Unknown = 10
    }

    /// <summary>
    /// Values that represent SFR bit state at resets (Master Clear, Power-On, ...).
    /// </summary>
    public enum SFRBitReset
    {
        /// <summary>Bit is reset to 0 ('0'). </summary>
        Zero = 0,
        /// <summary>Bit is reset to 1 ('1'). </summary>
        One = 1,
        /// <summary>Bit is unchanged ('u'). </summary>
        Unchanged = 2,
        /// <summary>Bit depends on condition ('q'). </summary>
        Cond = 3,
        /// <summary>Bit is unknown ('x'). </summary>
        Unknown = 4,
        /// <summary>Bit is not implemented ('-'). </summary>
        UnImpl = 5
    }

    /// <summary>
    /// The abstract class <see cref="MemoryAddrRange"/> represents a PIC memory address range [begin, end) (either in data, program or absolute space).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemoryAddrRange : IMemoryAddrRange,
        IEquatable<MemoryAddrRange>, IEquatable<IMemoryAddrRange>, IEqualityComparer<MemoryAddrRange>,
        IComparable<MemoryAddrRange>, IComparable<IMemoryAddrRange>, IComparer<MemoryAddrRange>
    {

        public MemoryAddrRange() { }

        [XmlIgnore] public abstract MemoryDomain MemoryDomain { get; }
        [XmlIgnore] public abstract MemorySubDomain MemorySubDomain { get; }

        /// <summary>
        /// Used to serialize <see cref="BeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "beginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _beginaddrformatted { get => $"0x{BeginAddr:X}"; set => BeginAddr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the begin address of the memory range.
        /// </summary>
        [XmlIgnore] public uint BeginAddr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "endaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _endaddrformatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the end address of the memory range.
        /// </summary>
        [XmlIgnore] public uint EndAddr { get; private set; }


        #region Implementation of the equality/comparison interfaces

        public bool Equals(IMemoryAddrRange other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (MemoryDomain != other.MemoryDomain)
                return false;
            if (BeginAddr != other.BeginAddr)
                return false;
            return (EndAddr == other.EndAddr);
        }

        public bool Equals(MemoryAddrRange other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (MemoryDomain != other.MemoryDomain)
                return false;
            if (BeginAddr != other.BeginAddr)
                return false;
            return (EndAddr == other.EndAddr);
        }

        public override bool Equals(object obj) => Equals(obj as IMemoryAddrRange);

        public override int GetHashCode() => (BeginAddr.GetHashCode() + 17 * EndAddr.GetHashCode()) ^ MemoryDomain.GetHashCode();

        public static bool operator ==(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) == 0;

        public static bool operator !=(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) != 0;

        public int Compare(MemoryAddrRange x, MemoryAddrRange y)
            => _Compare(x, y);

        private static int _Compare(MemoryAddrRange x, IMemoryAddrRange y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if ((object)x == null)
                return -1;
            return x.CompareTo(y);
        }

        public bool Equals(MemoryAddrRange x, MemoryAddrRange y)
        {
            if (ReferenceEquals(x, y))
                return true;
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode(MemoryAddrRange obj) => obj?.GetHashCode() ?? 0;

        public int CompareTo(IMemoryAddrRange other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            if (MemoryDomain != other.MemoryDomain)
                return MemoryDomain.CompareTo(other.MemoryDomain);
            if (BeginAddr == other.BeginAddr)
                return EndAddr.CompareTo(other.EndAddr);
            return BeginAddr.CompareTo(other.BeginAddr);
        }

        public int CompareTo(MemoryAddrRange other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            if (MemoryDomain != other.MemoryDomain)
                return MemoryDomain.CompareTo(other.MemoryDomain);
            if (BeginAddr == other.BeginAddr)
                return EndAddr.CompareTo(other.EndAddr);
            return BeginAddr.CompareTo(other.BeginAddr);
        }

        #endregion

    }

    #endregion

    #region ArchDef XML element

    /// <summary>
    /// Device ID to revision number.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DEVIDToRev
    {
        public DEVIDToRev() { }

        /// <summary>
        /// Gets the silicon revision.
        /// </summary>
        /// <value>
        /// A revision as a string.
        /// </value>
        [XmlAttribute(AttributeName = "revlist", Form = XmlSchemaForm.None, Namespace = "")]
        public string RevList { get; set; }

        /// <summary>
        /// Gets the binary value of the silicon revision.
        /// </summary>
        /// <value>
        /// The value as an integer.
        /// </value>
        [XmlIgnore] public int Value { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Memory trait (characteristics).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemTrait : IMemTrait
    {

        public MemTrait() { }


        /// <summary>
        /// Gets the size of the memory word.
        /// </summary>
        /// <value>
        /// The size of the memory word in bytes.
        /// </value>
        [XmlIgnore] public virtual uint WordSize { get; private set; }

        /// <summary>
        /// Gets the memory location access size.
        /// </summary>
        /// <value>
        /// The size of the location in bytes.
        /// </value>
        [XmlIgnore] public virtual uint LocSize { get; private set; }

        /// <summary>
        /// Gets the memory word implementation (bit mask).
        /// </summary>
        /// <value>
        /// The memory word implementation bit mask.
        /// </value>
        [XmlIgnore] public virtual uint WordImpl { get; private set; }

        /// <summary>
        /// Gets the initial (erased) memory word value.
        /// </summary>
        /// <value>
        /// The word initialize.
        /// </value>
        [XmlIgnore] public virtual uint WordInit { get; private set; }

        /// <summary>
        /// Gets the memory word safe value.
        /// </summary>
        /// <value>
        /// The memory word safe value.
        /// </value>
        [XmlIgnore] public virtual uint WordSafe { get; private set; }


        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "locsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string LocSizeFormatted { get => $"0x{LocSize:X}"; set => LocSize = value.ToUInt32Ex(); }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordimpl", Form = XmlSchemaForm.None, Namespace = "")]
        public string WordImplFormatted { get => $"0x{WordImpl:X}"; set => WordImpl = value.ToUInt32Ex(); }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordinit", Form = XmlSchemaForm.None, Namespace = "")]
        public string WordInitFormatted { get => $"0x{WordInit:X}"; set => WordInit = value.ToUInt32Ex(); }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordsafe", Form = XmlSchemaForm.None, Namespace = "")]
        public string WordSafeFormatted { get => $"0x{WordSafe:X}"; set => WordSafe = value.ToUInt32Ex(); }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string WordSizeFormatted { get => $"0x{WordSize:X}"; set => WordSize = value.ToUInt32Ex(); }

    }

    /// <summary>
    /// A default memory trait.
    /// </summary>
    public sealed class DefaultMemTrait : IMemTrait, ITrait
    {

        /// <summary>
        /// Gets the default size of the memory word.
        /// </summary>
        public uint WordSize => 1;

        /// <summary>
        /// Gets the default memory location access size.
        /// </summary>
        public uint LocSize => 1;

        /// <summary>
        /// Gets the default memory word implementation (bit mask).
        /// </summary>
        public uint WordImpl => 0xFF;

        /// <summary>
        /// Gets the default initial (erased) memory word value.
        /// </summary>
        public uint WordInit => 0xFF;

        /// <summary>
        /// Gets the default memory word safe value.
        /// </summary>
        public uint WordSafe => 0x00;

        public MemoryDomain Domain => MemoryDomain.Unknown;

        public MemorySubDomain SubDomain => MemorySubDomain.Undef;

    }

    /// <summary>
    /// Code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeMemTraits : MemTrait, ITrait
    {
        public CodeMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.Code;

    }

    /// <summary>
    /// External code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeMemTraits : MemTrait, ITrait
    {
        public ExtCodeMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.ExtCode;

    }

    /// <summary>
    /// Calibration data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataMemTraits : MemTrait, ITrait
    {
        public CalDataMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.Calib;

    }

    /// <summary>
    /// Background debug memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BackgroundDebugMemTraits : MemTrait
    {
        public BackgroundDebugMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.Debugger;

    }

    /// <summary>
    /// Test memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestMemTraits : MemTrait
    {
        public TestMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.Test;

    }

    /// <summary>
    /// User IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDMemTraits : MemTrait, ITrait
    {
        public UserIDMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.UserID;

    }

    /// <summary>
    /// Configuration fuses memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseMemTraits : MemTrait, ITrait
    {
        public ConfigFuseMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.DeviceConfig;


        [XmlIgnore] public int UnimplVal { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Configuration Write-Once-Read-Many memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigWORMMemTraits : MemTrait, ITrait
    {
        public ConfigWORMMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.Other;

    }

    /// <summary>
    /// Device IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDMemTraits : MemTrait, ITrait
    {
        public DeviceIDMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.DeviceID;

    }

    /// <summary>
    /// EEPROM data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataMemTraits : MemTrait, ITrait
    {
        public EEDataMemTraits() { }

        [XmlIgnore] public override uint LocSize => 1;
        [XmlIgnore] public override uint WordSize => 1;
        [XmlIgnore] public override uint WordImpl => 0xFF;
        [XmlIgnore] public override uint WordInit => 0xFF;
        [XmlIgnore] public override uint WordSafe => 0xFF;

        public MemoryDomain Domain => MemoryDomain.Prog;

        public MemorySubDomain SubDomain => MemorySubDomain.EEData;


        /// <summary>
        /// Gets address magic offset in the binary image for EEPROM content.
        /// </summary>
        [XmlIgnore] public uint MagicOffset { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "magicoffset", Form = XmlSchemaForm.None, Namespace = "")]
        public string MagicOffsetFormatted { get => $"0x{MagicOffset:X}"; set => MagicOffset = value.ToUInt32Ex(); } 

    }

    /// <summary>
    /// Data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataMemTraits : MemTrait, ITrait
    {
        public DataMemTraits() { }

        public MemoryDomain Domain => MemoryDomain.Data;

        public MemorySubDomain SubDomain => MemorySubDomain.Undef;

    }

    /// <summary>
    /// The various memory regions' traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MemTraits
    {
        public MemTraits() { }


        /// <summary>
        /// Gets the list of memory traits of the various memory regions.
        /// </summary>
        [XmlElement(ElementName = "CodeMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CodeMemTraits))]
        [XmlElement(ElementName = "ExtCodeMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ExtCodeMemTraits))]
        [XmlElement(ElementName = "CalDataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CalDataMemTraits))]
        [XmlElement(ElementName = "BackgroundDebugMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(BackgroundDebugMemTraits))]
        [XmlElement(ElementName = "TestMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(TestMemTraits))]
        [XmlElement(ElementName = "UserIDMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(UserIDMemTraits))]
        [XmlElement(ElementName = "ConfigFuseMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigFuseMemTraits))]
        [XmlElement(ElementName = "ConfigWORMMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigWORMMemTraits))]
        [XmlElement(ElementName = "DeviceIDMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceIDMemTraits))]
        [XmlElement(ElementName = "DataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataMemTraits))]
        [XmlElement(ElementName = "EEDataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EEDataMemTraits))]
        public List<MemTrait> Traits { get; set; }

        /// <summary>
        /// Gets the depth of the hardware stack.
        /// </summary>
        [XmlIgnore] public int HWStackDepth { get; private set; }

        /// <summary>
        /// Gets the number of memory banks.
        /// </summary>
        [XmlIgnore] public int BankCount { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "hwstackdepth", Form = XmlSchemaForm.None, Namespace = "")]
        public string HWStackDepthFormatted { get => $"{HWStackDepth}"; set => HWStackDepth = value.ToInt32Ex(); }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "bankcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string BankCountFormatted { get => $"{BankCount}"; set => BankCount = value.ToInt32Ex(); }

    }

    /// <summary>
    /// PIC memory architecture definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef : IArchDef
    {
        public ArchDef() { }


        /// <summary>
        /// Gets the memory traits of the PIC.
        /// </summary>
        [XmlElement(ElementName = "MemTraits", Form = XmlSchemaForm.None, Namespace = "")]
        public MemTraits MemTraits { get; set; }

        /// <summary>
        /// Gets the description of the PIC architecture.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary>
        /// Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture.
        /// </summary>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        [XmlIgnore] public int BankCount => MemTraits.BankCount;

        [XmlIgnore] public int HWStackDepth => MemTraits.HWStackDepth;

        [XmlIgnore]
        public uint MagicOffset
        {
            get
            {
                if (!magicOffset.HasValue)
                {
                    magicOffset = MemTraits.Traits.OfType<EEDataMemTraits>().Select(t => t.MagicOffset).FirstOrDefault();
                }
                return magicOffset ?? 0;
            }
        }
        private uint? magicOffset;

        [XmlIgnore]
        public IEnumerable<ITrait> MemoryTraits
            => MemTraits.Traits.OfType<ITrait>();

    }

    #endregion

    #region InstructionSet definition

    public sealed class InstructionSet
    {
        public InstructionSet() { }


        /// <summary>
        /// Gets the instruction set ID.
        /// </summary>
        [XmlAttribute(AttributeName = "instructionsetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

    }

    #endregion

    #region InterruptList XML element

    /// <summary>
    /// Interrupt request.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Interrupt
    {
        public Interrupt() { }

        /// <summary>
        /// Used to serialize <see cref="IRQ" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "irq", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string IRQFormatted { get => $"{IRQ}"; set => IRQ = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the IRQ number.
        /// </summary>
        [XmlIgnore] public uint IRQ { get; private set; }

        /// <summary>
        /// Gets the name of the interrupt request.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the interrupt request.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

    }

    #endregion

    #region ProgramSpace XML element

    public abstract class MemProgramSymbolAcceptorBase : IMemProgramSymbolAcceptor
    {

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public abstract void Accept(IMemProgramSymbolVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemProgramSymbolVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A program memory addresses [range)
    /// </summary>
    public abstract class ProgMemoryRange : MemoryAddrRange, IMemoryAddrRange
    {
        public ProgMemoryRange() { }

        public override MemoryDomain MemoryDomain => MemoryDomain.Prog;

    }

    /// <summary>
    /// A Program memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : ProgMemoryRange, IMemoryRegion, IMemProgramRegionAcceptor
    {
        public ProgMemoryRegion() { }


        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }


        #region IMemProgramRegionAcceptor interfaces

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public abstract void Accept(IMemProgramRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemProgramRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// Program Memory region seen as a section.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemorySection : ProgMemoryRegion
    {
        public ProgMemorySection() { }


        /// <summary>
        /// Gets a value indicating whether this memory region is a section.
        /// </summary>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsSection { get; set; }

        /// <summary>
        /// Gets the textual description of the section.
        /// </summary>
        [XmlAttribute(AttributeName = "sectiondesc", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionDesc { get; set; }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        [XmlAttribute(AttributeName = "sectionname", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionName { get; set; }

    }

    /// <summary>
    /// Adjust byte address pointing in program memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgByteAdjustPoint : MemProgramSymbolAcceptorBase, IAdjustPoint
    {
        public ProgByteAdjustPoint() { }


        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the relative byte offset to add for adjustment.
        /// </summary>
        [XmlIgnore] public int Offset { get; private set; }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} byte(s)";
            return $"Adjust 0x{Offset:X} bytes";
        }

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory byte address adjustment symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory byte address adjustment symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Adjust bit address pointing in program memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgBitAdjustPoint : MemProgramSymbolAcceptorBase, IAdjustPoint
    {
        public ProgBitAdjustPoint() { }


        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the relative bit offset to add for adjustment.
        /// </summary>
        [XmlIgnore] public int Offset { get; private set; }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} bit(s)";
            return $"Adjust 0x{Offset:X} bit(s)";
        }

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory bit address adjustment symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory bit address adjustment symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Code memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection
    {
        public CodeSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Code;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this code program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this code program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this code program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Calibration data zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataZone : ProgMemorySection
    {
        public CalDataZone() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Calib;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this calibration data program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this calibration data program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this calibration data program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Test zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestZone : ProgMemoryRegion
    {
        public TestZone() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Test;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this test zone program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this test zone program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this test zone program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// User IDs memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDSector : ProgMemorySection
    {
        public UserIDSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.UserID;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this User ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this User ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this User ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Revision IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RevisionIDSector : ProgMemoryRegion
    {
        public RevisionIDSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.RevisionID;

        /// <summary>
        /// Gets the device ID to silicon revision relationship.
        /// </summary>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Revision ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Revision ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Revision ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection
    {
        public DeviceIDSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceID;

        /// <summary>
        /// Gets the Device IDs to silicon revision level.
        /// </summary>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit mask to isolate device ID.
        /// </summary>
        [XmlIgnore] public int Mask { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Value"/> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the generic value of device ID
        /// </summary>
        [XmlIgnore] public int Value { get; private set; }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Device ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Device ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Device ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register field pattern semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic : MemProgramSymbolAcceptorBase
    {
        public DCRFieldSemantic() { }


        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of the field value.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the when condition for the field value.
        /// </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets a value indicating whether this configuration pattern is hidden.
        /// </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets or sets a value indicating whether this configuration pattern is hidden to language tools.
        /// </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Used to serialize <see cref="OscModeIDRef" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "oscmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OscModeIDRefFormatted { get => $"{OscModeIDRef}"; set => OscModeIDRef = value.ToInt32Ex(); }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool OscModeIDRefFormattedSpecified { get => OscModeIDRef != 0; set { } }

        /// <summary>
        /// Gets the oscillator mode identifier reference.
        /// </summary>
        [XmlIgnore] public int OscModeIDRef { get; private set; }

        [XmlAttribute(AttributeName = "_defeatcoercion", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefeatCoercion { get; set; }

        /// <summary>
        /// Gets the memory mode identifier reference.
        /// </summary>
        [XmlAttribute(AttributeName = "memmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string MemModeIDRef { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register field semantic symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol semantic with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register Field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef : MemProgramSymbolAcceptorBase
    {
        public DCRFieldDef() { }


        /// <summary>
        /// Gets the list of configuration field semantics for various configuration values.
        /// </summary>
        [XmlElement(ElementName = "DCRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of the field.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the bit position of the field. (Not currently populated by Microchip).
        /// </summary>
        [XmlIgnore] public int BitAddr { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit width of the field.
        /// </summary>
        [XmlIgnore] public int NzWidth { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit mask of the field in the register.
        /// </summary>
        [XmlIgnore] public int Mask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden.
        /// </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to language tools.
        /// </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to the MPLAB IDE.
        /// </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register field symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRMode : MemProgramSymbolAcceptorBase
    {
        public DCRMode() { }


        /// <summary>
        /// Gets the fields of the configuration register.
        /// </summary>
        [XmlElement(ElementName = "DCRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgBitAdjustPoint))]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register mode symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register mode symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register illegal definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal : MemProgramSymbolAcceptorBase
    {
        public DCRDefIllegal() { }


        /// <summary>
        /// Gets the "when" pattern of the illegal condition.
        /// </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets the textual description of the illegal condition.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this invalid device configuration register symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this invalid device configuration register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this invalid device configuration register symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef : MemProgramSymbolAcceptorBase
    {
        public DCRDef() { }


        /// <summary>
        /// Gets the list of illegal configuration values (if any).
        /// </summary>
        [XmlElement(ElementName = "Illegal", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRDefIllegal> Illegals { get; set; }

        /// <summary>
        /// Gets a list of Device Configuration Register modes.
        /// </summary>
        [XmlArray(ElementName = "DCRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem(ElementName = "DCRMode", Type = typeof(DCRMode), Form = XmlSchemaForm.None, IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the memory address of the configuration register.
        /// </summary>
        [XmlIgnore] public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the configuration register.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of the configuration register.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit width of the register.
        /// </summary>
        [XmlIgnore] public int NzWidth { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ImplFormatted { get => $"0x{Impl:X}"; set => Impl = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the implemented bit mask.
        /// </summary>
        [XmlIgnore] public int Impl { get; private set; }

        /// <summary>
        /// Gets the access modes of the register's bits.
        /// </summary>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Default" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "default", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string DefaultFormatted { get => $"0x{Default:X}"; set => Default = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the default value of the register.
        /// </summary>
        [XmlIgnore] public int Default { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="FactoryDefault" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "factorydefault", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string FactoryDefaultFormatted { get => $"0x{FactoryDefault:X}"; set => FactoryDefault = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the factory default value of the register.
        /// </summary>
        [XmlIgnore] public int FactoryDefault { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this register is hidden to language tools.
        /// </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Used to serialize <see cref="UnimplVal" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the unimplemented bitmask value.
        /// </summary>
        [XmlIgnore] public int UnimplVal { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Unused" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "unused", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UnusedFormatted { get => $"{Unused}"; set => Unused = value.ToInt32Ex(); }

        [XmlIgnore] public int Unused { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="UseInChecksum" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "useinchecksum", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UseInChecksumFormatted { get => $"0x{UseInChecksum:X}"; set => UseInChecksum = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit mask to use in checksum computation.
        /// </summary>
        [XmlIgnore] public int UseInChecksum { get; private set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Configuration Fuses memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion
    {
        public ConfigFuseSector() { }


        /// <summary>
        /// Gets the list of configuration registers definitions.
        /// </summary>
        [XmlElement(ElementName = "DCRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Defs { get; set; }

        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceConfig;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this configuration fuses program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this configuration fuses program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this configuration fuses program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Background debugger vector memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion
    {
        public BACKBUGVectorSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Debugger;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this debugger program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this debugger program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this debugger program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Data EEPROM memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataSector : ProgMemorySection
    {
        public EEDataSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.EEData;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Data EEPROM program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Data EEPROM program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Data EEPROM program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area (DIA) register.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceRegister : MemProgramSymbolAcceptorBase
    {
        public DeviceRegister() { }


        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the address of the register.
        /// </summary>
        [XmlIgnore] public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the register.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit width of the register.
        /// </summary>
        [XmlIgnore] public int NzWidth { get; private set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device information area program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area register array.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray : MemProgramSymbolAcceptorBase
    {
        public DIARegisterArray() { }


        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceRegister))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Registers { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the starting memory address of the DIA registers.
        /// </summary>
        [XmlIgnore] public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the DIA registers array.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device information areas program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information areas program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information areas program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceInfoAry;

        public DIASector() { }


        /// <summary>
        /// Gets the list of register arrays.
        /// </summary>
        [XmlElement(ElementName = "RegisterArray", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DIARegisterArray> RegisterArrays { get; set; }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this device information area program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Information memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion
    {
        public DCISector() { }


        /// <summary>
        /// Gets the list of configuration information registers.
        /// </summary>
        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DeviceRegister> Registers { get; set; }

        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceConfigInfo;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this device configuration information program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device configuration information program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device configuration information program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// External Code memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion
    {
        public ExtCodeSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.ExtCode;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this external code program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this external code program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this external code program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Interrupt Vector area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class VectorArea
    {
        public VectorArea() { }


        /// <summary>
        /// Used to serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzsizeformatted { get => $"0x{NzSize:X}"; set => NzSize = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bytes size of the area.
        /// </summary>
        [XmlIgnore] public int NzSize { get; private set; }

        /// <summary>
        /// Gets the identifier of the vector area.
        /// </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

    }

    /// <summary>
    /// Program memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {
        public ProgramSpace() { }


        /// <summary>
        /// Gets the list of program memory sectors.
        /// </summary>
        [XmlElement(ElementName = "CodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CodeSector))]
        [XmlElement(ElementName = "CalDataZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CalDataZone))]
        [XmlElement(ElementName = "TestZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(TestZone))]
        [XmlElement(ElementName = "UserIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(UserIDSector))]
        [XmlElement(ElementName = "RevisionIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(RevisionIDSector))]
        [XmlElement(ElementName = "DeviceIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceIDSector))]
        [XmlElement(ElementName = "ConfigFuseSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigFuseSector))]
        [XmlElement(ElementName = "BACKBUGVectorSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(BACKBUGVectorSector))]
        [XmlElement(ElementName = "EEDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EEDataSector))]
        [XmlElement(ElementName = "DIASector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DIASector))]
        [XmlElement(ElementName = "DCISector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCISector))]
        [XmlElement(ElementName = "ExtCodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ExtCodeSector))]
        public List<ProgMemoryRegion> Sectors { get; set; }

        /// <summary>
        /// Gets the list of interrupt vector area.
        /// </summary>
        [XmlElement(ElementName = "VectorArea", Form = XmlSchemaForm.None, Namespace = "")]
        public List<VectorArea> VectorArea { get; set; }

    }

    #endregion

    #region DataSpace XML element

    public abstract class MemDataSymbolAcceptorBase : IMemDataSymbolAcceptor
    {

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory symbol visitor and calls the appropriate
        /// "Visit()" method for this data memory symbol.
        /// </summary>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        public abstract void Accept(IMemDataSymbolVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataSymbolVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context);

        #endregion

    }

    public abstract class MemDataRegionAcceptorBase : IMemDataRegionAcceptor
    {

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the appropriate
        /// "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public abstract void Accept(IMemDataRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A data memory addresses range.
    /// </summary>
    public abstract class DataMemoryRange : MemoryAddrRange, IMemoryAddrRange
    {
        public DataMemoryRange() { }


        public override MemoryDomain MemoryDomain => MemoryDomain.Data;

    }

    /// <summary>
    /// A Data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : DataMemoryRange, IMemoryRegion, IMemDataRegionAcceptor
    {
        public DataMemoryRegion() { }


        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the appropriate
        /// "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public abstract void Accept(IMemDataRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A memory banked region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryBankedRegion : DataMemoryRegion
    {
        public DataMemoryBankedRegion() { }


        /// <summary>
        /// Used to serialize <see cref="Bank" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "bank", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BankFormatted { get => $"{Bank}"; set => Bank = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the memory bank number.
        /// </summary>
        [XmlIgnore] public int Bank { get; private set; }

    }

    /// <summary>
    /// Adjust byte address pointing in data memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataByteAdjustPoint : MemDataSymbolAcceptorBase, IAdjustPoint
    {
        public DataByteAdjustPoint() { }

        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the relative byte offset to add for adjustment.
        /// </summary>
        [XmlIgnore] public int Offset { get; private set; }


        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} byte(s)";
            return $"Adjust 0x{Offset:X} byte(s)";
        }

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data memory byte address adjustment symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Adjust bit address pointing in data memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataBitAdjustPoint : MemDataSymbolAcceptorBase, IAdjustPoint
    {
        public DataBitAdjustPoint() { }


        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the relative bit offset to add for adjustment.
        /// </summary>
        [XmlIgnore] public int Offset { get; private set; }


        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} bit(s)";
            return $"Adjust 0x{Offset:X} bit(s)";
        }

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data memory bit address adjustment symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// SFR Field semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic : MemDataSymbolAcceptorBase
    {
        public SFRFieldSemantic() { }


        /// <summary>
        /// Gets the textual description of the semantic.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the "when" condition of the semantic.
        /// </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register field semantic symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// SFR bits-field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRFieldDef : MemDataSymbolAcceptorBase
    {
        public SFRFieldDef() { }


        /// <summary>
        /// Gets the list of semantics of this SFR field.
        /// </summary>
        [XmlElement(ElementName = "SFRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the bit width of this SFR field.
        /// </summary>
        [XmlIgnore] public uint NzWidth { get; set; }

        /// <summary>
        /// Used to serialize <see cref="BitPos" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "bitpos", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        /// <summary>
        /// Gets the bit position/address (zero-based) of this SFR field.
        /// </summary>
        [XmlIgnore] public byte BitPos { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the bit mask of this SFR field.
        /// </summary>
        [XmlIgnore] public uint Mask { get; set; }

        /// <summary>
        /// Gets the name of this SFR Field.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of this SFR Field.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to language tools.
        /// </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden.
        /// </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to MPLAB IDE.
        /// </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register field symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFRBitField={CName} @{BitPos}[{NzWidth}]";
    }

    /// <summary>
    /// SFR Fields definitions for a given mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRMode : MemDataSymbolAcceptorBase
    {
        public SFRMode() { }


        /// <summary>
        /// Gets the list of SFR fields definitions.
        /// </summary>
        [XmlElement(ElementName = "SFRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataBitAdjustPoint))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset value of the mode. Not used.
        /// </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register mode symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFRMode={ID}";
    }

    /// <summary>
    /// List of SFR modes.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRModeList : MemDataSymbolAcceptorBase
    {
        public SFRModeList() { }


        /// <summary>
        /// Gets the list of SFR modes.
        /// </summary>
        [XmlElement(ElementName = "SFRMode", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRMode> SFRModes { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register modes list symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register modes list symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register modes list symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Special Function Register (SFR) definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDef : MemDataSymbolAcceptorBase
    {
        public SFRDef() { }

        /// <summary>
        /// Gets the list of modes for this SFR.
        /// </summary>
        [XmlArray("SFRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRMode", typeof(SFRMode), Form = XmlSchemaForm.None, Namespace = "", IsNullable = false)]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRMode> SFRModes { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the data memory address of this SFR.
        /// </summary>
        [XmlIgnore] public uint Addr { get; private set; }

        /// <summary>
        /// Gets the name of this SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of this SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the bit width of this SFR.
        /// </summary>
        [XmlIgnore] public uint NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of this SFR.
        /// </summary>
        [XmlIgnore] public uint ByteWidth => (NzWidth + 7) >> 3;

        /// <summary>
        /// Used to serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ImplFormatted { get => $"0x{Impl:X}"; set => Impl = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the implemented bits mask of this SFR.
        /// </summary>
        [XmlIgnore] public uint Impl { get; private set; }

        /// <summary>
        /// Gets the access mode bits descriptor for this SFR.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <value>
        /// The access mode bits descriptor as a string.
        /// </value>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary>
        /// Gets the Master Clear (MCLR) bits values (string) of this SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "mclr", Form = XmlSchemaForm.None, Namespace = "")]
        public string MCLR { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset bits values (string) of this SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is indirect.
        /// </summary>
        [XmlAttribute(AttributeName = "isindirect", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIndirect { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIndirectSpecified { get => IsIndirect; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is volatile.
        /// </summary>
        [XmlAttribute(AttributeName = "isvolatile", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsVolatile { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsVolatileSpecified { get => IsVolatile; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden.
        /// </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to language tools.
        /// </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to MPLAB IDE.
        /// </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        [XmlIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        /// <summary>
        /// Gets the name of the peripheral this SFR is the base address of.
        /// </summary>
        [XmlAttribute(AttributeName = "baseofperipheral", Form = XmlSchemaForm.None, Namespace = "")]
        public string BaseOfPeripheral { get; set; }

        /// <summary>
        /// Gets the Non-Memory-Mapped-Register identifier of the SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "nmmrid", Form = XmlSchemaForm.None, Namespace = "")]
        public string NMMRID { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is Non-Memory-Mapped.
        /// </summary>
        [XmlIgnore] public bool IsNMMR { get => !String.IsNullOrEmpty(NMMRID); set { } }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFR '{CName}' @{(IsNMMR ? $"NMMRID({NMMRID})" : $"0x{Addr:X}")}";
    }

    /// <summary>
    /// Mirrored registers area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class Mirror : MemDataSymbolAcceptorBase
    {
        public Mirror() { }


        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the memory address of the mirror
        /// </summary>
        [XmlIgnore] public int Addr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzSizeFormatted { get => $"0x{NzSize:X}"; set => NzSize = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the size in bytes of the mirrored area.
        /// </summary>
        [XmlIgnore] public int NzSize { get; private set; }

        /// <summary>
        /// Gets the region identifier reference of the mirrored memory region.
        /// </summary>
        [XmlAttribute(AttributeName = "regionidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionIDRef { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this mirrored data memory symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this mirrored data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this mirrored data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Mirror '{RegionIDRef}' @0x{Addr:X}[{NzSize}]";
    }

    /// <summary>
    /// Joined SFR (e.g. FSR2 register composed of FSR2H:FSR2L registers).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class JoinedSFRDef : MemDataSymbolAcceptorBase
    {
        public JoinedSFRDef() { }


        /// <summary>
        /// Gets the list of adjacent SFRs composing the join.
        /// </summary>
        [XmlElement("SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRDef> SFRs { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the memory address of the joined SFRs.
        /// </summary>
        [XmlIgnore] public uint Addr { get; private set; }

        /// <summary>
        /// Gets the name of the joined SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the textual description of the joined SFR.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the bit width of the joined SFR.
        /// </summary>
        [XmlIgnore] public uint NzWidth { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this joined special function registers symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this joined special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this joined special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Joined SFR '{CName}' @0x{Addr:X}[{NzWidth}]";
    }

    /// <summary>
    /// Selection of a SFR.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SelectSFR : MemDataSymbolAcceptorBase
    {
        public SelectSFR() { }


        /// <summary>
        /// Gets the SFR being selected.
        /// </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public SFRDef SFR { get; set; }

        /// <summary>
        /// Gets the (optional) "when" condition for selection.
        /// </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register selection symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register selection symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register selection symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Select '{SFR.CName}' when '{When}'";

    }

    /// <summary>
    /// Multiplexed SFRs definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class MuxedSFRDef : MemDataSymbolAcceptorBase
    {
        public MuxedSFRDef() { }


        /// <summary>
        /// Gets the list of selections of SFRs.
        /// </summary>
        [XmlElement(ElementName = "SelectSFR", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SelectSFR> SelectSFRs { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the memory address of the multiplexed SFRs.
        /// </summary>
        [XmlIgnore] public int Addr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bit width of the multiplex.
        /// </summary>
        [XmlIgnore] public int NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of the multiplex.
        /// </summary>
        [XmlIgnore] public int ByteWidth => (NzWidth + 7) >> 3;


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this multiplexed special function registers symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this multiplexed special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this multiplexed special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Muxed SFR @0x{Addr:X}[{NzWidth}]";

    }

    /// <summary>
    /// DMA Register mirror.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DMARegisterMirror : MemDataSymbolAcceptorBase
    {
        public DMARegisterMirror() { }


        /// <summary>
        /// Gets the name reference.
        /// </summary>
        [XmlAttribute(AttributeName = "cnameref", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameRef { get; set; }

        /// <summary>
        /// Gets the name suffix.
        /// </summary>
        [XmlAttribute(AttributeName = "cnamesuffix", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameSuffix { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this Direct Memory Access register symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this Direct Memory Access register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this Direct Memory Access register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Special Function Registers data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDataSector : DataMemoryBankedRegion
    {
        public SFRDataSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.SFR;

        /// <summary>
        /// Gets the list of SFRs (simple, joined, multiplexed, mirrored, DMA, adjusted) defined in this memory region.
        /// </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        [XmlElement(ElementName = "JoinedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(JoinedSFRDef))]
        [XmlElement(ElementName = "MuxedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(MuxedSFRDef))]
        [XmlElement(ElementName = "Mirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(Mirror))]
        [XmlElement(ElementName = "RegisterMirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DMARegisterMirror))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<object> SFRs { get; set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this special function registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this special function registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this special function registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// General Purpose Registers (GPR) data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class GPRDataSector : DataMemoryBankedRegion
    {
        public GPRDataSector() { }

 
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.GPR;

       /// <summary>
        /// Gets the shadow identifier reference, if any
        /// </summary>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ShadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the shadow memory address offset.
        /// </summary>
        [XmlIgnore] public int ShadowOffset { get; private set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this general purpose registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this general purpose registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this general purpose registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"GPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// Dual Port Registers data memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DPRDataSector : DataMemoryBankedRegion
    {
        public DPRDataSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DPR;

        /// <summary>
        /// Gets the list of SFRs as Dual Port registers.
        /// </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<object> SFRs { get; set; }

        /// <summary>
        /// Gets the shadowed memory region identifier reference.
        /// </summary>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ShadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the shadow memory offset.
        /// </summary>
        [XmlIgnore] public int ShadowOffset { get; private set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this dual port registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this dual port registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this dual port registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"DPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// Emulator memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class EmulatorZone : DataMemoryRegion
    {
        public EmulatorZone() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Emulator;

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this emulator zone data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this emulator zone data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this emulator zone data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Emulator zone";

    }

    /// <summary>
    /// Non-Memory-Mapped-Register (NMMR) definitions.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class NMMRPlace : MemDataRegionAcceptorBase
    {
        public NMMRPlace() { }


        /// <summary>
        /// Gets the list of SFR definitions.
        /// </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRDef> SFRDefs { get; set; }

        /// <summary>
        /// Gets the identifier of the NMMR region.
        /// </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"NMMR '{RegionID}'";

    }

    /// <summary>
    /// Linear data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class LinearDataSector : DataMemoryRange, IMemDataRegionAcceptor
    {
        public LinearDataSector() { }


        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Linear;

        /// <summary>
        /// Used to serialize <see cref="BankSize" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "banksize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BankSizeFormatted { get => $"0x{BankSize:X}"; set => BankSize = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the bytes size of the linear memory bank.
        /// </summary>
        [XmlIgnore] public int BankSize { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="BlockBeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "blockbeginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BlockBeginAddrFormatted { get => $"0x{BlockBeginAddr:X}"; set => BlockBeginAddr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the beginning address of the linear memory bank.
        /// </summary>
        [XmlIgnore] public uint BlockBeginAddr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="BlockEndAddr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "blockendaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BlockEndAddrFormatted { get => $"0x{BlockEndAddr:X}"; set => BlockEndAddr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the ending address (+1) of the linear memory bank.
        /// </summary>
        [XmlIgnore] public uint BlockEndAddr { get; private set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this linear access data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this linear access data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this linear access data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Linear Data sector [0x{BlockBeginAddr:X}-0x{BlockEndAddr:X}]";

    }

    /// <summary>
    /// Data memory regions regardless of PIC execution mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RegardlessOfMode
    {
        public RegardlessOfMode() { }


        /// <summary>
        /// Gets the list of data memory regions.
        /// </summary>
        [XmlElement(ElementName = "SFRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDataSector))]
        [XmlElement(ElementName = "DPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DPRDataSector))]
        [XmlElement(ElementName = "GPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(GPRDataSector))]
        [XmlElement(ElementName = "EmulatorZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EmulatorZone))]
        [XmlElement(ElementName = "NMMRPlace", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(NMMRPlace))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<object> Regions { get; set; }

    }

    /// <summary>
    /// Data memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DataSpace
    {
        public DataSpace() { }


        /// <summary>
        /// Gets the data memory regions regardless of PIC execution mode.
        /// </summary>
        [XmlElement(ElementName = "RegardlessOfMode", Form = XmlSchemaForm.None, Namespace = "")]
        public RegardlessOfMode RegardlessOfMode { get; set; }

        /// <summary>
        /// Gets the list of GPR data memory regions when PIC is in traditional execution mode.
        /// </summary>
        [XmlArray("TraditionalModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> TraditionalModeOnly { get; set; }

        /// <summary>
        /// Gets the list of GPR data memory regions when PIC is in extended execution mode.
        /// </summary>
        [XmlArray("ExtendedModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> ExtendedModeOnly { get; set; }

        /// <summary>
        /// Used to serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "endaddr", Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string EndAddrFormatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }

        /// <summary>
        /// Gets the highest (end) address (+1) of the data memory space.
        /// </summary>
        [XmlIgnore] public uint EndAddr { get; private set; }

        private string _debugDisplay
            => $"Data Space [0x0-0x{EndAddr:X}]";

    }

    #endregion


    /// <summary>
    /// PIC definition. (Version 1)
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(ElementName ="PIC", Namespace = "", IsNullable = false)]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class PIC_v1 : IPIC
    {

        // Maps the 'InstructionsetID' identifiers to internal code.
        private readonly static Dictionary<string, InstructionSetID> mapInstrID = new Dictionary<string, InstructionSetID>() {
                { "pic16f77", InstructionSetID.PIC16 },
                { "cpu_mid_v10", InstructionSetID.PIC16_ENHANCED },
                { "cpu_p16f1_v1", InstructionSetID.PIC16_FULLFEATURED },
                { "egg", InstructionSetID.PIC18_EXTENDED },
                { "pic18", InstructionSetID.PIC18 },
                { "cpu_pic18f_v6", InstructionSetID.PIC18_ENHANCED }
            };

        public PIC_v1() { }


        /// <summary>
        /// Gets the architecture definition from the XML.
        /// </summary>
        [XmlElement(ElementName = "ArchDef", Form = XmlSchemaForm.None, Namespace = "")]
        public ArchDef ArchDef { get; set; }

        /// <summary>
        /// Gets the instruction set identifier from the XML.
        /// </summary>
        [XmlElement(ElementName = "InstructionSet", Form = XmlSchemaForm.None, Namespace = "")]
        public InstructionSet InstructionSet { get; set; }

        /// <summary>
        /// Gets a list of interrupts (IRQ) from the XML.
        /// </summary>
        [XmlArray("InterruptList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("Interrupt", typeof(Interrupt), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary>
        /// Gets the program memory space definitions from the XML.
        /// </summary>
        [XmlElement(ElementName = "ProgramSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary>
        /// Gets the data memory space definitions from the XML.
        /// </summary>
        [XmlElement(ElementName = "DataSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public DataSpace DataSpace { get; set; }

        /// <summary>
        /// Gets the list of SFRs in the DMA space from the XML.
        /// </summary>
        [XmlArray("DMASpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRDataSector", typeof(SFRDataSector), IsNullable = false, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary>
        /// Gets the list of linear data memory regions in the indirect space from the XML.
        /// </summary>
        [XmlArray("IndirectSpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("LinearDataSector", typeof(LinearDataSector), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<LinearDataSector> IndirectSpace { get; set; }

        /// <summary>
        /// Gets the PIC name from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx) from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "arch", Form = XmlSchemaForm.None, Namespace = "")]
        public string Arch { get; set; }

        /// <summary>
        /// Gets the PIC description from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ProcID" /> property from/to hexadecimal string.
        /// </summary>
        [XmlAttribute(AttributeName = "procid", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ProcIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the unique processor identifier from the XML. Used by development tools.
        /// </summary>
        [XmlIgnore] public int ProcID { get; private set; }

        /// <summary>
        /// Gets the data sheet identifier of the PIC from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "dsid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DsID { get; set; }

        [XmlAttribute(AttributeName = "dosid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DosID { get; set; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// Overridden by the data space definition containing non-empty extended-mode-only memory space.
        /// </summary>
        [XmlAttribute(AttributeName = "isextended", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsExtended { get => DataSpace?.ExtendedModeOnly?.Count > 0; set { } }

        /// <summary>
        /// Gets a value indicating whether this PIC supports freezing of peripherals from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "hasfreeze", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool HasFreeze { get; set; }

        /// <summary>
        /// Gets a value indicating whether this PIC supports debugging from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "isdebuggable", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsDebuggable { get; set; }

        [XmlAttribute(AttributeName = "informedby", Form = XmlSchemaForm.None, Namespace = "")]
        public string Informedby { get; set; }

        [XmlAttribute(AttributeName = "masksetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string MaskSetID { get; set; }

        [XmlAttribute(AttributeName = "psid", Form = XmlSchemaForm.None, Namespace = "")]
        public string PsID { get; set; }

        /// <summary>
        /// Gets the name of the PIC, this PIC is the clone of, from the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "clonedfrom", Form = XmlSchemaForm.None, Namespace = "")]
        public string ClonedFrom { get; set; }

        /// <summary>
        /// Gets the instruction set identifier of this PIC as a value from the <see cref="InstructionSetID"/> enumeration.
        /// </summary>
        [XmlIgnore]
        public InstructionSetID GetInstructionSetID
        {
            get
            {
                if (InstructionSet == null)
                    return InstructionSetID.UNDEFINED;
                if (!mapInstrID.TryGetValue(InstructionSet.ID, out InstructionSetID id))
                {
                    id = InstructionSetID.UNDEFINED;
                    if (Arch == "16xxxx")
                        id = InstructionSetID.PIC16;
                    if (Arch == "16Exxx")
                        id = InstructionSetID.PIC16_FULLFEATURED;
                    if (Arch == "18xxxx")
                        id = (IsExtended ? InstructionSetID.PIC18_EXTENDED : InstructionSetID.PIC18);
                }
                return id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this PIC is belonging to the PIC18 family.
        /// </summary>
        [XmlIgnore] public bool IsPIC18 => GetInstructionSetID >= InstructionSetID.PIC18;

        [XmlIgnore] public IArchDef ArchDefinitions => ArchDef;

        [XmlIgnore] public string InstructionSetFamily => InstructionSet.ID;

        private string _debugDisplay
            => $"PIC (v1) '{Name}' ({Arch}, {GetInstructionSetID}) ";

    }

}


