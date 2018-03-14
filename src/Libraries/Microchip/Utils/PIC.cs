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
//
namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;

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
    public abstract class MemoryAddrRange :
        IEquatable<MemoryAddrRange>, IEqualityComparer<MemoryAddrRange>,
        IComparer<MemoryAddrRange>, IComparable<MemoryAddrRange>

    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemoryAddrRange() { }

        #endregion

        #region Properties

        [XmlIgnore]
        public abstract MemoryDomain MemoryDomain { get; }

        [XmlIgnore]
        public abstract MemorySubDomain MemorySubDomain { get; }

        /// <summary>
        /// Used to serialize <see cref="BeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The begin address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "beginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _beginaddrformatted { get => $"0x{BeginAddr:X}";  set => BeginAddr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the begin address of the memory range.
        /// </summary>
        /// <value>
        /// The begin address as an integer.
        /// </value>
        [XmlIgnore]
        public uint BeginAddr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The end address as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "endaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _endaddrformatted { get => $"0x{EndAddr:X}";  set => EndAddr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the end address of the memory range.
        /// </summary>
        /// <value>
        /// The end address as an integer.
        /// </value>
        [XmlIgnore]
        public uint EndAddr { get; private set; }

        #endregion

        #region Implementation of the IEquatable<MemoryAddrRange>, IEqualityComparer<MemoryAddrRange>, IComparer<MemoryAddrRange>, IComparable<MemoryAddrRange> interfaces

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

        public override bool Equals(object obj) => Equals(obj as MemoryAddrRange);

        public override int GetHashCode() => (BeginAddr.GetHashCode() + EndAddr.GetHashCode()) ^ MemoryDomain.GetHashCode();

        public static bool operator ==(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) == 0;

        public static bool operator !=(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) != 0;

        public int Compare(MemoryAddrRange x, MemoryAddrRange y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            return x?.CompareTo(y) ?? -1;
        }

        private static int _Compare(MemoryAddrRange x, MemoryAddrRange y)
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
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DEVIDToRev()
        {
        }

        #endregion

        #region Properties

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
        [XmlIgnore]
        public int Value { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); } 

        #endregion

    }

    /// <summary>
    /// Memory trait (characteristics).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemTrait : IMemTraitsSymbolAcceptor
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemTrait() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of the memory word.
        /// </summary>
        /// <value>
        /// The size of the memory word in bytes.
        /// </value>
        [XmlIgnore]
        public virtual uint WordSize { get; private set; }

        /// <summary>
        /// Gets the memory location access size.
        /// </summary>
        /// <value>
        /// The size of the location in bytes.
        /// </value>
        [XmlIgnore]
        public virtual uint LocSize { get; private set; }

        /// <summary>
        /// Gets the memory word implementation (bit mask).
        /// </summary>
        /// <value>
        /// The memory word implementation bit mask.
        /// </value>
        [XmlIgnore]
        public virtual uint WordImpl { get; private set; }

        /// <summary>
        /// Gets the initial (erased) memory word value.
        /// </summary>
        /// <value>
        /// The word initialize.
        /// </value>
        [XmlIgnore]
        public virtual uint WordInit { get; private set; }

        /// <summary>
        /// Gets the memory word safe value.
        /// </summary>
        /// <value>
        /// The memory word safe value.
        /// </value>
        [XmlIgnore]
        public virtual uint WordSafe { get; private set; }

        #endregion

        #region XML conversion helpers

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

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public abstract void Accept(IMemTraitsSymbolVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemTraitsSymbolVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A default memory trait.
    /// </summary>
    public sealed class DefaultMemTrait : MemTrait
    {

        #region Properties

        /// <summary>
        /// Gets the default size of the memory word.
        /// </summary>
        /// <value>
        /// The size of the memory word in bytes.
        /// </value>
        [XmlIgnore]
        public override uint WordSize => 1;

        /// <summary>
        /// Gets the default memory location access size.
        /// </summary>
        /// <value>
        /// The size of the location in bytes.
        /// </value>
        [XmlIgnore]
        public override uint LocSize => 1;

        /// <summary>
        /// Gets the default memory word implementation (bit mask).
        /// </summary>
        /// <value>
        /// The memory word implementation bit mask.
        /// </value>
        [XmlIgnore]
        public override uint WordImpl => 0xFF;

        /// <summary>
        /// Gets the default initial (erased) memory word value.
        /// </summary>
        /// <value>
        /// The word initialize.
        /// </value>
        [XmlIgnore]
        public override uint WordInit => 0xFF;

        /// <summary>
        /// Gets the default memory word safe value.
        /// </summary>
        /// <value>
        /// The memory word safe value.
        /// </value>
        [XmlIgnore]
        public override uint WordSafe => 0x00;

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this default memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this code memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this code memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context)
        {
            throw new NotImplementedException();
        }


        #endregion

    }

    /// <summary>
    /// Code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CodeMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this code memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this code memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this code memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// External code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtCodeMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this external code memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this external code memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this external code memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Calibration data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalDataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this calibration memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this calibration memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this calibration memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Background debug memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BackgroundDebugMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BackgroundDebugMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this debugger memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this debugger memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this debugger memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Test memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this test memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this test memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this test memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// User IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserIDMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this User ID memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this User ID memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this User ID memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Configuration fuses memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigFuseMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this configuration fuses memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this configuration fuses memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this configuration fuses memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        #region Properties

        [XmlIgnore]
        public int UnimplVal { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); } 

        #endregion

    }

    /// <summary>
    /// Configuration Write-Once-Read-Many memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigWORMMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigWORMMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this configuration words memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this configuration words memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this configuration words memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceIDMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this Device ID memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this Device ID memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this Device ID memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// EEPROM data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EEDataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this Data EEPROM memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this Data EEPROM memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this Data EEPROM memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        #region Properties

        public override uint LocSize => 1;
        public override uint WordSize => 1;
        public override uint WordImpl => 0xFF;
        public override uint WordInit => 0xFF;
        public override uint WordSafe => 0xFF;

        /// <summary>
        /// Gets address magic offset in the binary image for EEPROM content.
        /// </summary>
        /// <value>
        /// The offset address as an integer.
        /// </value>
        [XmlIgnore]
        public uint MagicOffset { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "magicoffset", Form = XmlSchemaForm.None, Namespace = "")]
        public string MagicOffsetFormatted { get => $"0x{MagicOffset:X}"; set => MagicOffset = value.ToUInt32Ex(); } 

        #endregion

    }

    /// <summary>
    /// Data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataMemTraits : MemTrait
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interfaces 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this data memory trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public override void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this data memory trait.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemTraitsSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a memory trait visitor and calls the appropriate
        /// "Visit()" function for this data memory trait with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemTraitsSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// The various memory regions' traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MemTraits
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemTraits() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the memory traits of the various memory regions.
        /// </summary>
        /// <value>
        /// The memory traits.
        /// </value>
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
        /// <value>
        /// The depth of the hardware stack as an integer.
        /// </value>
        [XmlIgnore]
        public int HWStackDepth { get; private set; }

        /// <summary>
        /// Gets the number of memory banks.
        /// </summary>
        /// <value>
        /// The number of banks as an integer.
        /// </value>
        [XmlIgnore]
        public int BankCount { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "hwstackdepth", Form = XmlSchemaForm.None, Namespace = "")]
        public string HWStackDepthFormatted { get => $"{HWStackDepth}"; set => HWStackDepth = value.ToInt32Ex(); } 

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "bankcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string BankCountFormatted { get => $"{BankCount}"; set => BankCount = value.ToInt32Ex(); } 

        #endregion

    }

    /// <summary>
    /// PIC memory architecture definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArchDef() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the memory traits of the PIC.
        /// </summary>
        /// <value>
        /// The memory traits.
        /// </value>
        [XmlElement(ElementName = "MemTraits", Form = XmlSchemaForm.None, Namespace = "")]
        public MemTraits MemTraits { get; set; }

        /// <summary>
        /// Gets the description of the PIC architecture.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture.
        /// </summary>
        /// <value>
        /// The name of architecture (16xxxx, 16Exxx, 18xxxx) as a string
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        #endregion

    }

    public static class PICArch
    {
        private static Dictionary<MemorySubDomain, MemoryDomain> subdom2dom = new Dictionary<MemorySubDomain, MemoryDomain>
        {
            { MemorySubDomain.SFR,      MemoryDomain.Data },
            { MemorySubDomain.GPR,      MemoryDomain.Data },
            { MemorySubDomain.DPR,      MemoryDomain.Data },
            { MemorySubDomain.NNMR,     MemoryDomain.Data },
            { MemorySubDomain.Emulator, MemoryDomain.Data },
            { MemorySubDomain.Linear,   MemoryDomain.Data },
            { MemorySubDomain.DMA,      MemoryDomain.Data },
            { MemorySubDomain.Code,             MemoryDomain.Prog },
            { MemorySubDomain.ExtCode,          MemoryDomain.Prog },
            { MemorySubDomain.EEData,           MemoryDomain.Prog },
            { MemorySubDomain.DeviceConfig,     MemoryDomain.Prog },
            { MemorySubDomain.DeviceConfigInfo, MemoryDomain.Prog },
            { MemorySubDomain.DeviceInfoAry,    MemoryDomain.Prog },
            { MemorySubDomain.UserID,           MemoryDomain.Prog },
            { MemorySubDomain.DeviceID,         MemoryDomain.Prog },
            { MemorySubDomain.RevisionID,       MemoryDomain.Prog },
            { MemorySubDomain.Debugger,         MemoryDomain.Prog },
            { MemorySubDomain.Calib,            MemoryDomain.Prog },
            { MemorySubDomain.Test,             MemoryDomain.Prog },
        };

        public static MemoryDomain GetDomain(MemorySubDomain subdom)
        {
            if (subdom2dom.TryGetValue(subdom, out MemoryDomain dom))
                return dom;
            return MemoryDomain.Unknown;
        }
    }

    #endregion

    #region InstructionSet definition

    public sealed class InstructionSet
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstructionSet() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instruction set ID.
        /// </summary>
        /// <value>
        /// The ID as a string.
        /// </value>
        [XmlAttribute(AttributeName = "instructionsetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        #endregion

    }

    #endregion

    #region InterruptList XML element

    /// <summary>
    /// Interrupt request.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Interrupt
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Interrupt() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="IRQ" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The IRQ content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "irq", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string IRQFormatted { get => $"{IRQ}"; set => IRQ = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the IRQ number.
        /// </summary>
        /// <value>
        /// The IRQ number as an integer.
        /// </value>
        [XmlIgnore]
        public uint IRQ { get; private set; }

        /// <summary>
        /// Gets the name of the interrupt request.
        /// </summary>
        /// <value>
        /// The interrupt request name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the interrupt request.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        #endregion

    }

    #endregion

    #region ProgramSpace XML element

    public abstract class MemProgramSymbolAcceptor : IMemProgramSymbolAcceptor
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
    public abstract class ProgMemoryRange : MemoryAddrRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemoryRange() { }

        #endregion

        public override MemoryDomain MemoryDomain => MemoryDomain.Prog;

    }

    /// <summary>
    /// A Program memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : ProgMemoryRange, IMemProgramRegionAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemoryRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

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
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemorySection() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this memory region is a section.
        /// </summary>
        /// <value>
        /// True if this memory region is a section, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsSection { get; set; }

        /// <summary>
        /// Gets the section description.
        /// </summary>
        /// <value>
        /// Information describing the section.
        /// </value>
        [XmlAttribute(AttributeName = "sectiondesc", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionDesc { get; set; }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section as a string.
        /// </value>
        [XmlAttribute(AttributeName = "sectionname", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionName { get; set; }

        #endregion

    }

    /// <summary>
    /// Adjust byte address pointing in program memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgByteAdjustPoint : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgByteAdjustPoint() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the relative byte offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

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

    }

    /// <summary>
    /// Adjust bit address pointing in program memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgBitAdjustPoint : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgBitAdjustPoint() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the relative bit offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

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

    }

    /// <summary>
    /// Code memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Code;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CodeSector() { }

        #endregion

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
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Calib;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalDataZone() { }

        #endregion

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
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Test;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestZone() { }

        #endregion

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
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.UserID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserIDSector() { }

        #endregion

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
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.RevisionID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RevisionIDSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the device ID to silicon revision relationship.
        /// </summary>
        /// <value>
        /// The device IDs to revision levels.
        /// </value>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        #endregion

    }

    /// <summary>
    /// Device IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceIDSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the Device IDs to silicon revision level.
        /// </summary>
        /// <value>
        /// The Device IDs
        /// </value>
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
        /// Gets the mask to isolate device ID.
        /// </summary>
        /// <value>
        /// The mask.
        /// </value>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Value"/> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The value content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the generic value of device ID
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [XmlIgnore]
        public int Value { get; private set; }

        #endregion

    }

    /// <summary>
    /// Device Configuration Register field pattern semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRFieldSemantic() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name as a string
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the field value.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the when condition for the field value.
        /// </summary>
        /// <value>
        /// The when expression as a string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets a value indicating whether this configuration pattern is hidden.
        /// </summary>
        /// <value>
        /// True if this configuration is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets or sets a value indicating whether this configuration pattern is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this configuration is language hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Used to serialize <see cref="OscModeIDRef" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The oscillator mode id reference content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "oscmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OscModeIDRefFormatted { get => $"{OscModeIDRef}"; set => OscModeIDRef = value.ToInt32Ex(); } 
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool OscModeIDRefFormattedSpecified { get => OscModeIDRef != 0; set { } }

        /// <summary>
        /// Gets the oscillator mode identifier reference.
        /// </summary>
        /// <value>
        /// The oscillator mode identifier reference as an integer.
        /// </value>
        [XmlIgnore]
        public int OscModeIDRef { get; private set; }

        [XmlAttribute(AttributeName = "_defeatcoercion", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefeatCoercion { get; set; }

        /// <summary>
        /// Gets the memory mode identifier reference.
        /// </summary>
        /// <value>
        /// The memory mode identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "memmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string MemModeIDRef { get; set; }

        #endregion

    }

    /// <summary>
    /// Device Configuration Register Field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRFieldDef() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the configuration field semantics for various configuration values.
        /// </summary>
        /// <value>
        /// The field semantics.
        /// </value>
        [XmlElement(ElementName = "DCRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The field name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the field.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the bit position of the field. (Not defined by Microchip).
        /// </summary>
        /// <remarks>
        /// This value must be set by consumer.
        /// </remarks>
        [XmlIgnore]
        public int BitAddr { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of the field.
        /// </summary>
        /// <value>
        /// The width in number of bits.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit mask of the field in the register.
        /// </summary>
        /// <value>
        /// The bit mask as an integer.
        /// </value>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to the MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        #endregion

    }

    /// <summary>
    /// Device Configuration Register mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRMode : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRMode() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the fields of the configuration register.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        [XmlElement(ElementName = "DCRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgBitAdjustPoint))]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        #endregion

    }

    /// <summary>
    /// Device Configuration Register illegal definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRDefIllegal() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the when pattern of the illegal condition.
        /// </summary>
        /// <value>
        /// The when string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets the description of the illegal condition.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        #endregion

    }

    /// <summary>
    /// Device Configuration Register definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef : MemProgramSymbolAcceptor
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRDef() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the illegal configuration values (if any).
        /// </summary>
        /// <value>
        /// The illegal configuration values.
        /// </value>
        [XmlElement(ElementName = "Illegal", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRDefIllegal> Illegals { get; set; }

        /// <summary>
        /// Gets a list of Device Configuration Register modes.
        /// </summary>
        /// <value>
        /// A list of modes.
        /// </value>
        [XmlArray(ElementName = "DCRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem(ElementName = "DCRMode", Type = typeof(DCRMode), Form = XmlSchemaForm.None, IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the memory address of the configuration register
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the configuration register.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the configuration register.
        /// </summary>
        /// <value>
        /// The description as a string
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of the register.
        /// </summary>
        /// <value>
        /// The bit width of the register.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The implementation mask as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ImplFormatted { get => $"0x{Impl:X}"; set => Impl = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the implemented bits mask.
        /// </summary>
        /// <value>
        /// The implementation mask as an integer.
        /// </value>
        [XmlIgnore]
        public int Impl { get; private set; }

        /// <summary>
        /// Gets the access of the register's bits.
        /// </summary>
        /// <value>
        /// The access bits as a string.
        /// </value>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Default" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The default value content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "default", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string DefaultFormatted { get => $"0x{Default:X}"; set => Default = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the default value of the register.
        /// </summary>
        /// <value>
        /// The default value as an integer.
        /// </value>
        [XmlIgnore]
        public int Default { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="FactoryDefault" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The factory default content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "factorydefault", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string FactoryDefaultFormatted { get => $"0x{FactoryDefault:X}"; set => FactoryDefault = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the factory default value of the register.
        /// </summary>
        /// <value>
        /// The factory default value as an integer.
        /// </value>
        [XmlIgnore]
        public int FactoryDefault { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this register is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this register is language hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Used to serialize <see cref="UnimplVal" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The unimplemented value content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); } 

        [XmlIgnore]
        public int UnimplVal { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Unused" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The unused content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "unused", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UnusedFormatted { get => $"{Unused}"; set => Unused = value.ToInt32Ex(); } 

        [XmlIgnore]
        public int Unused { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="UseInChecksum" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The use-in-checksum content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "useinchecksum", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string UseInChecksumFormatted { get => $"0x{UseInChecksum:X}"; set => UseInChecksum = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit mask to use in checksum computation.
        /// </summary>
        /// <value>
        /// The bit mask as an integer.
        /// </value>
        [XmlIgnore]
        public int UseInChecksum { get; private set; }

        #endregion

    }

    /// <summary>
    /// Configuration Fuses memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceConfig;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigFuseSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the configuration registers definitions.
        /// </summary>
        /// <value>
        /// The definitions.
        /// </value>
        [XmlElement(ElementName = "DCRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Defs { get; set; }

        #endregion

    }

    /// <summary>
    /// Background debugger vector memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Debugger;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BACKBUGVectorSector() { }

        #endregion

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
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.EEData;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EEDataSector() { }

        #endregion

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
    public sealed class DeviceRegister : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceRegister() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the address of the register.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the register.
        /// </summary>
        /// <value>
        /// The register name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of the register.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        #endregion

    }

    /// <summary>
    /// Device Information Area register array.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray : MemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DIARegisterArray() { }

        #endregion

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

        #region Properties

        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceRegister))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Registers { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the starting memory address of the DIA registers.
        /// </summary>
        /// <value>
        /// The memory address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the DIA registers array.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        #endregion

    }

    /// <summary>
    /// Device Information Area memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceInfoAry;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DIASector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the register arrays.
        /// </summary>
        /// <value>
        /// The register arrays.
        /// </value>
        [XmlElement(ElementName = "RegisterArray", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DIARegisterArray> RegisterArrays { get; set; }

        #endregion

    }

    /// <summary>
    /// Device Configuration Information memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceConfigInfo;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCISector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the configuration information registers.
        /// </summary>
        /// <value>
        /// The registers.
        /// </value>
        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DeviceRegister> Registers { get; set; }

        #endregion

    }

    /// <summary>
    /// External Code memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.ExtCode;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtCodeSector() { }

        #endregion

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
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VectorArea() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The byte size content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzsizeformatted { get => $"0x{NzSize:X}"; set => NzSize = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bytes size of the area.
        /// </summary>
        /// <value>
        /// The size in bytes as an integer.
        /// </value>
        [XmlIgnore]
        public int NzSize { get; private set; }

        /// <summary>
        /// Gets the identifier of the vector area.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// Program memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgramSpace() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the program memory sectors.
        /// </summary>
        /// <value>
        /// The various sectors.
        /// </value>
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
        /// Gets the interrupt vector area.
        /// </summary>
        /// <value>
        /// The vector area.
        /// </value>
        [XmlElement(ElementName = "VectorArea", Form = XmlSchemaForm.None, Namespace = "")]
        public List<VectorArea> VectorArea { get; set; }

        #endregion

    }

    #endregion

    #region DataSpace XML element

    public abstract class MemDataSymbolAcceptor : IMemDataSymbolAcceptor
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

    public abstract class MemDataRegionAcceptor : IMemDataRegionAcceptor
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
    public abstract class DataMemoryRange : MemoryAddrRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryRange() { }

        #endregion

        public override MemoryDomain MemoryDomain => MemoryDomain.Data;

    }

    /// <summary>
    /// A Data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : DataMemoryRange, IMemDataRegionAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

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
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryBankedRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Bank" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bank number content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "bank", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BankFormatted { get => $"{Bank}"; set => Bank = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the memory bank number.
        /// </summary>
        /// <value>
        /// The bank number as an integer.
        /// </value>
        [XmlIgnore]
        public int Bank { get; private set; }

        #endregion

    }

    /// <summary>
    /// Adjust byte address pointing in data memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataByteAdjustPoint : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataByteAdjustPoint() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the relative byte offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} byte(s)";
            return $"Adjust 0x{Offset:X} byte(s)";
        }

    }

    /// <summary>
    /// Adjust bit address pointing in data memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataBitAdjustPoint : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBitAdjustPoint() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the relative bit offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        public override string ToString()
        {
            if (Offset < 10)
                return $"Adjust {Offset} bit(s)";
            return $"Adjust 0x{Offset:X} bit(s)";
        }

    }

    /// <summary>
    /// SFR Field semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRFieldSemantic() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the semantic description.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the condition of the semantic.
        /// </summary>
        /// <value>
        /// The expression associated with the semantic.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        #endregion

    }

    /// <summary>
    /// SFR bits-field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("SFRField = {Name}")]
    public sealed class SFRFieldDef : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRFieldDef() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the various semantics of this SFR field.
        /// </summary>
        /// <value>
        /// The SFR field semantics.
        /// </value>
        [XmlElement(ElementName = "SFRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        /// <summary>
        /// Gets the name of this SFR Field.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of this SFR Field.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of this SFR field.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public uint NzWidth { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the bit mask of this SFR field.
        /// </summary>
        /// <value>
        /// The bit mask as an integer.
        /// </value>
        [XmlIgnore]
        public uint Mask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        #endregion

    }

    /// <summary>
    /// SFR Fields definitions for a given mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("SFRMode = {ID}")]
    public sealed class SFRMode : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRMode() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the SFR fields definitions.
        /// </summary>
        /// <value>
        /// The SFR fields or adjust points.
        /// </value>
        [XmlElement(ElementName = "SFRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataBitAdjustPoint))]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset value of the mode. Not used.
        /// </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        #endregion

    }

    /// <summary>
    /// List of SFR modes.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRModeList : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRModeList() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the SFR modes.
        /// </summary>
        /// <value>
        /// The modes.
        /// </value>
        [XmlElement(ElementName = "SFRMode", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRMode> SFRModes { get; set; }

        #endregion

    }

    /// <summary>
    /// Special Function Register (SFR) definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("SFR = {Name}")]
    public sealed class SFRDef : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRDef() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets list of modes for this SFR.
        /// </summary>
        /// <value>
        /// A list of modes.
        /// </value>
        [XmlArray("SFRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRMode", typeof(SFRMode), Form = XmlSchemaForm.None, Namespace = "", IsNullable = false)]
        public List<SFRMode> SFRModes { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the data memory address of this SFR.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary>
        /// Gets the name of this SFR.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of this SFR.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of this SFR.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public uint NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of this SFR.
        /// </summary>
        /// <value>
        /// The width of the SFR in number of bytes.
        /// </value>
        [XmlIgnore]
        public uint ByteWidth => (NzWidth + 7) >> 3;

        /// <summary>
        /// Used to serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The implementation mask as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ImplFormatted { get => $"0x{Impl:X}"; set => Impl = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the implemented bits mask of this SFR.
        /// </summary>
        /// <value>
        /// The implementation mask as an integer.
        /// </value>
        [XmlIgnore]
        public uint Impl { get; private set; }

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
        /// Gets the Master Clear (MCLR) bits values of this SFR.
        /// </summary>
        /// <value>
        /// The bits value as a string.
        /// </value>
        [XmlAttribute(AttributeName = "mclr", Form = XmlSchemaForm.None, Namespace = "")]
        public string MCLR { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset bits values of this SFR.
        /// </summary>
        /// <value>
        /// The bits value as a string.
        /// </value>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is indirect.
        /// </summary>
        /// <value>
        /// True if this register is indirect, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isindirect", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIndirect { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIndirectSpecified { get => IsIndirect; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is volatile.
        /// </summary>
        /// <value>
        /// True if this register is volatile, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isvolatile", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsVolatile { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsVolatileSpecified { get => IsVolatile; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        /// <summary>
        /// Gets the name of the peripheral this SFR is the base address of.
        /// </summary>
        /// <value>
        /// The name of the peripheral.
        /// </value>
        [XmlAttribute(AttributeName = "baseofperipheral", Form = XmlSchemaForm.None, Namespace = "")]
        public string BaseOfPeripheral { get; set; }

        /// <summary>
        /// Gets the Non-Memory-Mapped-Register identifier of the SFR.
        /// </summary>
        /// <value>
        /// The identifier as an integer.
        /// </value>
        [XmlAttribute(AttributeName = "nmmrid", Form = XmlSchemaForm.None, Namespace = "")]
        public string NMMRID { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is Non-Memory-Mapped.
        /// </summary>
        [XmlIgnore]
        public bool IsNMMR { get => !String.IsNullOrEmpty(NMMRID); set { } }

        #endregion

    }

    /// <summary>
    /// Mirrored registers area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Mirror : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mirror() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the memory address of the mirror
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The byte size content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzSizeFormatted { get => $"0x{NzSize:X}"; set => NzSize = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the size in bytes of the mirrored area.
        /// </summary>
        /// <value>
        /// The size as an integer.
        /// </value>
        [XmlIgnore]
        public int NzSize { get; private set; }

        /// <summary>
        /// Gets the region identifier reference of the mirrored memory region.
        /// </summary>
        /// <value>
        /// The region identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionIDRef { get; set; }

        #endregion

    }

    /// <summary>
    /// Joined SFR (e.g. FSR2 register composed of FSR2H:FSR2L registers).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class JoinedSFRDef : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JoinedSFRDef()
        {
        }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the individual SFRs composing the join.
        /// </summary>
        /// <value>
        /// The list of SFRDef.
        /// </value>
        [XmlElement("SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRDef> SFRs { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the memory address of the joined SFRs.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>s
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary>
        /// Gets the name of the joined SFRs.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the joined SFRs.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of the joined SFR.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public uint NzWidth { get; private set; }

        #endregion

    }

    /// <summary>
    /// Selection of a SFR.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SelectSFR : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectSFR() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the SFR being selected.
        /// </summary>
        /// <value>
        /// The SFR definition.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public SFRDef SFR { get; set; }

        /// <summary>
        /// Gets the (optional) when condition.
        /// </summary>
        /// <value>
        /// The condition as a string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        #endregion

    }

    /// <summary>
    /// Multiplexed SFRs definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MuxedSFRDef : MemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MuxedSFRDef() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the selections of SFRs.
        /// </summary>
        /// <value>
        /// The selections.
        /// </value>
        [XmlElement(ElementName = "SelectSFR", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SelectSFR> SelectSFRs { get; set; }

        /// <summary>
        /// Used to serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the memory address of the multiplexed SFRs.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bit width content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bit width of the multiplex.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of the multiplex.
        /// </summary>
        /// <value>
        /// The byte width as an integer.
        /// </value>
        [XmlIgnore]
        public int ByteWidth => (NzWidth + 7) >> 3;

        #endregion

    }

    /// <summary>
    /// DMA Register mirror.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DMARegisterMirror : MemDataSymbolAcceptor
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DMARegisterMirror() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the name reference.
        /// </summary>
        /// <value>
        /// The name reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cnameref", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameRef { get; set; }

        /// <summary>
        /// Gets the name suffix.
        /// </summary>
        /// <value>
        /// The name suffix as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cnamesuffix", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameSuffix { get; set; }

        #endregion

    }

    /// <summary>
    /// Special Function Registers data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{MemorySubDomain} sector")]
    public sealed class SFRDataSector : DataMemoryBankedRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.SFR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRDataSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the various SFRs (simple, joined, multiplexed, mirrored, DMA, adjusted) defined in this memory region.
        /// </summary>
        /// <value>
        /// The SFRs, MuxedSFR, JoinedSFR, ... definitions
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        [XmlElement(ElementName = "JoinedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(JoinedSFRDef))]
        [XmlElement(ElementName = "MuxedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(MuxedSFRDef))]
        [XmlElement(ElementName = "Mirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(Mirror))]
        [XmlElement(ElementName = "RegisterMirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DMARegisterMirror))]
        public List<object> SFRs { get; set; }

        #endregion

    }

    /// <summary>
    /// General Purpose Registers (GPR) data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{MemorySubDomain} sector")]
    public sealed class GPRDataSector : DataMemoryBankedRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.GPR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GPRDataSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the shadow identifier reference, if any
        /// </summary>
        /// <value>
        /// The shadow identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The shadow offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ShadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the shadow memory address offset.
        /// </summary>
        /// <value>
        /// The shadow offset as an integer.
        /// </value>
        [XmlIgnore]
        public int ShadowOffset { get; private set; }

        #endregion

    }

    /// <summary>
    /// Dual Port Registers data memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{MemorySubDomain} sector")]
    public sealed class DPRDataSector : DataMemoryBankedRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DPR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DPRDataSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the SFR as Dual Port registers.
        /// </summary>
        /// <value>
        /// The SFRs.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        public List<object> SFRs { get; set; }

        /// <summary>
        /// Gets the shadowed memory region identifier reference.
        /// </summary>
        /// <value>
        /// The shadow identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The shadow offset content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ShadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the shadow memory offset.
        /// </summary>
        /// <value>
        /// The shadow offset as an integer.
        /// </value>
        [XmlIgnore]
        public int ShadowOffset { get; private set; }

        #endregion

    }

    /// <summary>
    /// Emulator memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{MemorySubDomain} sector")]
    public sealed class EmulatorZone : DataMemoryRegion
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Emulator;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EmulatorZone() { }

        #endregion

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

    }

    /// <summary>
    /// Non-Memory-Mapped-Register (NMMR) definitions.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class NMMRPlace : MemDataRegionAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NMMRPlace() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Gets the SFR definitions.
        /// </summary>
        /// <value>
        /// The SFR definitions.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRDef> SFRDefs { get; set; }

        /// <summary>
        /// Gets the identifier of the NMMR region.
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// Linear data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{MemorySubDomain} sector")]
    public sealed class LinearDataSector : DataMemoryRange, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Linear;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinearDataSector() { }

        #endregion

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

        #region Properties

        /// <summary>
        /// Used to serialize <see cref="BankSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bank size content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "banksize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BankSizeFormatted { get => $"0x{BankSize:X}"; set => BankSize = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the bytes size of the linear memory bank.
        /// </summary>
        /// <value>
        /// The size in bytes as an integer.
        /// </value>
        [XmlIgnore]
        public int BankSize { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="BlockBeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The block begin address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "blockbeginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BlockBeginAddrFormatted { get => $"0x{BlockBeginAddr:X}"; set => BlockBeginAddr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the beginning address of the linear memory bank.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public uint BlockBeginAddr { get; private set; }

        /// <summary>
        /// Used to serialize <see cref="BlockEndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The block end address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "blockendaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string BlockEndAddrFormatted { get => $"0x{BlockEndAddr:X}"; set => BlockEndAddr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the ending address of the linear memory bank.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public uint BlockEndAddr { get; private set; }

        #endregion

    }

    /// <summary>
    /// Data memory regions regardless of PIC execution mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RegardlessOfMode
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegardlessOfMode() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data memory regions.
        /// </summary>
        /// <value>
        /// The memory regions.
        /// </value>
        [XmlElement(ElementName = "SFRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDataSector))]
        [XmlElement(ElementName = "DPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DPRDataSector))]
        [XmlElement(ElementName = "GPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(GPRDataSector))]
        [XmlElement(ElementName = "EmulatorZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EmulatorZone))]
        [XmlElement(ElementName = "NMMRPlace", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(NMMRPlace))]
        public List<object> Regions { get; set; }

        #endregion

    }

    /// <summary>
    /// Data memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataSpace
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataSpace() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data memory regions regardless of PIC execution mode.
        /// </summary>
        /// <value>
        /// The memory regions.
        /// </value>
        [XmlElement(ElementName = "RegardlessOfMode", Form = XmlSchemaForm.None, Namespace = "")]
        public RegardlessOfMode RegardlessOfMode { get; set; }

        /// <summary>
        /// Gets the data memory regions when PIC is in traditional execution mode.
        /// </summary>
        /// <value>
        /// The memory regions (GPRs).
        /// </value>
        [XmlArray("TraditionalModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> TraditionalModeOnly { get; set; }

        /// <summary>
        /// Gets the data memory regions when PIC is in extended execution mode.
        /// </summary>
        /// <value>
        /// The memory regions (GPRs).
        /// </value>
        [XmlArray("ExtendedModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false)]
        public List<GPRDataSector> ExtendedModeOnly { get; set; }

        /// <summary>
        /// Used to serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The end address content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "endaddr", Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string EndAddrFormatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); } 

        /// <summary>
        /// Gets the highest (end) address of the data memory space.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public uint EndAddr { get; private set; }

        #endregion

    }

    #endregion

    #region PIC XML definition

    /// <summary>
    /// PIC definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    [DebuggerDisplay("{Name} - {Desc}")]
    public sealed class PIC
    {
        #region Locals

        // Maps the 'InstructionsetID' identifiers to internal code.
        //
        private static Dictionary<string, InstructionSetID> mapInstrID = new Dictionary<string, InstructionSetID>() {
                { "pic16f77", InstructionSetID.PIC16 },
                { "cpu_mid_v10", InstructionSetID.PIC16_ENHANCED },
                { "cpu_p16f1_v1", InstructionSetID.PIC16_FULLFEATURED },
                { "egg", InstructionSetID.PIC18_EXTENDED },
                { "pic18", InstructionSetID.PIC18 },
                { "cpu_pic18f_v6", InstructionSetID.PIC18_ENHANCED }
            };

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the architecture definition.
        /// </summary>
        /// <value>
        /// The architecture definition.
        /// </value>
        [XmlElement(ElementName = "ArchDef", Form = XmlSchemaForm.None, Namespace = "")]
        public ArchDef ArchDef { get; set; }

        /// <summary>
        /// Gets the instruction set identifier.
        /// </summary>
        /// <value>
        /// The instruction set definition.
        /// </value>
        [XmlElement(ElementName = "InstructionSet", Form = XmlSchemaForm.None, Namespace = "")]
        public InstructionSet InstructionSet { get; set; }

        /// <summary>
        /// Gets a list of interrupts (IRQ).
        /// </summary>
        /// <value>
        /// A list of interrupts.
        /// </value>
        [XmlArray("InterruptList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("Interrupt", typeof(Interrupt), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary>
        /// Gets the program memory space.
        /// </summary>
        /// <value>
        /// The program memory space.
        /// </value>
        [XmlElement(ElementName = "ProgramSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary>
        /// Gets the data memory space.
        /// </summary>
        /// <value>
        /// The data memory space.
        /// </value>
        [XmlElement(ElementName = "DataSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public DataSpace DataSpace { get; set; }

        /// <summary>
        /// Gets the SFRs in the DMA space.
        /// </summary>
        /// <value>
        /// The SFRs in DMA space.
        /// </value>
        [XmlArray("DMASpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRDataSector", typeof(SFRDataSector), IsNullable = false, Namespace = "")]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary>
        /// Gets the linear data memory region in the indirect space.
        /// </summary>
        /// <value>
        /// The linear data memory regions.
        /// </value>
        [XmlArray("IndirectSpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("LinearDataSector", typeof(LinearDataSector), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<LinearDataSector> IndirectSpace { get; set; }

        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx)
        /// </summary>
        /// <value>
        /// The architecture as a string.
        /// </value>
        [XmlAttribute(AttributeName = "arch", Form = XmlSchemaForm.None, Namespace = "")]
        public string Arch { get; set; }

        /// <summary>
        /// Gets the PIC description.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to serialize <see cref="ProcID" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The processor id content as an hexadecimal string.
        /// </value>
        [XmlAttribute(AttributeName = "procid", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ProcIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); } 

        /// <summary>
        /// Gets the unique processor identifier.
        /// </summary>
        /// <value>
        /// The identifier as an integer.
        /// </value>
        [XmlIgnore]
        public int ProcID { get; private set; }

        /// <summary>
        /// Gets the data sheet identifier of the PIC.
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "dsid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DsID { get; set; }

        [XmlAttribute(AttributeName = "dosid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DosID { get; set; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// Overridden by the data space definition containing - or not - extended-mode-only memory space.
        /// </summary>
        /// <value>
        /// True if this PIC supports extended execution mode, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isextended", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsExtended { get => DataSpace?.ExtendedModeOnly?.Count > 0; set { } }

        /// <summary>
        /// Gets a value indicating whether this PIC supports freezing of peripherals.
        /// </summary>
        /// <value>
        /// True if this PIC has freeze capabilities, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "hasfreeze", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool HasFreeze { get; set; }

        /// <summary>
        /// Gets a value indicating whether this PIC supports debugging.
        /// </summary>
        /// <value>
        /// True if this PIC supports debugging, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isdebuggable", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsDebuggable { get; set; }

        [XmlAttribute(AttributeName = "informedby", Form = XmlSchemaForm.None, Namespace = "")]
        public string Informedby { get; set; }

        [XmlAttribute(AttributeName = "masksetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string MaskSetID { get; set; }

        [XmlAttribute(AttributeName = "psid", Form = XmlSchemaForm.None, Namespace = "")]
        public string PsID { get; set; }

        /// <summary>
        /// Gets the name of the PIC this PIC is cloned from.
        /// </summary>
        /// <value>
        /// The cloned PIC name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "clonedfrom", Form = XmlSchemaForm.None, Namespace = "")]
        public string ClonedFrom { get; set; }

        /// <summary>
        /// Gets the instruction set identifier of this PIC as a value from the <see cref="InstructionSetID"/> enumeration.
        /// </summary>
        /// <value>
        /// The instruction set as a value from <see cref="InstructionSetID"/> enumeration.
        /// </value>
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
                    if (Arch == "16xxxx") id = InstructionSetID.PIC16;
                    if (Arch == "16Exxx") id = InstructionSetID.PIC16_FULLFEATURED;
                    if (Arch == "18xxxx") id = (IsExtended ? InstructionSetID.PIC18_EXTENDED : InstructionSetID.PIC18);
                }
                return id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this PIC is belonging to the PIC18 family.
        /// </summary>
        /// <value>
        /// True if PIC18 family, false if not.
        /// </value>
        [XmlIgnore]
        public bool IsPIC18 => GetInstructionSetID >= InstructionSetID.PIC18;

        #endregion

    }

    #endregion

    #region PICCrownking Extensions

    /// <summary>
    /// Various extensions methods to manipulate PIC definitions.
    /// </summary>
    public static partial class PICCrownkingEx
    {

        private static readonly Dictionary<string, SFRBitAccess> _xlat2Access = new Dictionary<string, SFRBitAccess>()
        {
            { "x", SFRBitAccess.RW },               // read-write
            { "n", SFRBitAccess.RW },               // read-write
            { "X", SFRBitAccess.RW_Persistant },    // read-write persistent
            { "r", SFRBitAccess.ROnly },            // read-only
            { "w", SFRBitAccess.WOnly },            // write-only
            { "0", SFRBitAccess.Zero },             // 0-value
            { "1", SFRBitAccess.One },              // 1-value
            { "c", SFRBitAccess.Clr },              // clear-able only
            { "s", SFRBitAccess.Set },              // settable only
            { "-", SFRBitAccess.UnImpl },           // not implemented
        };

        /// <summary>
        /// Translates the first char of a SFR access mode string to a value from the <see cref="SFRBitAccess"/> enumeration.
        /// </summary>
        /// <param name="sAccess">The access mode string. Only first char is used.</param>
        /// <returns>
        /// A value from <see cref="SFRBitAccess"/> enumeration.
        /// </returns>
        public static SFRBitAccess SFRBitAccessMode(this string sAccess)
        {
            if (sAccess.Length >= 1)
            {
                if (_xlat2Access.TryGetValue(sAccess.Substring(1, 1), out SFRBitAccess bmode))
                    return bmode;
            }
            return SFRBitAccess.Unknown;
        }

        private static readonly Dictionary<string, SFRBitReset> _xlat2BitReset = new Dictionary<string, SFRBitReset>()
        {
            { "0", SFRBitReset.Zero },          // reset to 0.
            { "1", SFRBitReset.One },           // reset to 1.
            { "u", SFRBitReset.Unchanged },     // unchanged by reset.
            { "x", SFRBitReset.Unknown },       // unknown after reset.
            { "q", SFRBitReset.Cond },          // depends on condition.
            { "-", SFRBitReset.UnImpl },        // no implemented.
        };

        /// <summary>
        /// Translates the SFR bit reset mode string (MCLR, POR) to a value from the <see cref="SFRBitReset"/> enumeration.
        /// </summary>
        /// <param name="sReset">The reset mode string.</param>
        /// <returns>
        /// A value from <see cref="SFRBitReset"/> enumeration.
        /// </returns>
        public static SFRBitReset SFRBitResetMode(this string sReset)
        {
            if (sReset.Length >= 1)
            {
                if (_xlat2BitReset.TryGetValue(sReset.Substring(1, 1), out SFRBitReset bmode))
                    return bmode;
            }
            return SFRBitReset.Unknown;
        }

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="sPICName">Name of the PIC.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static PIC GetPIC(this PICCrownking db, string sPICName)
            => db.GetPICAsXML(sPICName)?.ToObject<PIC>();

    }

    #endregion

}


